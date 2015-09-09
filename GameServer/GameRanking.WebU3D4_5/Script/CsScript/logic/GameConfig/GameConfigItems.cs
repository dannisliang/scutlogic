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
    class GameConfigProducts : GameConfigBase
    {
        Dictionary<string, string> _dicKeyValue;
        public override void init()
        {
            base.init();
            _dicKeyValue = new Dictionary<string, string>();
        }

        public GameConfigProducts() { }

        public GameConfigProducts(string path)
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

        public string getProductItems(string productID,string ServerOrderID)
        {
            try
            {
                if(_rwLock.TryEnterReadLock(10000))
                {
                    try
                    {
                        return _dicKeyValue[productID];
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.showErrorInfo(0, "getProductItems:" + e.Message);
                        return "error";
                    }
                    finally
                    {
                        if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                    }
                }
                else
                {
                    return "error";
                }
            }
            catch(Exception e)
            {
                ConsoleLog.showErrorInfo(0, "getProductItems:" + e.Message);
                return "error";
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
                string productID = worlds[0];
                string items = worlds[1];
                _dicKeyValue.Add(productID,items);

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
