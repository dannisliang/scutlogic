using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZyGames.Framework.Cache.Generic;
using GameRanking.Pack;
using GameServer.Model;
using ZyGames.Framework.Common;
using System.Xml;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Game.Runtime; // color
using System.IO;
using Game.NSNS;

namespace Game.Script
{
    public class GameConfigBase
    {
        protected ReaderWriterLockSlim _rwLock = null;
        protected const int _readWaiteTime = 1;               // ms
        protected const int _writeWaiteTime = 60 * 1000;      // 1 min
        public class openCloseData
        {
            public string key;       // Country,Province,City
            public ConfigData value; // chinaMobile,chinaUnicom,chinaTelecom
        }

        public class activityData
        {
            public bool bActibity;   // 
            public DateTime begin;   //  activity begin
            public DateTime end;     //  activity end
            public ConfigData value; //  value 
        }
        public enum EnumConfigType
        {
            ETask = 0,
            EAchievement = 1,
            EShop = 2,
            EItem = 3,
            EPackage = 4,
            ESelectStageUI = 5,
            EAfterRelive = 6,
            EChongKe = 7,
            ERecharge = 253,
            EActivity = 254,
            EIOS = 255,
        }

        protected string _name;
        protected string _path;
        protected int _defaultIntValue = -1;
        protected int logLine = 0;
        protected Dictionary<string, DateTime> _configLastWriteTime;

        public virtual void update() 
        {
            List<string> allVersions = new List<string>();
            getAllConfigFiles(allVersions);

            foreach (string version in allVersions)
            {
                string fullConfigName = getFullConfigName(version);
                System.IO.FileInfo fi = new System.IO.FileInfo(fullConfigName);

                if (fi.Exists == false) continue; // not find config file
                if (!needUpdate(fullConfigName, fi.LastWriteTime)) continue;

                try
                {
                    if (_rwLock.TryEnterWriteLock(_writeWaiteTime))
                    {
                        try
                        {
                            ConsoleLog.showNotifyInfo(version + ":配置文件更新开始", ConsoleLog.ConsoleLogType.Debug);
                            gameConfigCreate(fullConfigName, version);
                            ConsoleLog.showNotifyInfo(version + ":配置文件更新结束", ConsoleLog.ConsoleLogType.Debug);

                        }
                        catch (Exception e) { ConsoleLog.showErrorInfo(0, _name + e.Message); }
                        finally
                        {
                            if (_rwLock.IsWriteLockHeld) _rwLock.ExitWriteLock();
                        }
                    }
                }
                catch (Exception e) { ConsoleLog.showErrorInfo(0, _name + e.Message); }
            }

        }
        public virtual void init() { }

        protected virtual string getFullConfigName(string version)
        {
            return "";
        }

        protected virtual void gameConfigCreate(string fileName,string version)
        {

        }

        public GameConfigBase()
        {
            _configLastWriteTime = new Dictionary<string, DateTime>();
        }

        public virtual void getAllConfigFiles(List<string> versions)
        {
            DirectoryInfo folder = new DirectoryInfo(_path);

            foreach (FileInfo file in folder.GetFiles())
            {
                string[] datas = file.Name.Split('_');
                if (datas.Length > 1)
                {
                    string ver = datas[datas.Length - 1];
                    ver = ver.Remove(ver.LastIndexOf('.'));
                    if (versions.Contains(ver)) continue;
                    versions.Add(ver);
                }
            }
        }
        protected string getStr(string str)
        {
            return str;
        }
        protected int getInt(string str)
        {
            if ("" == str) return _defaultIntValue;
            else return int.Parse(str);
        }
        protected byte getByte(string str)
        {
            int tmp = int.Parse(str);

            if (tmp > byte.MaxValue)
            {
                ConsoleLog.showErrorInfo(0, "must be 0-255");
                return byte.MaxValue;
            }

            return byte.Parse(str);
        }

        protected ushort getUshort(string str)
        {
            int tmp = int.Parse(str);
            if (tmp > ushort.MaxValue)
            {
                ConsoleLog.showErrorInfo(0, "must be 0-65535");
                return ushort.MaxValue;
            }

            return ushort.Parse(str);
        }
        protected bool needUpdate(string key, DateTime LastWriteTime)
        {
            if (_configLastWriteTime.ContainsKey(key))
            {
                if (_configLastWriteTime[key] >= LastWriteTime) return false;
                _configLastWriteTime[key] = LastWriteTime;
            }
            else
            {
                _configLastWriteTime.Add(key, LastWriteTime);
            }
            return true;
        }

