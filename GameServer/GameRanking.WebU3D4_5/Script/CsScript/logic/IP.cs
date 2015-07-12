using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Game.Script
{
    class IP
    {

        public static bool EnableFileWatch = false;

        private static int offset;
        private static uint[] index = new uint[256];
        private static byte[] dataBuffer;
        private static byte[] indexBuffer;
        private static long lastModifyTime = 0L;
        private static string ipFile;
        private static readonly object @lock = new object();
        private static Dictionary<long, string> ipLong2info = new Dictionary<long, string>();
        private static Dictionary<string, string> ip2Info = new Dictionary<string, string>();
        private static List<long> ipLongLst = new List<long>();
        private static List<string> ipInfoLst = new List<string>();

        public static void check()
        {
            var start = (int)index[0];
            var max_comp_len = offset - 1028;
            long index_offset = -1;
            var index_length = -1;
            byte b = 0;
            for (start = start * 8 + 1024; start < max_comp_len; start += 8)
            {
                index_offset = BytesToLong(b, indexBuffer[start + 6], indexBuffer[start + 5],
                    indexBuffer[start + 4]);
                index_length = 0xFF & indexBuffer[start + 7];
                var areaBytes = new byte[index_length];
                Array.Copy(dataBuffer, offset + (int)index_offset - 1024, areaBytes, 0, index_length);
                string[] infos = Encoding.UTF8.GetString(areaBytes).Split('\t');
                string ip = indexBuffer[start + 0] + "." +
                             indexBuffer[start + 1] + "." +
                             indexBuffer[start + 2] + "." +
                             indexBuffer[start + 3];
                long ip2long_value = BytesToLong(indexBuffer[start + 0], indexBuffer[start + 1], indexBuffer[start + 2],
                       indexBuffer[start + 3]);

                string info = "";
                if (infos.Length == 1) info = infos[0] + "," + ",";
                else
                    if (infos.Length == 2) info = infos[0] + "," + infos[1] + ",";
                    else
                        if (infos.Length > 2) info = infos[0] + "," + infos[1] + "," + infos[2];


                ipLong2info.Add(ip2long_value, info); // is sorted
                ip2Info.Add(ip, info);

            }
            Console.WriteLine("Check END");
            Dictionary<long, string> sortedDic = ipLong2info.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
            foreach (var v in sortedDic.Keys)
            {
                ipLongLst.Add(v);
                ipInfoLst.Add(sortedDic[v]);
            }

        }

        public static string FindQuit(string ip)
        {
            var ips = ip.Split('.');
            var ip_prefix_value = int.Parse(ips[0]);
            long ip2long_value = BytesToLong(byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]),
                byte.Parse(ips[3]));

            int pos = ipLongLst.BinarySearch(ip2long_value);
            if (pos < 0) pos = ~pos;
           // Console.WriteLine(ipInfoLst[pos]);
            return ipInfoLst[pos];
        }
        public static void Start()
        {
            IP.EnableFileWatch = true;
            IP.Load("Script\\CsScript\\Config\\17monipdb.dat");
            Console.WriteLine(string.Join("\n", IP.Find("8.8.8.8")));
            Console.WriteLine(string.Join("\n", IP.Find("255.255.255.255")));
            check();
        }

        public static void Load(string filename)
        {
            ipFile = new FileInfo(filename).FullName;
            Load();
            if (EnableFileWatch)
            {
                Watch();
            }
        }

        public static string Find(string ip)
        {
            //lock (@lock)
            {
                var ips = ip.Split('.');
                var ip_prefix_value = int.Parse(ips[0]);
                long ip2long_value = BytesToLong(byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]),
                    byte.Parse(ips[3]));
                var start = index[ip_prefix_value];
                var max_comp_len = offset - 1028;
                long index_offset = -1;
                var index_length = -1;
                byte b = 0;
                for (start = start * 8 + 1024; start < max_comp_len; start += 8)
                {
                    if (
                        BytesToLong(indexBuffer[start + 0], indexBuffer[start + 1], indexBuffer[start + 2],
                            indexBuffer[start + 3]) >= ip2long_value)
                    {
                        index_offset = BytesToLong(b, indexBuffer[start + 6], indexBuffer[start + 5],
                            indexBuffer[start + 4]);
                        index_length = 0xFF & indexBuffer[start + 7];
                        break;
                    }
                }
                var areaBytes = new byte[index_length];
                Array.Copy(dataBuffer, offset + (int)index_offset - 1024, areaBytes, 0, index_length);
                string[] infos = Encoding.UTF8.GetString(areaBytes).Split('\t');

                string info = "";
                if (infos.Length == 1) info = infos[0] + "," + ",";
                else
                if (infos.Length == 2) info = infos[0] + "," + infos[1] + ",";
                else 
                    if(infos.Length > 2) info = infos[0] + "," + infos[1] + "," + infos[2];
                return info;
            }
        }

        private static void Watch()
        {
            var file = new FileInfo(ipFile);
            if (file.DirectoryName == null) return;
            var watcher = new FileSystemWatcher(file.DirectoryName, file.Name) { NotifyFilter = NotifyFilters.LastWrite };
            watcher.Changed += (s, e) =>
            {
                var time = File.GetLastWriteTime(ipFile).Ticks;
                if (time > lastModifyTime)
                {
                    Load();
                }
            };
            watcher.EnableRaisingEvents = true;
        }

        private static void Load()
        {
            lock (@lock)
            {
                var file = new FileInfo(ipFile);
                lastModifyTime = file.LastWriteTime.Ticks;
                try
                {
                    dataBuffer = new byte[file.Length];
                    using (var fin = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        fin.Read(dataBuffer, 0, dataBuffer.Length);
                    }

                    var indexLength = BytesToLong(dataBuffer[0], dataBuffer[1], dataBuffer[2], dataBuffer[3]);
                    indexBuffer = new byte[indexLength];
                    Array.Copy(dataBuffer, 4, indexBuffer, 0, indexLength);
                    offset = (int)indexLength;

                    for (var loop = 0; loop < 256; loop++)
                    {
                        index[loop] = BytesToLong(indexBuffer[loop * 4 + 3], indexBuffer[loop * 4 + 2],
                            indexBuffer[loop * 4 + 1],
                            indexBuffer[loop * 4]);
                    }
                }
                catch { }
            }
        }

        private static uint BytesToLong(byte a, byte b, byte c, byte d)
        {
            return ((uint)a << 24) | ((uint)b << 16) | ((uint)c << 8) | d;
        }
    }
}
