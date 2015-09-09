using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;
using ZyGames.Framework.Data;
using GameRanking.Pack;

namespace Game.Script
{
    public class PaySwitchMemoryCache : MemoryCacheStruct<memoryPaySwitchModel>
    {
        public void getDate(List<ConfigData> outData, string version,string ip)
        {
            string defaultKey = "中国,,";
            string defaultVersino = "error";
            string ipKey = IP.FindQuit(ip);
            memoryPaySwitchModel muic = null;
            this.TryGet(version, out muic);
            if (muic == null)
                this.TryGet(defaultVersino,out muic);
            if (muic == null)
                return;

            string key = defaultKey;
            if (muic.keyVal.ContainsKey(ipKey))
            {
                key = ipKey;
            }
            outData.Add(muic.keyVal[key]);
        }
        
        protected override bool InitCache()
        {
            var dbProvider = DbConnectionProvider.FindFirst().Value;
            var command = dbProvider.CreateCommandStruct(typeof(PaySwitchModel).ToString(), CommandMode.Inquiry);
            command.Filter = dbProvider.CreateCommandFilter();
            //command.Filter.Condition = ""
            command.Parser();
            using (var read = dbProvider.ExecuteReader(System.Data.CommandType.Text, "select * from PaySwitchModel"))
            {
                while (read.Read())
                {
                    memoryPaySwitchModel mpm = null;
                    string version = read["version"].ToString();
                    string ipKey = read["ipKey"].ToString();
                    TryGet(version, out mpm);
                    if (mpm == null)
                    {
                        mpm = new memoryPaySwitchModel();
                        mpm.version = version;
                        this.AddOrUpdate(version, mpm);
                    }
                    var cd = memoryPaySwitchModel.getConfigData();
                    cd.ext.Add(ushort.Parse(read["ChinaMobile"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaUnicom"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaTelecom"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaMobile_360"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaUnicom_360"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaTelecom_360"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaMobile_BD"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaUnicom_BD"].ToString()));
                    cd.ext.Add(ushort.Parse(read["ChinaTelecom_BD"].ToString()));
                    mpm.keyVal.Add(ipKey, cd);
                }

            }

            return true;
        }
    }
}
