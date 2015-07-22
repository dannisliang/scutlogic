using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Log;
using GameServer.Model;

namespace Game.Script
{
    public class GameConfigRankingReward : GameConfigBase
    {
        public class rewardData
        {
            public int rank;
            public int Diamonds;
            public int Score;
        }

        List<rewardData> _lst;
        public rewardData _defaultValue;
        public override void init()
        {
            base.init();
            _lst = new List<rewardData>();
            _defaultValue = new rewardData();
            _defaultValue.rank = 0;
            _defaultValue.Diamonds = 0;
            _defaultValue.Score = 0;
        }

        public class comp : IComparer<rewardData>
        {
            public int Compare(rewardData x,rewardData y)
            {
                return x.rank.CompareTo(y.rank);
            }
        }
        public rewardData get(int index)
        {
            rewardData rd = new rewardData();
            rd.rank = index;
            int pos = _lst.BinarySearch(rd, new comp());
            if(pos<0)
            {
                pos = ~pos;
            }
            if (pos > _lst.Count-1)
            {
                rd.rank = _defaultValue.rank;
                rd.Diamonds = _defaultValue.Diamonds;
                rd.Score = _defaultValue.Score;
            }
            else
            {
                rd.rank = index;
                rd.Score = _lst[pos].Score;
                rd.Diamonds = _lst[pos].Diamonds;
            }
            return rd;
        }
        
        public GameConfigRankingReward() { }

        public GameConfigRankingReward(string path)
        {
            _path = path;
            _name = path + "//";
        }
        public override void getAllConfigFiles(List<string> versions)
        {
            DirectoryInfo folder = new DirectoryInfo(_path);

            foreach (FileInfo file in folder.GetFiles())
            {
                string name = file.Name;
                name = name.Remove(name.LastIndexOf('.'));
                if (versions.Contains(name)) continue;
                versions.Add(name);
            }
        }

        protected override void gameConfigCreate(string fileName, string name)
        {
            var cache = new ShareCacheStruct<ExchangeCode>();
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            _lst.Clear();
            string line = "";
            int index = 0;
            while ((line = stream.ReadLine()) != null)
            {
                index++;
                if (index < 3) continue;
                if (line == "") continue;
                // TODO

                string[] strData    = line.Split('\t');
                rewardData rd       = new rewardData();
                rd.rank             = getInt(strData[0]);
                rd.Diamonds         = getInt(strData[1]);
                rd.Score            = getInt(strData[2]);
                _lst.Add(rd);
            }
            _lst.Sort(new comp());
            rewardData rrrddd = get(1);
            rrrddd = get(4);
            rrrddd = get(2000);
            rrrddd = get(3000);
            rrrddd = get(10000);
            rrrddd = get(100000);
            stream.Close();
        }
        protected override string getFullConfigName(string version)
        {
            return _name + version + ".txt";
        }
    }
}
