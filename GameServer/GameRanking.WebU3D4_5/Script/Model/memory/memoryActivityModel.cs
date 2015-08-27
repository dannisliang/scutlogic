using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Model;
using System;

namespace GameServer.Model
{
    public class memoryActivityModel : MemoryEntity 
    {
        public int id { get; set; }
        public byte type { get; private set; }
        public ushort activityid { get; set; }

        public string version { get; set; }

        public DateTime begin { get; set; }

        public DateTime end { get; set; }

        public List<ushort> parms { get; set; }

        public memoryActivityModel()
        {
            parms = new List<ushort>();
            type = 254; 
        }

        public bool isOpen(){

            return (DateTime.Now>begin&&DateTime.Now<end);
        }
    }
}
