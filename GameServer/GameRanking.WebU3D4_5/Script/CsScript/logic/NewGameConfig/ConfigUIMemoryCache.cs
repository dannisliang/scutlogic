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
    public class ConfigUIMemoryCache : MemoryCacheStruct<memoryConfigUIModel>
    {
        public void getDate(List<ConfigData> outData, string version)
        {
            this.Foreach((string s,memoryConfigUIModel mcui)=>{

                if (mcui.version == version)
                    outData.Add(mcui.cd);

                return true;
            
            });
        }
        protected override bool InitCache()
        {
            var dbProvider = DbConnectionProvider.FindFirst().Value;
            var command = dbProvider.CreateCommandStruct(typeof(ConfigUIModel).ToString(), CommandMode.Inquiry);
            command.Filter = dbProvider.CreateCommandFilter();
            //command.Filter.Condition = ""
            command.Parser();
            using (var read = dbProvider.ExecuteReader(System.Data.CommandType.Text, "select * from ConfigUIModel"))
            {
                while (read.Read())
                {
                    var t = new memoryConfigUIModel();
                    t.id = int.Parse(read["id"].ToString());
                    t.cd.type = byte.Parse(read["type"].ToString());
                    t.version = read["version"].ToString();

                    string parms = read["parms"].ToString();
                    string[] p = parms.Split(',');
                    foreach(string v in p)
                    {
                        ushort d = ushort.Parse(v);
                        t.cd.ext.Add(d);
                    }
                    this.AddOrUpdate(t.id.ToString(), t);
                }

            }

            return true;
        }
    }
}
