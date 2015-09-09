using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Log;
using GameServer.Model;
using System.Text.RegularExpressions;
using Game.NSNS;

namespace Game.Script
{
    public class GameConfigHappyPoint : GameConfigBase
    {
        private class HappyData
        {
            public int itemID{get;set;}
            public string name { get; set; }
            public int needHappyPoint{get;set;}
            public int RefleshNum {get;set;}
            public int MinuteForReflesh { get; set; }
            public int timeRefleshCng { get; set; }
            public int canReplace { get; set; }

        }

        Dictionary<int, HappyData> _dicKeyValue;
        public override void init()
        {
            base.init();
            _dicKeyValue = new Dictionary<int, HappyData>();
        }

        public GameConfigHappyPoint() { }

        public GameConfigHappyPoint(string path)
        {
            _path = path;
            _name = path + "//";
        }

        public override void getAllConfigFiles(List<string> names)
        {
            DirectoryInfo folder = new DirectoryInfo(_path);

            foreach (FileInfo file in folder.GetFiles())
            {
                string name = file.Name;
                name = name.Remove(name.LastIndexOf('.'));
                if (names.Contains(name)) continue;
                names.Add(name);
            }
        }

        public List<int> getKeys()
        {
            try
            {
                if(_rwLock.TryEnterReadLock(10000))
                {
                    try
                    {
                        return new List<int>(_dicKeyValue.Keys);
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.showErrorInfo(0, "getKeys:" + e.Message);
                        return null;
                    }
                    finally
                    {
                        if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                ConsoleLog.showErrorInfo(0, "getKeys:" + e.Message);
                return null;
            }
        }

         HappyData getRealItemInfo(int id)
        {
            try
            {
                if(_rwLock.TryEnterReadLock(10000))
                {
                    try
                    {
                        if (_dicKeyValue.ContainsKey(id))
                            return _dicKeyValue[id];
                        else
                            return null;
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.showErrorInfo(0, "getRealItemInfo:" + e.Message);
                        return null;
                    }
                    finally
                    {
                        if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                    }
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception e)
            {
                ConsoleLog.showErrorInfo(0, "getRealItemInfo:" + e.Message);
                return null;
            }
        }
        protected override void gameConfigCreate(string fileName, string name)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            string line = "";
            _dicKeyValue.Clear();
            int count = 0;
            while ((line = stream.ReadLine()) != null)
            {
                count++;
                if (count < 3) continue;
                if (line == "") continue;
                // TODO
                string[] worlds = line.Split('\t');
                int realItemID = getInt(worlds[0]);
                string itemName = getStr(worlds[1]);
                int happyPoint = getInt(worlds[2]);
                int Minute = getInt(worlds[3]);
                int num = getInt(worlds[4]);
                int timeRefleshCng = getInt(worlds[5]);
                int canReplace = getInt(worlds[6]);
                

                HappyData hd = new HappyData();
                hd.itemID = realItemID;
                hd.needHappyPoint = happyPoint;
                hd.MinuteForReflesh = Minute;
                hd.RefleshNum = num;
                hd.name = itemName;
                hd.timeRefleshCng = timeRefleshCng;
                hd.canReplace = canReplace;

                _dicKeyValue.Add(realItemID, hd);
            }
            foreach (var key in _dicKeyValue.Keys)
            {
                ConsoleLog.showNotifyInfo(key + ":" + _dicKeyValue[key]);
            }
            stream.Close();
        }
        protected override string getFullConfigName(string version)
        {
            return _name + version + ".txt";
        }
    }
}
