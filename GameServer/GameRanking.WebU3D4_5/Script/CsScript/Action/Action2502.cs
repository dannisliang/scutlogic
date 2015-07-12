/****************************************************************************
Copyright (c) 2013-2015 scutgame.com

http://www.scutgame.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using GameRanking.Pack;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using System.Diagnostics;
using System.Collections.Generic;
using ZyGames.Framework.Common.Log;
using Game.Script;
using System.IO;
using System.Text;
using System.Web;
using Game.NSNS;
using System.Collections;
using Newtonsoft.Json;

namespace GameServer.CsScript.Action
{

    public class DeliverReceiveReturn
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppID { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public int ResultCode { get; set; }
        /// <summary>
        /// 返回值描述
        /// </summary>
        public string ResultMsg { get; set; }
        /// <summary>
        /// 签名值
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Content { get; set; }

    }

    public class OrderQueryReturn
    {
        /// <summary>
        /// Uid
        /// </summary>
        public long Uid { get; set; }
        /// <summary>
        /// OrderSerial
        /// </summary>
        public string OrderSerial { get; set; }
        /// <summary>
        /// MerchandiseName
        /// </summary>
        public string MerchandiseName { get; set; }
        /// <summary>
        /// OrderMoney
        /// </summary>
        public decimal OrderMoney { get; set; }
        /// <summary>
        /// CooperatorOrderSerial
        /// </summary>
        public string CooperatorOrderSerial { get; set; }
        /// <summary>
        /// OrderStatus
        /// </summary>
        public int OrderStatus { get; set; }
        /// <summary>
        /// StatusMsg
        /// </summary>
        public string StatusMsg { get; set; }
        /// <summary>
        /// StartDateTime
        /// </summary>
        public string StartDateTime { get; set; }
        /// <summary>
        /// BankDateTime
        /// </summary>
        public string BankDateTime { get; set; }
        /// <summary>
        /// ExtInfo
        /// </summary>
        public string ExtInfo { get; set; }
        /// <summary>
        /// VoucherMoney
        /// </summary>
        public int VoucherMoney { get; set; }
    }

    // 
    // 客户端支付成功后,收到第三方支付回掉
    // 处理回掉
    public class Action2502 : BaseAction
    {
        private string urlParams;
        private int appID = 6368606;
        private string secretkey = "NuWnKNHmDndw3HxuGT8ovxfgSp8dbGDq";
        private int resultCode = 1;
        private string resultMsg = "成功";
        private string returnJson = "";

        string app_uid = "";
        string server_orderid = "";
        string order_id = "";
        enum ErrorCodeEx
        {
            OK = 0,
            timeout_serviceError=1,
            order_failed=2,
            order_success=3,
            itemInfo_parseFaield=4,
            itemInfo_error=5,
            check_returncoderError=6,
            check_singError=7,
        }

        public Action2502(ActionGetter actionGetter)
            : base(2502, actionGetter)
        {
            urlParams = "";
        }

        public override bool GetUrlElement()
        {
            urlParams = (string)actionGetter.GetMessage();
            return true;
        }



        public class jsonReturn
        {
            public jsonReturn() { }
            public string ret { get; set; }
        }

        int check(string cooperatorOrderSerial)
        {
            //cooperatorOrderSerial = "40DDF8AC-6BE3-4F72-A10B-A057ADE3093C";
            //var cooperatorOrderSerial = "";//查询的CP订单号 eg:40DDF8AC-6BE3-4F72-A10B-A057ADE3093C
            var orderType = 1;//固定值
            var sign = Utility.Encrypt_MD5_UTF8(appID + cooperatorOrderSerial + secretkey);//签名
            var action = 10002;//固定值
            const string url = "http://querysdkapi.91.com/CpOrderQuery.ashx";//接口地址
            
            //请求接口获取数据
            var postdata = string.Empty;
            List<string> pairs = new List<string>();
            pairs.Add(string.Format("{0}={1}", "AppID", appID));
            pairs.Add(string.Format("{0}={1}", "CooperatorOrderSerial", cooperatorOrderSerial));
            pairs.Add(string.Format("{0}={1}", "OrderType", orderType));
            pairs.Add(string.Format("{0}={1}", "Sign", sign));
            pairs.Add(string.Format("{0}={1}", "Action", action));
            postdata = string.Join("&", pairs.ToArray());
            //请求接口
            var returnValue = Utility.ReceiveResponse(url, postdata, System.Text.Encoding.UTF8);
            //解析json数据
            var result = JsonConvert.DeserializeAnonymousType(returnValue, new
            {
                AppID = 0,
                ResultCode = 0,
                ResultMsg = string.Empty,
                Sign = string.Empty,
                Content = string.Empty
            });
            if (result == null)//请求超时或服务器错误的情况下
            {
                //Response.Write("接口请求出错！");
                ConsoleLog.showErrorInfo(0,"check 2502 result=1");
                return (int)ErrorCodeEx.timeout_serviceError;
            }

            ConsoleLog.showErrorInfo(0, "result:" + returnValue);
            //获取返回数据
            if (result.ResultCode != 1)
            {
                return (int)ErrorCodeEx.check_returncoderError;
            }
            
            string calcSign = Utility.Encrypt_MD5_UTF8(appID + result.ResultCode.ToString() + HttpUtility.UrlDecode(result.Content) + secretkey);
            if( result.Sign != calcSign)
            {
                ConsoleLog.showErrorInfo(0, "sign:" + sign + "\ncalcSing:"+calcSign);
                return (int)ErrorCodeEx.check_singError;
            }
            //成功
            //获取数据，业务处理
            var content = HttpUtility.UrlDecode(result.Content);//Content参数需要UrlDecode
            // 【BASE64解码--》JSON解析】
            string itemInfo = Utility.Base64StringDecode(content);
            var item = JsonConvert.DeserializeObject<OrderQueryReturn>(itemInfo);
            if (item != null && !string.IsNullOrEmpty(item.OrderSerial))
            {
                //业务处理
                if(item.OrderStatus == 0) //failed
                {
                    return (int)ErrorCodeEx.order_failed;
                }
                else
                {
                    return (int)ErrorCodeEx.OK;
                }
            }
            else
            {
                return (int)ErrorCodeEx.itemInfo_parseFaield;
            }
        }
        public override bool TakeAction()
        {
            ConsoleLog.showNotifyInfo("baiDu callBack . Action2502 call" + urlParams);
            //1.获取请求参数 提供两种获取参数方式

            #region 1.Request方式获取请求参数
            //var orderSerial = string.IsNullOrEmpty(Request["OrderSerial"]) ? string.Empty : Request["OrderSerial"];
            //var cooperatorOrderSerial = string.IsNullOrEmpty(Request["CooperatorOrderSerial"]) ? string.Empty : Request["CooperatorOrderSerial"];
            //var content = string.IsNullOrEmpty(Request["Content"]) ? string.Empty : Request["Content"];//Content通过Request读取的数据已经自动解码
            //var sign = string.IsNullOrEmpty(Request["Sign"]) ? string.Empty : Request["Sign"];
            #endregion

            #region 2.读取POST流方式获取请求参数
            ConsoleLog.showNotifyInfo("action2502 urlParms"+urlParams);
            var postData = urlParams; // parse at customActionDispatcher.
           // using (var br = new System.IO.BinaryReader(this.Request.InputStream))
            //{
           //     postData = System.Text.Encoding.UTF8.GetString(br.ReadBytes(int.Parse(this.Request.InputStream.Length.ToString())));
            //}

            //解析postData
            Hashtable ht = Utility.ConvertKeyValueStrToHashtable(postData);
            var orderSerial = string.Empty;
            if (ht != null && ht.ContainsKey("OrderSerial"))
                orderSerial = ht["OrderSerial"].ToString();
            var cooperatorOrderSerial = string.Empty;
            if (ht != null && ht.ContainsKey("CooperatorOrderSerial"))
                cooperatorOrderSerial = ht["CooperatorOrderSerial"].ToString();
            var content = string.Empty;
            if (ht != null && ht.ContainsKey("Content"))
                content = HttpUtility.UrlDecode(ht["Content"].ToString());//读取POST流的方式需要进行HttpUtility.UrlDecode解码操作
            var sign = string.Empty;
            if (ht != null && ht.ContainsKey("Sign"))
                sign = ht["Sign"].ToString();
            #endregion

            //2.先检测请求数据签名是否合法 
            string calcSign = Utility.Encrypt_MD5_UTF8(appID + orderSerial + cooperatorOrderSerial + content + secretkey);
            if (sign != calcSign)
            {
                resultCode = 1001;//自定义错误信息
                resultMsg  = "签名错误";//自定义错误信息
                returnJson = "";
                returnJson = JsonConvert.SerializeObject(new DeliverReceiveReturn()
                {
                    AppID = appID,
                    ResultCode = resultCode,
                    ResultMsg = resultMsg,
                    Sign = Utility.Encrypt_MD5_UTF8(appID + resultCode + secretkey),
                    Content = string.Empty
                }
                );
                string info = "SignError:sign:" + sign + 
                              "\ncalcSign:" + calcSign +
                              "\nappID:"+appID +
                              "\norderSerial"+orderSerial+
                              "\ncooperatorOrderSerial" + cooperatorOrderSerial+
                              "\ncontent" + content +
                              "\nsecretkey" + secretkey ;
                ConsoleLog.showNotifyInfo(info);
                return true;
            }

            //3.解析Content 内容  【BASE64解码--》JSON解析】
            var item = JsonConvert.DeserializeAnonymousType(Utility.Base64StringDecode(content), new
            {
                UID = 0L,
                MerchandiseName = string.Empty,
                OrderMoney = 0F,
                StartDateTime = System.DateTime.Now,
                BankDateTime = System.DateTime.Now,
                OrderStatus = 0,
                StatusMsg = string.Empty,
                ExtInfo = string.Empty,
                VoucherMoney = 0 //增加代金券金额获取
            });

            //5.返回成功结果
            resultCode = 1;
            resultMsg = "成功";
            returnJson = JsonConvert.SerializeObject(new DeliverReceiveReturn()
            {
                AppID = appID,
                ResultCode = resultCode,
                ResultMsg = resultMsg,
                Sign = Utility.Encrypt_MD5_UTF8(appID + resultCode + secretkey),//签名
                Content = string.Empty
            }
            );

            ConsoleLog.showNotifyInfo("action 2502 ok");
            // 5 验证。
            int checkResult = check(cooperatorOrderSerial);
            if(checkResult != 0)
            {
                ConsoleLog.showErrorInfo(0,checkResult+" action 2502 check error");
                return true;
            }

            // 6.执行业务逻辑处理,发放道具
            do
            {
                if (item != null)
                {
                    //获取数据
                    var uid = item.UID;
                    app_uid = uid.ToString();
                    server_orderid = cooperatorOrderSerial;
                    order_id = orderSerial;
                    string info = "item info:"+server_orderid + "#uid:"+uid+"#order_id"+order_id;
                    ConsoleLog.showErrorInfo(0,info);
                    bool hasGet = ProcessHMD(server_orderid, true, "ok");
                    processCache(server_orderid, true, "ok", hasGet);
                    //var merchandiseName = item.MerchandiseName;
                    //var orderMoney = item.OrderMoney;
                    //var startDateTime = item.StartDateTime;
                    //var bankDateTime = item.BankDateTime;
                    //var orderStatus = item.OrderStatus;
                    //var statusMsg = item.StatusMsg;
                    //var extInfo = item.ExtInfo;
                    //var voucherMoney = item.VoucherMoney;
                }
                //业务逻辑代码

            } while (false);


       
            return true;
        }


        bool addProductOnServer(PayOrderPersion payData, HappyModeData hmd)
        {
            string hd = GameConfigMgr.Instance().getProductInfo(payData.ProductId, payData.ServerOrderId);
            ConsoleLog.showNotifyInfo(hd);
            string[] itemInfos = hd.Split(',');
            bool add = false;
            for(int i=0; i<itemInfos.Length; ++i)
            {
                string[] infos = itemInfos[i].Split('*');
                int itemID = int.Parse(infos[0]);
                int itemNum = int.Parse(infos[1]);
                string infoLog = string.Format("add item:{0}num:{1}", itemID, payData.num * itemNum);
                ConsoleLog.showNotifyInfo(infoLog);
                TraceLog.WriteInfo(infoLog);
                hmd.ModifyLocked(() =>
                {
                    if (payData.ProductId == "5019")
                    {
                        hmd.HappyPoint += payData.num * itemNum;
                    }
                    if (payData.ProductId == "5020")
                    {
                        hmd.HappyReliveNum += payData.num * itemNum;
                    }
                    if (payData.ProductId == "5021")
                    {
                        hmd.HappyReliveNum += payData.num * itemNum;
                    }
                });
                if (payData.ProductId == "5019" ||
                    payData.ProductId == "5020" ||
                    payData.ProductId == "5021")
                {
                    add = true;
                }
            }

            return add;
        }

        bool ProcessHMD(string orderId, bool bSuccess, string jrRet)
        {
            // hmd 
            var hmdCache = new PersonalCacheStruct<HappyModeData>();
            int keyid = Action1005.getHappyIndex("YYS_BaiDu", app_uid);
            HappyModeData hmd = hmdCache.FindKey(keyid.ToString());
            if (null == hmd)
            {
                // errorcode
                ConsoleLog.showErrorInfo(0, "PayLog ProcessHMD hmd is null");
                TraceLog.WriteError("PayLog ProcessHMD hmd is null" );
                return false;
            }
            else
            {
                if (false == hmd.PayInfoDic.ContainsKey(server_orderid))
                {
                    // errorcode
                    ConsoleLog.showErrorInfo(0, "PayLog ProcessHMD hmd not find :" + orderId);
                    TraceLog.WriteError("PayLog ProcessHMD hmd not find :" + orderId);
                    return false;
                }
                else
                {
                    hmd.PayInfoDic[server_orderid].userParms = urlParams;
                    hmd.PayInfoDic[server_orderid].the3rdOrderId = order_id;
                }
            }

            PayOrderPersion payDataPersion = hmd.PayInfoDic[server_orderid];
            bool hasGetPayReward = false;
            if (false == payDataPersion.process)
            {
                if (bSuccess) hasGetPayReward = addProductOnServer(payDataPersion, hmd);
                hmd.PayInfoDic.ModifyLocked(() =>
                {
                    payDataPersion.state = bSuccess ? PayOrderPersion.PayStatus.paySuccess : PayOrderPersion.PayStatus.payFailed;
                    payDataPersion.process = true;
                    payDataPersion.typeUser = "YYS_BaiDu";
                    payDataPersion.jrRet = jrRet;
                    payDataPersion.hasGetPayReward = hasGetPayReward;
                });
            }
            return hasGetPayReward;
        }

        bool processCache(string orderId,bool bSuccess, string jrRet, bool hasGet)
        {
            // cache order
            var payCache = new ShareCacheStruct<PayOrder>();
            PayOrder payData = payCache.Find((o) =>
            {
                if (orderId == o.ServerOrderId)
                    return true;
                return false;
            });
            if (null == payData)
            {
                ConsoleLog.showErrorInfo(0, "PayLog not find order:" + orderId);
                TraceLog.WriteError("PayLog not find order:" + orderId);
                return false;
            }

            PayOrder.PayStatus payStatus = PayOrder.PayStatus.payFailed;
            if (bSuccess)
            {
                payStatus = PayOrder.PayStatus.paySuccess;
            }

            if (false == payData.process)
            {
                payData.ModifyLocked(() =>
                {
                    payData.userParms = urlParams;  // save ..
                    payData.the3rdOrderId = order_id;     // dic["order_id"];
                    payData.state = payStatus;
                    payData.process = true;
                    payData.typeUser = jrRet;
                    payData.hasGetPayReward = hasGet;
                });
            }

            //ConsoleLog.showNotifyInfo("process HttpPost:" + jrRet);
            //TraceLog.ReleaseWrite("process HttpPost:" + jrRet);

            return true;
        }
        protected override byte[] BuildResponsePack()
        {
            // send success or failed info 
            // for send to baidu verify
            return System.Text.Encoding.UTF8.GetBytes(returnJson);
        }


    }

}