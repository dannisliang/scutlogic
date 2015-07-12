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
    
    public class GameConfigNotSupportVersion : GameConfigBase
    {
        List<string>  _versionNotSupport;
       // MemoryCacheStruct<GameServer.Model.NotSupportVersionEntity> _memoryData;
        public override void init()
        {
            base.init();
            _versionNotSupport = new List<string>();
        }

        public bool isSupport(string version)
        {
            return ( false == _versionNotSupport.Contains(version) );
        }
        public GameConfigNotSupportVersion() { }
        public GameConfigNotSupportVersion(string path)
        {
            _path = path;
            _name = path + "\\config1002_";
        }

        protected override string getFullConfigName(string version)
        {
            return _name + "version.txt";
        }

        protected override void gameConfigCreate(string fileName, string version)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            string line = "";
            _versionNotSupport.Clear();
            while ((line = stream.ReadLine()) != null)
            {
                // TODO
                if (!_versionNotSupport.Contains(line)) _versionNotSupport.Add(line);
            }
            ConsoleLog.showNotifyInfo("#####NotSupportVersion######");
            foreach (string s in _versionNotSupport)
            {
                ConsoleLog.showNotifyInfo(s);
            }
            stream.Close();

            string memoryId = typeof(GameConfigNotSupportVersion).ToString();
        }
    }
    public class GameConfigOpenClose : GameConfigBase
    {
        Dictionary<string, Dictionary<string, ConfigData>> _versionDataDic;
        private const string _defaultOpenCloseKey = "中国,,";

        public override void init()
        {
            base.init();
            _versionDataDic = new Dictionary<string, Dictionary<string, ConfigData>>();
        }

        public GameConfigOpenClose() { }
        public GameConfigOpenClose(string path)
        {
            _path = path ;
            _name = path + "\\config1002_openclose_";
        }

        public void getDate(List<ConfigData> outData, string version,string ip)
        {
             ConfigData ocD = getRechargeData(version, ip);
             outData.Add(ocD);
        }

        ConfigData getRechargeData(string version, string ip)
        {
            string ipKey = IP.FindQuit(ip);
            ipKey = FormatOpenCloseKey(version, ipKey);
            //ConsoleLog.showNotifyInfo(ipKey + ":" + ip);

            if (false == _versionDataDic.ContainsKey(version))
            {
                version = "error";
            }
            Dictionary<string, ConfigData> ocd = _versionDataDic[version];
            if (false == ocd.ContainsKey(ipKey)) ipKey = _defaultOpenCloseKey;

            return ocd[ipKey];
        }
        string FormatOpenCloseKey(string version, string key)
        {
            if (false == _versionDataDic.ContainsKey(version))
            {
                version = "error";
            }

            if (false == _versionDataDic.ContainsKey(version)) return _defaultOpenCloseKey;

            Dictionary<string, ConfigData> ocd = _versionDataDic[version];
            if (ocd.ContainsKey(key)) return key;
            else
            {
                string[] tmp = key.Split(',');
                string newKey = "";
                if (tmp.Length == 3) // length = 3
                {
                    newKey = tmp[0] + "," + tmp[1] + ",";
                    //showNotifyInfo(newKey);
                    if (ocd.ContainsKey(newKey))
                        return newKey; // 2
                }

                if (tmp.Length > 1)  // length = 2 / 3
                {
                    newKey = tmp[0] + "," + ",";
                    if (ocd.ContainsKey(newKey))
                        return newKey; // 1
                }
            }

            return key;
        }
        void addConfigData(ConfigData value, ushort chinaModile, ushort chinaUnicom, ushort chinaTelecom)
        {
            value.ext.Add(chinaModile);
            value.ext.Add(chinaUnicom);
            value.ext.Add(chinaTelecom);
        }
        bool getOpenCloseDataByLine(Dictionary<string, ConfigData> openCloseDataLst, string line,string preCountry,string preProvince)
        {
            string country = preCountry;
            string province =  preProvince;
            string[] strData = line.Split('\t');
            if (getStr(strData[0]) != "") country = getStr(strData[0]);
            if (getStr(strData[1]) != "") province = getStr(strData[1]);
            string city = getStr(strData[2]);
            string key = openCloseKey(country, province, city);

            bool parseOk = true;
            if (strData.Length % 3 != 0)
            {
                ConsoleLog.showErrorInfo(0, "lineCnt%3!=0 line:" + line);
                return false;
            }
            ConfigData value = new ConfigData();
            value.ext = new List<ushort>();
            value.type = (byte)EnumConfigType.ERecharge;
            for (int i = 3; i < strData.Length; i += 3)
            {
                int chinaMobile = getInt(strData[i]);        //mobile
                int chinaUnicom = getInt(strData[i + 1]);      //unicom
                int chinaTelecom = getInt(strData[i + 2]);     //tecele


                bool bcm = checkChinaMoblie(chinaMobile);
                bool bcu = checkChinaUnicom(chinaUnicom);
                bool bct = checkChinaTelecom(chinaTelecom);

                if (false == bcm || false == bcu || false == bct)
                {
                    parseOk = false;
                }


                if (-1 == chinaMobile || -1 == chinaUnicom || -1 == chinaTelecom)
                {
                    string upKey = "";
                    if ("" != city && province != "" && country != "")
                    {
                        upKey = openCloseKey(country, province, "");
                    }
                    else if ("" == city && province != "" && country != "")
                    {
                        upKey = openCloseKey(country, "", "");
                    }
                    else
                    {
                        upKey = _defaultOpenCloseKey;
                        TraceLog.WriteError("openClose.txt Error! country not set");
                    }

                    if (openCloseDataLst.ContainsKey(upKey))
                    {
                        // udate this....
                        ConfigData tmp = openCloseDataLst[upKey];
                        if (-1 == chinaMobile) chinaMobile   = tmp.ext[i - 3];
                        if (-1 == chinaUnicom) chinaUnicom   = tmp.ext[i - 2];
                        if (-1 == chinaTelecom) chinaTelecom = tmp.ext[i - 1];
                    }
                    else
                    {
                        TraceLog.WriteError("openClose.txt Error! (key,upKey)=" + key + ":" + upKey);
                    }
                }

                addConfigData(value, (ushort)chinaMobile, (ushort)chinaUnicom, (ushort)chinaTelecom);
            }
            if(openCloseDataLst.ContainsKey(key))
            {
                ConsoleLog.showErrorInfo(0, "why...");
            }
            openCloseDataLst.Add(key, value);
            return parseOk;
        }

        protected  override void gameConfigCreate(string fileName, string version)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            string line = "";
            string country = "";
            string province = "";

            int count = 0;
            Dictionary<string, ConfigData> openCloseDataLst = new Dictionary<string, ConfigData>();
            while ((line = stream.ReadLine()) != null)
            {
                count++;
                logLine = count;
                if (count < 3) continue;
                if (line == "") continue;

                string[] strData = line.Split('\t');
                if (getStr(strData[0]) != "") country = getStr(strData[0]);
                if (getStr(strData[1]) != "") province = getStr(strData[1]);
                string city = getStr(strData[2]);

                if (false == getOpenCloseDataByLine(openCloseDataLst, line, country,province))
                {
                    ConsoleLog.showErrorInfo(logLine, "version:" + version + "#fileName:" + fileName);
                }
            }

            if (_versionDataDic.ContainsKey(version))
            {
                ConsoleLog.showNotifyInfo(fileName + " Update Success");
                _versionDataDic[version] = openCloseDataLst;
            }
            else
            {
                ConsoleLog.showNotifyInfo(fileName + " Parse Success");
                _versionDataDic.Add(version, openCloseDataLst);
            }
            stream.Close();
        }

        bool checkChinaMoblie(int value)
        {
            if (_defaultIntValue != value &&
                0 != value &&
                1 != value &&
                2 != value &&
                3 != value)
            {
                ConsoleLog.showErrorInfo(logLine, "chinaMoblie(0,1,2,3)not:" + value);
                return false;
            }
            return true;
        }

        bool checkChinaUnicom(int value)
        {
            if (_defaultIntValue != value &&
            0 != value &&
            11 != value &&
            12 != value &&
            3 != value)
            {
                ConsoleLog.showErrorInfo(logLine, "ChinaUnicom(0,11,12,3)not:" + value);
                return false;
            }
            return true;
        }

        bool checkChinaTelecom(int value)
        {
            if (_defaultIntValue != value &&
             0 != value &&
             21 != value &&
             3 != value)
            {
                ConsoleLog.showErrorInfo(logLine, "ChinaTelecom(0,21,3)not:" + value);
                return false;
            }
            return true;
        }

        protected override string getFullConfigName(string version)
        {
            return _name + version + ".txt";
        }

        private string openCloseKey(string Country, string Province, string City)
        {
            string key = _defaultOpenCloseKey;
            if ("" != Country || "" != Province || "" != City)
                key = Country + "," + Province + "," + City;
            return key;
        }
    }
    
    public class GameConfigActivity : GameConfigBase
    {
        Dictionary<string, List<activityData>> _versionDataDic;

        public override void init()
        {
            base.init();
            _versionDataDic = new Dictionary<string, List<activityData>>();
        }
        public GameConfigActivity() { }
        public GameConfigActivity(string path)
        {
            _path = path;
            _name = path + "\\config1002_activity_";
        }
        public void getDate(List<ConfigData> outData, string version)
        {
            if (false == _versionDataDic.ContainsKey(version)) return;

            foreach (activityData data in _versionDataDic[version])
            {
                if (data.bActibity)
                {
                    var value = data.value;
                    DateTime now = DateTime.Now;
                    ConfigData cd = new ConfigData();
                    cd.ext = new List<ushort>();
                    cd.type = value.type;
                    foreach (ushort us in value.ext)
                        cd.ext.Add(us);
                    TimeSpan span = data.end - now;
                    cd.ext.Add((ushort)span.Days);
                    cd.ext.Add((ushort)span.Hours);
                    cd.ext.Add((ushort)span.Minutes);
                    cd.ext.Add((ushort)span.Seconds);
                    cd.ext.Add((ushort)data.begin.Month);
                    cd.ext.Add((ushort)data.begin.Day);
                    cd.ext.Add((ushort)data.begin.Hour);
                    cd.ext.Add((ushort)data.begin.Minute);
                    cd.ext.Add((ushort)data.end.Month);
                    cd.ext.Add((ushort)data.end.Day);
                    cd.ext.Add((ushort)data.end.Hour);
                    cd.ext.Add((ushort)data.end.Minute);
                    outData.Add(cd);
                }
            }
        }

        public override void update()
        {
            base.update();
            ActivityCheck();
        }

        protected override void gameConfigCreate(string fileName, string version)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            string line = "";

            int count = 0;
            List<activityData> activityDataLst = new List<activityData>();
            while ((line = stream.ReadLine()) != null)
            {
                count++;
                if (count < 3) continue;
                if (line == "") continue;

                string[] strData = line.Split('\t');

                activityData data = new activityData();
                data.value = new ConfigData();
                data.value.ext = new List<ushort>();

                data.bActibity = false;
                data.value.type = (byte)EnumConfigType.EActivity; // type of Data
                try
                {
                    data.begin = DateTime.Parse(strData[1]);// date
                    data.end = DateTime.Parse(strData[2]); // date
                }
                catch (Exception e)
                {
                    ConsoleLog.showErrorInfo(count, e.ToString() + " \nDataTime Set Error...:" + strData[1] + ":" + strData[2]);
                }


                data.value.ext.Add(getUshort(strData[0])); // activity ID
                string[] pars = strData[3].Split(',');
                for (int i = 0; i < pars.Length; ++i)
                {
                    data.value.ext.Add(getUshort(pars[i]));
                }

                activityDataLst.Add(data);
            }

            if (_versionDataDic.ContainsKey(version))
            {
                _versionDataDic[version] = activityDataLst;
                ConsoleLog.showNotifyInfo(fileName + " Update Success");
            }
            else
            {
                _versionDataDic.Add(version, activityDataLst);
                ConsoleLog.showNotifyInfo(fileName + " Parse Success");
            }
            stream.Close();
        }


        protected override string getFullConfigName(string version)
        {
            return _name + version + ".txt";
        }

        public bool isOpen(string version,int activityID)
        {
            if (false == _versionDataDic.ContainsKey(version)) return false;

            foreach(var k in _versionDataDic[version])
            {
                if (k.bActibity && k.value.ext[0] == (byte)activityID)
                    return true;
            }
            
            return false;
        }
        void ActivityCheck()
        {
            foreach (var v in _versionDataDic.Values)
            {
                foreach (var l in v)
                {
                    if (DateTime.Now > l.begin && DateTime.Now < l.end)
                        l.bActibity = true;
                    else
                        l.bActibity = false;
                }
            }
        }

        bool checkChinaMoblie(int value)
        {
            if (_defaultIntValue != value &&
                0 != value &&
                1 != value &&
                2 != value &&
                3 != value)
            {
                ConsoleLog.showErrorInfo(logLine, "chinaMoblie(0,1,2,3)not:" + value);
                return false;
            }
            return true;
        }

        bool checkChinaUnicom(int value)
        {
            if (_defaultIntValue != value &&
            0 != value &&
            11 != value &&
            12 != value &&
            3 != value)
            {
                ConsoleLog.showErrorInfo(logLine, "ChinaUnicom(0,11,12,3)not:" + value);
                return false;
            }
            return true;
        }

        bool checkChinaTelecom(int value)
        {
            if (_defaultIntValue != value &&
             0 != value &&
             21 != value &&
             3 != value)
            {
                ConsoleLog.showErrorInfo(logLine, "ChinaTelecom(0,21,3)not:" + value);
                return false;
            }
            return true;
        }

    }

    public class GameConfigUI : GameConfigBase
    {

        Dictionary<string, List<ConfigData>> _versionDataDic;

        public override void init()
        {
            base.init();
            _versionDataDic = new Dictionary<string, List<ConfigData>>();
        }
        public GameConfigUI() { }
        public GameConfigUI(string path)
        {
            _path = path;
            _name = _path + "\\config1002_";
        }

        public void getDate(List<ConfigData> outData, string version)
        {
            if (false == _versionDataDic.ContainsKey(version))
                version = "error";

            foreach (ConfigData cd in _versionDataDic[version])
            {
                outData.Add(cd);
            }
        }
        protected override string getFullConfigName(string version)
        {
            return _name + version + ".txt";
        }
       protected override void gameConfigCreate(string fileName, string version)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            string line = "";

            int count = 0;
            List<ConfigData> ConfigDataLst = new List<ConfigData>();
            while ((line = stream.ReadLine()) != null)
            {
                count++;
                if (count < 3) continue;
                if (line == "") continue;
                string[] strData = line.Split('\t');

                ConfigData data = new ConfigData();
                data.ext = new List<ushort>();
                data.type = getByte(strData[0]);
                string[] extString = getStr(strData[4]).Split(',');
                for (int i = 0; i < extString.Length; ++i)
                {
                    data.ext.Add(getUshort(extString[i]));
                }
                checkGameConfig(data);
                ConfigDataLst.Add(data);
            }
            if (_versionDataDic.ContainsKey(version))
            {
                _versionDataDic[version] = ConfigDataLst;
                ConsoleLog.showNotifyInfo(fileName + " Update Success");
            }
            else
            {
                _versionDataDic.Add(version, ConfigDataLst);
                ConsoleLog.showNotifyInfo(fileName + " Parse Success");
            }
            stream.Close();
        }
        bool checkGameConfig(ConfigData d)
        {
            if (d.ext != null)
            {
                if (d.ext[0] > ushort.MaxValue || d.ext[0] < 0)
                    ConsoleLog.showErrorInfo(logLine, "CheckGameConfig Error :" + d.type + "," + d.ext[0]);
            }

            return true;
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
            GameConfigUI gcu = _configDic[(int)EnumConfigFileType.UI] as GameConfigUI;
            gcu.getDate(newCD, version);

            // activity
            GameConfigActivity gca = _configDic[(int)EnumConfigFileType.Activity] as GameConfigActivity;
            gca.getDate(newCD, version);

            // openClose
            GameConfigOpenClose gcoc = _configDic[(int)EnumConfigFileType.OpenClose] as GameConfigOpenClose;
            gcoc.getDate(newCD, version, ip);

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

        public GameConfigHappyPoint.HappyData getHappyData(int realItemId)
        {
            GameConfigHappyPoint config = _configDic[(int)EnumConfigFileType.RealItems] as GameConfigHappyPoint;
            return config.getRealItemInfo(realItemId);
        }

        public List<int> getHappyDataKeys()
        {
            GameConfigHappyPoint config = _configDic[(int)EnumConfigFileType.RealItems] as GameConfigHappyPoint;
            return config.getKeys();
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
