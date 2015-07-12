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