        public void setLock(ReaderWriterLockSlim rwl)
        {
            _rwLock = rwl;
        }

    }
 
    public class GameConfigMgr
    {
        ReaderWriterLockSlim _rwLock = null;
        static GameConfigMgr _instance = null;
        string TimeId = "GameConfigMgr";
        const int _readWaiteTime = 1;               // ms
        const int _writeWaiteTime = 60 * 1000;      // 1 min
        public delegate void cbFunc(List<ConfigData> data);
        private const string configFileFolderName = "Script\\CsScript\\Config\\";
        enum EnumConfigFileType
        {
            UI,
            Activity,
            OpenClose,
            ExchangeCode,
            NotSupportVersion,
            ConstValues,
            RankReward,
            Products,
            RealItems,
        }

        Dictionary<int, GameConfigBase> _configDic;

       public string getString(string key,string defaultValue="")
        {
           try
           {
               GameConfigValue gcv = _configDic[(int)EnumConfigFileType.ConstValues] as GameConfigValue;
               string str = gcv.getData(key);
               if (string.IsNullOrEmpty(str))
               {
                   ConsoleLog.showErrorInfo(0, string.Format("getConfigValue:Key:{0}", key));
                   return defaultValue;
               }
               return str; 
           }
           catch
           {
               ConsoleLog.showErrorInfo(0,"getString:"+key);
               return defaultValue;
           }
       }

       public int getInt(string key,int defaultValue=0)
       {
           try
           {
               return Convert.ToInt32(getString(key));
           }
           catch
           {
               ConsoleLog.showErrorInfo(0, "getInt:" + key);
               return defaultValue;
           }
       }

       public float getfloat(string key,float defaultValue=0)
       {
           try
           {
               return (float)Convert.ToDouble(getString(key));
           }
           catch
           {
               ConsoleLog.showErrorInfo(0, "getfloat:" + key);
               return defaultValue;
           }
       }


        public static GameConfigMgr Instance()
        {
            if (null == _instance)
            {
                _instance = new GameConfigMgr();
                _instance._rwLock = new ReaderWriterLockSlim();
                _instance.Init();
            }
            return _instance;
        }
        public void Start() { }
        public GameConfigRankingReward.rewardData getRankReward(int index)
        {
            if(_rwLock.TryEnterReadLock(_readWaiteTime))
            {
                try
                {
                    GameConfigRankingReward rankReward = _configDic[(int)EnumConfigFileType.RankReward] as GameConfigRankingReward;
                    return rankReward.get(index);
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                }
            }
            else
            {
                GameConfigRankingReward rankReward = _configDic[(int)EnumConfigFileType.RankReward] as GameConfigRankingReward;
                return rankReward._defaultValue;   
            }
        }
        public void getData(cbFunc func, string version, string ip = "")
        {
            try
            {
                if(_rwLock.TryEnterReadLock(_readWaiteTime))
                {
                    try
                    {
                        func(getConfigByVersion(version, ip));
                    }
                    finally
                    {
                        if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleLog.showErrorInfo(0,"GameConfigMgr getData"+e.Message);
            }
        }

        public bool isSupportVersion(string version)
        {
            if(_rwLock.TryEnterReadLock(_readWaiteTime))
            {
                try
                {
                    GameConfigNotSupportVersion v = _configDic[(int)EnumConfigFileType.NotSupportVersion] as GameConfigNotSupportVersion;
                    return v.isSupport(version);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                }
            }
            else
            {
                return false;
            }
        }

        List<ConfigData> getConfigByVersion(string version, string ip)
        {
            List<ConfigData> newCD = new List<ConfigData>();

            // UI
            //GameConfigUI gcu = _configDic[(int)EnumConfigFileType.UI] as GameConfigUI;
            //gcu.getDate(newCD, version);
            NewGameConfig.Singleton().getConfigUIData(newCD, version);
            

            // activity
            //GameConfigActivity gca = _configDic[(int)EnumConfigFileType.Activity] as GameConfigActivity;
            //gca.getDate(newCD, version);
            NewGameConfig.Singleton().getActivityDate(newCD, version);

                
            // openClose
            //GameConfigOpenClose gcoc = _configDic[(int)EnumConfigFileType.OpenClose] as GameConfigOpenClose;
            //gcoc.getDate(newCD, version, ip);
            NewGameConfig.Singleton().getPaySwitchData(newCD, version, ip);

            return newCD;
        }

        void Init()
        {
            IP.Start();
            _configDic = new Dictionary<int, GameConfigBase>();
            TimerMgr.Singleton().add(TimeId, update, 1000, 10000);

            GameConfigValue constValue = new GameConfigValue(configFileFolderName + EnumConfigFileType.ConstValues.ToString());
            constValue.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.ConstValues, constValue);

            GameConfigUI gcu =  new GameConfigUI(configFileFolderName + EnumConfigFileType.UI.ToString());
            gcu.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.UI,gcu);

            GameConfigActivity Activity = new GameConfigActivity(configFileFolderName + EnumConfigFileType.Activity.ToString());
            Activity.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.Activity, Activity);

