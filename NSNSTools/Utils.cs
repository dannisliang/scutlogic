using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Collections;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Model;

namespace Game.NSNS
{
   public class ConsoleLog
    {
        public enum ConsoleLogType
        {
            Debug,
            Publish,
        }

        StringBuilder _sb = new StringBuilder(1024);
        public static void SetConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
        public static void showErrorInfo(int line, string info)
        {
            try
            {
                SetConsoleColor(ConsoleColor.Red);
                Console.WriteLine("Error@Line:" + line + ";need:" + info);
                SetConsoleColor(ConsoleColor.Gray);
            }
            catch
            { }
        }
        public static void showNotifyInfo(string info, ConsoleLogType t = ConsoleLogType.Publish)
        {
            try
            {
                if (ConsoleLogType.Debug == t)
                {
                    return;
                }
                SetConsoleColor(ConsoleColor.DarkGreen);
                Console.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + ":" + info);
                SetConsoleColor(ConsoleColor.Gray);
            }
            catch
            { }
        }

        public static void showPercent(string info)
        {
            try
            {
                SetConsoleColor(ConsoleColor.Green);
                Console.Write("\r" + System.DateTime.Now.ToString("HH:mm:ss") + ":" + info);
                SetConsoleColor(ConsoleColor.Gray);
            }
            catch
            { }
        }

    }
    public class utils
    {
        public static void doFrom_Model_person<T>(object parm, string key = "UserId") where T : BaseEntity, new()
        {
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<T>();
            string typeName = typeof(T).ToString();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From Start:" + max);
            int Step = 1000;
            var cache = new PersonalCacheStruct<T>();
            cache.ReLoad();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where " + key + " >=@Key1 and " + key + " <@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo(typeName + ":" + i + " load");
            }
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From End");
        }

        public static void doFrom_Model_share<T>(object parm, string key = "UserID") where T : ShareEntity, new()
        {
            string typeName = typeof(T).ToString();
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<T>();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From Start:" + max);
            int Step = 1000;
            var cache = new ShareCacheStruct<T>();
            cache.ReLoad();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where " + key + " >=@Key1 and " + key + " <@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo(typeName + ":" + i + " load");
            }
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From End");
        }
        static public  uint BytesToLong(byte a, byte b, byte c, byte d)
        {
            return ((uint)a << 24) | ((uint)b << 16) | ((uint)c << 8) | d;
        }

        static public uint getIpUnit(string s)
        {
            string[] ipbyte = s.Split('.');
            if (ipbyte.Length != 4)
            {

            }
            else
            {
                byte a, b, c, d;
                a = byte.Parse(ipbyte[0]);
                b = byte.Parse(ipbyte[1]);
                c = byte.Parse(ipbyte[2]);
                d = byte.Parse(ipbyte[3]);
                return BytesToLong(a, b, c, d);
            }
            return 0;
        }

        static public string getIp(uint serverHost)
        {
            Byte a, b, c, d;
            a = (byte)(serverHost & 0xff000000);
            b = (byte)(serverHost & 0x00ff0000);
            c = (byte)(serverHost & 0x0000ff00);
            d = (byte)(serverHost & 0x000000ff);
            string ip = a + "." + b + "." + c + "." + d;
            return ip;
        }
        static public uint KeyInt2Uint(int id)
        {
            if(id < 0)
            {
                return (uint)((uint)int.MaxValue - id);
            }
            else
            {
                return (uint)id;
            }
        }
        static public int KeyUInt2Int(uint id)
        {
            if(id > int.MaxValue)
            {
                return -((int)(id-int.MaxValue));
            }
            else
            {
                return (int)id;
            }
        }
        static public string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            using (Stream myRequestStream = request.GetRequestStream())
            {
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();
            }


            // response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
    }
    public class JsonHelper
    {
        /// <summary>
        /// 生成Json格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJson<T>(T obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, obj);
                string szJson = Encoding.UTF8.GetString(stream.ToArray());
                return szJson;
            }
        }
        public static string prettyJson<T>(T obj)
        {
            string json = GetJson<T>(obj);
            if(json.Length>128)
            {
                if(json.Contains("},")||json.Contains("],"))
                {
                    json = json.Replace("},", "\n");
                    json = json.Replace("],", "\n");
                }
                else
                {
                    json = json.Replace(",", "\n");
                }
                json=json.Replace("{", "");
                json=json.Replace("}", "");
                json = json.Replace("[", "");
                json = json.Replace("]", "");
            }
            return json;
        }
        /// <summary>
        /// 获取Json的Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="szJson"></param>
        /// <returns></returns>
        public static T ParseFromJson<T>(string szJson) where T : new()
        {
            T obj = new T();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(szJson)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }
    }

    public class Utility
    {
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static string Encrypt_MD5_UTF8(string appKey)
        {
            MD5 MD5 = new MD5CryptoServiceProvider();
            byte[] datSource = Encoding.GetEncoding("utf-8").GetBytes(appKey);
            byte[] newSource = MD5.ComputeHash(datSource);
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < newSource.Length; i++)
            {
                sb.Append(newSource[i].ToString("x").PadLeft(2, '0'));
            }
            string crypt = sb.ToString();
            return crypt;
        }
        /// <summary>
        /// BASE64解码
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static string Base64StringDecode(string sourceStr)
        {
            byte[] buf = Convert.FromBase64String(sourceStr);
            return Encoding.UTF8.GetString(buf);
        }
        /// <summary>
        /// 读取知道url的所有内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encode"></param>
        /// <param name="timeOutSeconds"></param>
        /// <returns></returns>
        public static string ReceiveResponse(string url, string data, Encoding encode, int timeOutSeconds = 10)
        {
            try
            {
                byte[] PostData = encode.GetBytes(data);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; Q312461; .NET CLR 1.0.3705)";
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = PostData.Length;
                req.Timeout = timeOutSeconds * 1000;

                Stream webStream = req.GetRequestStream();
                webStream.Write(PostData, 0, PostData.Length);
                webStream.Close();

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream(), encode);
                string temp = sr.ReadToEnd();
                sr.Close();
                res.Close();
                return temp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// KeyValue字符串组转Hashtable
        /// </summary>
        /// <param name="keyValueStr">格式如：key1=value1&key2=value2</param>
        /// <param name="connector">keyvalue连接符</param>
        /// <param name="spilt">keyvalue分隔符</param>
        /// <returns></returns>
        public static Hashtable ConvertKeyValueStrToHashtable(string keyValueStr, char connector = '&', char spilt = '=')
        {
            Hashtable ht = null;
            if (!string.IsNullOrEmpty(keyValueStr) && keyValueStr.IndexOf(connector.ToString()) > -1 && keyValueStr.IndexOf(connector.ToString()) > -1)
            {
                ht = new Hashtable();
                foreach (string keyValue in keyValueStr.Split(connector))
                {
                    var kv = keyValue.Split(spilt);
                    if (kv.Length > 1 && !ht.ContainsKey(kv[0]))
                    {
                        ht.Add(kv[0], kv[1]);
                    }
                }
            }
            return ht;
        }

    }
}
