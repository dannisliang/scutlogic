
/****************************************************************************
    File:		NewGameConfig.cs
    Desc:	    1：使用数据库来维护所有的配置文件。
                   提供mysql配置文件的读取接口。修改使用web后台做。
                2：不提供任何cache修改配置文件的接口。
                
    Date:		2015-7-21 11:11:11
    Author:		guccang
    URL:		http://guccang.github.io
    Email:		guccang@126.com
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.NSNS;
using GameServer.Model;
using GameRanking.Pack;
using ZyGames.Framework.Cache.Generic;

namespace Game.Script
{

    /*
     * Class:   NewGameConfig.cs
     * Desc:    实现类	
     * Author：	guccang
     * Date：	2015-08-21 11:11:11
     */
    /// <summary>
    /// NewGameConfig.cs Document
    /// </summary>
    
    public class NewGameConfig
    {
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
        static NewGameConfig ins = null;

        public static NewGameConfig Singleton()
        {
            if(null == ins)
            {
                ins = new NewGameConfig();
                ins.Init();
            }
            return ins;
        }


        public void CreateDBTable()
        {
            // create in Init method
        }

        void Init()
        {
            new ShareCacheStruct<ActivityModel>();
            new ShareCacheStruct<RealInfoDataModel>();
            new ShareCacheStruct<ConfigUIModel>();
            new ShareCacheStruct<PaySwitchModel>();

            restore("activitymodel", "10000");
            restore("happydatamodel", "10000");
            restore("configuimodel", "10000");
            restore("payswitchmodel", "10000"); ;
        }

        public bool restore(string tableName,string parms_num)
        {
            if("activitymodel"==tableName)
            {
                toMemoryActivityModel();
            }
            else if("happydatamodel"==tableName)
            {
                toMemoryHappyDataModel();
            }
            else if("configuimodel"==tableName)
            {
                toMemoryConfigUIModel();
            }
            else if ("configpayswitchmodel" == tableName)
            {
                toMemoryConfigPaySwitchModel();
            }
            else 
            {
                return false;
            }

            return true;
        }
        public void toMemoryConfigPaySwitchModel()
        {
            CacheFactory.tryRemoveMemoryCache(typeof(memoryPaySwitchModel).ToString());
            new PaySwitchMemoryCache();
        }
        public void toMemoryConfigUIModel()
        {
            CacheFactory.tryRemoveMemoryCache(typeof(memoryConfigUIModel).ToString());
            new ConfigUIMemoryCache();
        }
        public void toMemoryHappyDataModel()
        {
            CacheFactory.tryRemoveMemoryCache(typeof(memoryRealInfoDataModel).ToString());
            new RealInfoDataModelMemoryCache();

        }

        public void toMemoryActivityModel()
        {
            CacheFactory.tryRemoveMemoryCache(typeof(memoryActivityModel).ToString());
            var memoryCache = new ActivityModelMemoryCache(); 
        }

        public void getConfigUIData(List<ConfigData> outData,string version)
        {
            var cache = new ConfigUIMemoryCache();
            cache.Foreach((string s, memoryConfigUIModel ui) =>
            {
                if (ui.version == version)
                    outData.Add(ui.cd);

                return true;
            
            });
        }

        public void getPaySwitchData(List<ConfigData> outData,string version,string ip)
        {
            var cache = new PaySwitchMemoryCache();
            cache.getDate(outData, version, ip);
        }

        public void getActivityDate(List<ConfigData> outData, string version)
        {
            DateTime now = DateTime.Now;
            var cache = new ActivityModelMemoryCache(); 
            cache.Foreach((string s, memoryActivityModel mam) =>
            {
                if (mam.version != version || false == mam.isOpen())
                    return true; // next

                var data = mam;
                ConfigData cd = new ConfigData();
                cd.ext = new List<ushort>();
                cd.type = mam.type;
                cd.ext.Add(mam.activityid);
                foreach (ushort us in data.parms)
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
                return true;
            });
        }
     }
}