            GameConfigOpenClose OpenClose = new GameConfigOpenClose(configFileFolderName + EnumConfigFileType.OpenClose.ToString());
            OpenClose.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.OpenClose, OpenClose);

            GameConfigNotSupportVersion NotSupportVersion = new GameConfigNotSupportVersion(configFileFolderName + EnumConfigFileType.NotSupportVersion.ToString());
            NotSupportVersion.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.NotSupportVersion, NotSupportVersion);

            GameConfigExchangeCode exchangeCode = new GameConfigExchangeCode(configFileFolderName + EnumConfigFileType.ExchangeCode.ToString());
            exchangeCode.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.ExchangeCode, exchangeCode);

         
            GameConfigRankingReward rankingReward = new GameConfigRankingReward(configFileFolderName + EnumConfigFileType.RankReward.ToString());
            rankingReward.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.RankReward, rankingReward);

            GameConfigProducts items = new GameConfigProducts(configFileFolderName + EnumConfigFileType.Products.ToString());
            items.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.Products, items);


            GameConfigHappyPoint happyPoint = new GameConfigHappyPoint(configFileFolderName+EnumConfigFileType.RealItems.ToString());
            happyPoint.setLock(_rwLock);
            _configDic.Add((int)EnumConfigFileType.RealItems,happyPoint);

            foreach (GameConfigBase gcb in _configDic.Values)
            {
                gcb.init();
            }
            update(null);
        }

        public bool ActivityIsOpen(string version,int activityId)
        {
            try
            {
                if(_rwLock.TryEnterReadLock(_readWaiteTime))
                {
                    try
                    {
                        GameConfigActivity config = _configDic[(int)EnumConfigFileType.Activity] as GameConfigActivity;
                        return config.isOpen(version, activityId);
                    }
                    finally
                    {
                        if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ConsoleLog.showErrorInfo(0, "GameConfigMgr getData" + e.Message);
                return false;
            }
        }

        //public memoryHappyModeDataModel.HappyData getHappyData(int realItemId)
        //{
        //    GameConfigHappyPoint config = _configDic[(int)EnumConfigFileType.RealItems] as GameConfigHappyPoint;
         //   return config.getRealItemInfo(realItemId);
       //  }

        public memoryRealInfoDataModel.HappyData getHappyData(int realItemId)
        {
            var cache = new RealInfoDataModelMemoryCache();
            return cache.getRealItemInfo(realItemId);
        }

        public List<int> getHappyDataKeys()
        {
           // GameConfigHappyPoint config = _configDic[(int)EnumConfigFileType.RealItems] as GameConfigHappyPoint;
           // return config.getKeys();
            var cache = new RealInfoDataModelMemoryCache();
            return cache.getKeys();
        }
        public string getProductInfo(string productID,string orderID)
        {
            GameConfigProducts gci = _configDic[(int)EnumConfigFileType.Products] as GameConfigProducts;
            string info = gci.getProductItems(productID,orderID);
            return info;
        }
        void update(object obj)
        {
            try
            {
                foreach (GameConfigBase gcb in _configDic.Values)
                {
                    gcb.update();
                }
            }
            catch (Exception e)
            {
                ConsoleLog.showErrorInfo(0, e.Message);
            }
        }
    }
}// namespace GameServer.CSScript
