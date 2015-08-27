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
    public class RealInfoDataModelMemoryCache : MemoryCacheStruct<memoryRealInfoDataModel>
    {
        public List<int> getKeys()
        {
            //TODO:
            List<int> keys = new List<int>();

            this.Foreach((string s, memoryRealInfoDataModel mhmdm) => {
                keys.Add(mhmdm.d.itemID);
                return true;
            });

            return keys;
        }



        public memoryRealInfoDataModel.HappyData getRealItemInfo(int id)
        {
            //TODO:

            memoryRealInfoDataModel hd = null;
            hd = Find((memoryRealInfoDataModel d) => {

                return d.d.itemID == id; 
            
            });
            if (null != hd) return hd.d;//refrence ... maybe carefull...however we do not change this data in game.
            return null;
        }

        protected override bool InitCache()
        {
            var dbProvider = DbConnectionProvider.FindFirst().Value;
            var command = dbProvider.CreateCommandStruct(typeof(RealInfoDataModel).ToString(), CommandMode.Inquiry);
            command.Filter = dbProvider.CreateCommandFilter();
            //command.Filter.Condition = ""
            command.Parser();
            using (var read = dbProvider.ExecuteReader(System.Data.CommandType.Text, "select * from RealInfoDataModel"))
            {
                while (read.Read())
                {
                    var t = new memoryRealInfoDataModel();
                    t.id = int.Parse(read["id"].ToString());
                    t.d.itemID = int.Parse(read["itemid"].ToString());
                    t.d.name = read["name"].ToString();
                    t.d.needHappyPoint = int.Parse(read["needhappypoint"].ToString());
                    t.d.RefleshNum= int.Parse(read["refleshNum"].ToString());
                    t.d.MinuteForReflesh = int.Parse(read["minuteforreflesh"].ToString());
                    t.d.timeRefleshCng = int.Parse(read["timerefleshcng"].ToString());
                    t.d.canReplace = int.Parse(read["canreplace"].ToString());
                    this.AddOrUpdate(t.id.ToString(), t);
                }

            }

            return true;
        }
    }
}
