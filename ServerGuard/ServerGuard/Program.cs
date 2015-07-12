using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerGuard
{
    /*
* 短信http接口的c#代码调用示例
* User: jacky
* Date: 2013/12/10
* Time: 16:02
* 
*/
    using System.Net;
    using System.IO;
        class Guard
        {
            /**
            * 服务http地址
            */
            private static string BASE_URI = "http://yunpian.com";
            /**
            * 服务版本号
            */
            private static string VERSION = "v1";
            /**
            * 查账户信息的http地址
            */
            private static string URI_GET_USER_INFO = BASE_URI + "/" + VERSION + "/user/get.json";
            /**
            * 通用接口发短信的http地址
            */
            private static string URI_SEND_SMS = BASE_URI + "/" + VERSION + "/sms/send.json";
            /**
            * 模板接口短信接口的http地址
            */
            private static string URI_TPL_SEND_SMS = BASE_URI + "/" + VERSION + "/sms/tpl_send.json";

            /**
            * 取账户信息
            * @return json格式字符串
            */
            public static string getUserInfo(string apikey)
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI_GET_USER_INFO + "?apikey=" + apikey);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            /**
            * 通用接口发短信
            * @param text　短信内容　
            * @param mobile　接受的手机号
            * @return json格式字符串
            */
            public static string sendSms(string apikey, string text, string mobile)
            {
                //注意：参数必须进行Uri.EscapeDataString编码。以免&#%=等特殊符号无法正常提交
                string parameter = "apikey=" + apikey + "&text=" + Uri.EscapeDataString(text) + "&mobile=" + mobile;
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI_SEND_SMS);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameter);//这里编码设置为utf8
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }

            /**
            * 模板接口发短信
            * @param tpl_id 模板id
            * @param tpl_value 模板变量值
            * @param mobile　接受的手机号
            * @return json格式字符串
            */
            public static string tplSendSms(string apikey, long tpl_id, string tpl_value, string mobile)
            {
                string encodedTplValue = Uri.EscapeDataString(tpl_value);
                string parameter = "apikey=" + apikey + "&tpl_id=" + tpl_id + "&tpl_value=" + encodedTplValue + "&mobile=" + mobile;
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI_TPL_SEND_SMS);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameter);//这里编码设置为utf8
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }

            static void sendMsg(string sendStr,long tmpID)
            {
                //修改为您的apikey
                string apikey = "0cfe0e23f83bf7cc2b93777e1edaf925";
                //修改为您要发送的手机号
                string mobile = "15210842209";
                //调用模板接口发短信
                long tpl_id = tmpID; //使用模板1，对应的模板内容为：您的验证码是#code#【#company#】
                //注意：参数必须进行Uri.EscapeDataString编码。以免&#%=等特殊符号无法正常提交
                string tpl_value = "#time#=" + Uri.EscapeDataString(sendStr);
                System.Console.WriteLine(tplSendSms(apikey, tpl_id, tpl_value, mobile));

            }

            static string getSeerverURL()
            {
                // read form local config.
                return "www.baidu.com";
            }
            static public string HttpPost(string Url, string postDataStr)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                request.KeepAlive = false;
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

            static int timerCntForGuard = 1000;
            static int sendMsgCng       = 10;
            static int sendMsgNum       = 0;
            static int dotCnt           = 0;
            static int breakCnt         = 3;
            static void dotCntPrint()
            {
                while(true)
                {
                    Thread.Sleep(800);

                    if (sendMsgNum > breakCnt) break;

                    string running = "running";
                    for (int i = 0; i < dotCnt; ++i)
                    {
                        running += ".";
                    }
                    if (dotCnt == 0) running = "running      ";
                    Console.Write("\r" + System.DateTime.Now.ToString("HH:mm:ss") + ":" + running);
                    dotCnt++;
                    if (dotCnt > 6)
                    {
                        dotCnt = 0;
                    }
                }
              
            }
            static public void showLog(string log)
            {
                Console.WriteLine(System.DateTime.Now.ToString() +":"+ log);
            }

            static public void threadCb()
            {
                showLog("Begin Guard.");
                while(true)
                {
                    if (sendMsgNum > breakCnt) break;
                    try
                    {
                        string ret = HttpPost(getSeerverURL(), "hello Guard");
                        if(ret != "guard ok")
                        {
                            if(ret == "")
                            {
                                
                            }
                            else
                            {
                                Console.WriteLine(ret);
                                if (sendMsgCng-- < 0)
                                {
                                    sendMsgNum++;
                                    sendMsgCng = 10;
                                    showLog("send msg");
                                    //sendMsg("服务器错误消息:" + e.Message, 867975);
                                }
                            }

                        }
                        else
                        {
                            sendMsgNum = 0;
                            sendMsgCng = 10;
                            Console.WriteLine("Reset sendMsgNum");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if (sendMsgCng-- < 0)
                        {
                            sendMsgNum++;
                            sendMsgCng = 10;
                            showLog("send msg");
                            //sendMsg("服务器错误消息:" + e.Message, 867975);
                        }
                    }

                    Thread.Sleep(timerCntForGuard);
                }
                
            }

            public static void Main(string[] args)
            {
                Thread dot = new Thread(dotCntPrint);
                dot.Start();
                Thread sub = new Thread(threadCb);
                sub.Start();
            

                sub.Join();//最重要就是这句，阻塞主线程，直到子线程执行完毕
                dot.Join();
                Console.Write("end guard must have be send 3 msg.");
                Console.ReadKey(true);
            }
        }
    }

