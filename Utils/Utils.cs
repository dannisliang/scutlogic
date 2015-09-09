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
using System.Collections.Generic;
using System.Reflection;

namespace Game.Utils
{
    public class Utils
    {
        public static void SetData(object obj, Dictionary<string, object> PropertiesValDic)
        {
            Type t = obj.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                object value1 = pi.GetValue(obj, null);//用pi.GetValue获得值
                string name = pi.Name;//获得属性的名字,后面就可以根据名字判断来进行些自己想要的操作
                if (PropertiesValDic.ContainsKey(name))
                {
                    pi.SetValue(obj, PropertiesValDic[name]);
                }
            }
        }
        static public uint BytesToLong(byte a, byte b, byte c, byte d)
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
            if (json.Length > 128)
            {
                if (json.Contains("},") || json.Contains("],"))
                {
                    json = json.Replace("},", "\n");
                    json = json.Replace("],", "\n");
                }
                else
                {
                    json = json.Replace(",", "\n");
                }
                json = json.Replace("{", "");
                json = json.Replace("}", "");
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
}
