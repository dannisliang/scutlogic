using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Model;
using System;

namespace GameServer.Model
{
    public class memoryRealInfoDataModel: MemoryEntity
    {
        public class HappyData
        {
            public int itemID { get; set; }
            public string name { get; set; }
            public int needHappyPoint { get; set; }
            public int RefleshNum { get; set; }
            public int MinuteForReflesh { get; set; }
            public int timeRefleshCng { get; set; }
            public int canReplace { get; set; }
        }

        public int id { get; set; }

        public HappyData d { get; set; }

        public memoryRealInfoDataModel()
        {
            d = new HappyData();
        }

    }
}
