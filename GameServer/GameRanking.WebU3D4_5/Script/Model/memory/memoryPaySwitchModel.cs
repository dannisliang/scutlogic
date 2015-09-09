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
    public class memoryPaySwitchModel : MemoryEntity
    {
        public string version { get; set; }

        public Dictionary<string, ConfigData> keyVal;

        public static ConfigData getConfigData()
        {
            ConfigData cd = new ConfigData();
            cd.type = 253;
            cd.ext = new List<ushort>();
            return cd;
        }
        public memoryPaySwitchModel()
        {
            keyVal = new Dictionary<string, ConfigData>();
        }

    }
}
