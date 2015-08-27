using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Model;
using System;
using GameRanking.Pack;

namespace GameServer.Model
{
    public class memoryConfigUIModel: MemoryEntity
    {
        public int id { get; set; }

        public string version { get; set; }

        public ConfigData cd { get; set; }

        public memoryConfigUIModel()
        {
            cd = new ConfigData();
            cd.ext = new List<ushort>();
        }
    }
}
