using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;
using ZyGames.Framework.Data;

namespace Game.Script
{
    public class ActivityModelMemoryCache : MemoryCacheStruct<memoryActivityModel>
    {
        protected override bool InitCache()
        {
            var dbProvider = DbConnectionProvider.FindFirst().Value;
            var command = dbProvider.CreateCommandStruct(typeof(ActivityModel).ToString(), CommandMode.Inquiry);
            command.Filter = dbProvider.CreateCommandFilter();
            //command.Filter.Condition = ""
            command.Parser();
            using (var read = dbProvider.ExecuteReader(System.Data.CommandType.Text,"select * from ActivityModel"))
            {
                while(read.Read())
                {
                    var t = new memoryActivityModel();
                    t.id = int.Parse(read["id"].ToString());
                    t.activityid = ushort.Parse(read["activityid"].ToString());
                    t.version = read["version"].ToString();
                    t.begin = DateTime.Parse(read["begin"].ToString());
                    t.end = DateTime.Parse(read["end"].ToString());
                    string parms = read["parms"].ToString();
                    string[] vals = parms.Split(',');

                    foreach(string d in  vals)
                    {
                        ushort u = ushort.Parse(d);
                        t.parms.Add(u);
                    }

                    this.AddOrUpdate(t.id.ToString(), t);
                }

            }
                
            return true;
        }
    }
}
