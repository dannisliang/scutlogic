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
    class GameConfigValue : GameConfigBase
    {
        Dictionary<string, string> _dicKeyValue;
        public override void init()
        {
            base.init();
            _dicKeyValue = new Dictionary<string, string>();
        }

        public GameConfigValue() { }

        public GameConfigValue(string path)
        {
            _path = path;
            _name = path + "//";
        }

        public string getData(string key)
        {
            if(_dicKeyValue.ContainsKey(key))
            {
                return _dicKeyValue[key];
            }
            return "";
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

        string removeComment(string line)
        {
            int commentIndex = line.IndexOf('#');
            if(commentIndex>0)line = line.Remove(commentIndex);
            Regex r = new Regex("[\\w\\s]+[=][\\w\\s]+",RegexOptions.Compiled);
            if (!r.IsMatch(line))
            {
                return "";
            }
            string[] worlds = line.Split('=');
            Regex r2 = new Regex("^[0-9]+|[0-9]+[.]{1}[0-9]+$",RegexOptions.Compiled);
            worlds[0] = worlds[0].Replace(" ","").Replace("\t","");
            if(r2.IsMatch(worlds[1])) // 数字浮点
                worlds[1] = worlds[1].Replace(" ","").Replace("\t","");
            line = worlds[0] + "=" + worlds[1];
            return line;
        }

        protected override void gameConfigCreate(string fileName, string name)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            string  line = "";
            _dicKeyValue.Clear();
            while ((line = stream.ReadLine()) != null)
            {
                // TODO
                line = removeComment(line);
                if (string.IsNullOrEmpty(line)) continue;
               //ConsoleLog.showNotifyInfo(line);
                string[] worlds = line.Split('=');
                _dicKeyValue.Add(worlds[0], worlds[1]);
              
            }
            foreach (var key in _dicKeyValue.Keys)
            {
                ConsoleLog.showNotifyInfo(key + ":" + _dicKeyValue[key]);
            }
            stream.Close();
        }
        protected override string getFullConfigName(string version)
        {
            return _name  +  version + ".txt";
        }
    }
}
