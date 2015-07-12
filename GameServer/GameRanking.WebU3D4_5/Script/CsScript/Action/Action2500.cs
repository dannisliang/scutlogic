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


namespace GameServer.CsScript.Action
{
    // 
    // 客户端支付成功后,收到第三方支付回掉
    // 处理回掉
    public class Action2500 : BaseAction
    {
        private string urlParams;
        private  string _appKey    = "0cbe7dfc08ec7013a4bc3fecd50b9d0e";
        private  string appSecret  = "69ff132c1dfdb7051856eccf51b34a84";
        private  string urlVerfily = "http://mgame.360.cn/pay/order_verify.json?";
        private  string verfilyOK  = "verified";
        private  string app_uid    = "";
        private  string order_id      = "";
        private string server_orderid = "";
        enum ErrorCodeEx
        {
            OK          = 1,
            ParamError  = 2,
            AppKeyError = 3,
            SignError   = 4,
        }

        public Action2500(ActionGetter actionGetter)
            : base(2500, actionGetter)
        {
            urlParams = "";
            _appKey = GameConfigMgr.Instance().getString("360AppKey", "");
            appSecret = GameConfigMgr.Instance().getString("360AppSecret", "");
            urlVerfily = GameConfigMgr.Instance().getString("360UrlVerfily", "");
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


        private bool _isValidRequest(Dictionary<string,string> dic) 
        {

		    string []arrFields = {"app_key", "product_id", "app_uid",
			"order_id", "sign_type", "gateway_flag",
			"sign", "sign_return","amount"
		    };

		string key;
		string value;
	    for(int i=0; i<arrFields.Length; ++i)
        {
            key = arrFields[i];
            if(false == dic.ContainsKey(key))
                return false;

            value = dic[key];
            if(""==value || null==value)
                return false;
        }
		
		if(dic["app_key"] != _appKey)
        {
			return false;
		}
		string sign = getSign(dic);
		string paramSign = dic["sign"];
        ConsoleLog.showNotifyInfo("calc   sign:" + sign);
        ConsoleLog.showNotifyInfo("360    sign:" + paramSign);
        TraceLog.WriteError("calc sign:" + sign);
        TraceLog.WriteError("360  sign:" + paramSign);
        if (sign != paramSign)
        {
           
            ConsoleLog.showNotifyInfo(string.Format("_isValidRequest Sign error::{0},{1},{2},{3},{4}", app_uid, order_id, server_orderid, sign, paramSign));
            return false;
        }
        return true;
	}

        Dictionary<string,string> getDic()
        {
            Dictionary<string, string> dicParm = new Dictionary<string, string>();
            // urlParam 2 dic...
            string stringSplit = urlParams.Substring(urlParams.IndexOf('?')+1);
            string[] orderParams = stringSplit.Split('&');
            for(int i=0; i<orderParams.Length; ++i)
            {
                string[] worlds = orderParams[i].Split('=');
                if(worlds.Length==2)
                {
                    dicParm.Add(worlds[0], worlds[1]);
                }
            }
            return dicParm;
        }

        string getSignForVerify(Dictionary<string,string> dicParm)
        {
            List<string> keys = new List<string>(dicParm.Keys);
            keys.Sort();
            string str = "";
            string v = "";
            foreach (string k in keys)
            {
                if (k == "sign")
                {
                    continue;
                }
                if (dicParm[k] == null)
                {
                    continue;
                }
                v = dicParm[k];
                if (v == "0" || v == "")
                {
                    continue;
                }
                str += v + "#";
            }
            string sign = ZyGames.Framework.RPC.IO.SignUtils.EncodeSign(str, appSecret);
            string info = "keySign:" + str + appSecret + "\nresult:" + sign;
            //ConsoleLog.showNotifyInfo(info);
            //TraceLog.WriteError(info);
            return sign;
        }
        string getSign(Dictionary<string, string> dicParm)
        {
            List<string> keys = new List<string>(dicParm.Keys);
            keys.Sort();
		    string str = "";
            string v = "";
            foreach (string k in keys)
            {
                if (k == "sign" || k == "sign_return")
                {
				    continue;
			    }
                if(dicParm[k] ==null){
                    continue;
                }
                v = dicParm[k];
			    if (v == "0" || v=="") 
                {
				    continue;
			    }
			    str += v + "#";
		    }
            string sign = ZyGames.Framework.RPC.IO.SignUtils.EncodeSign(str, appSecret);
           // ConsoleLog.showNotifyInfo("old:" + dicParm["sign"]);
           // ConsoleLog.showNotifyInfo("new:" + sign);
           // string info = "keySign:" + str + appSecret + "\nresult:" + sign;
            //ConsoleLog.showNotifyInfo(info);
            //TraceLog.WriteError(info);
            return sign;
        }

        public string geturlParam(Dictionary<string,string> dic)
        {
            bool isOk = _isValidRequest(dic);
            if(false == isOk)
            {
                return "";
            }
            dic["sign"] = getSignForVerify(dic);
            //dic["sign"] = sign;
            string str = "";
            string value = "";
            foreach(var key in dic.Keys)
            {
                value = dic[key];
                str += HttpUtility.UrlEncode(key, Encoding.UTF8) + "=" + HttpUtility.UrlEncode(value, Encoding.UTF8)+"&";
            }
            return str.Substring(0,str.Length - 1);
        }
        public string md5(string str)
        {
            StringBuilder sb = new StringBuilder();
             try
             {
                byte[] bytes = Encoding.UTF8.GetBytes(str);

			    for (int i = 0; i < bytes.Length; i++) {
				    if ((bytes[i] & 0xff) < 0x10) {
					    sb.Append("0");
				}
                sb.Append(System.Convert.ToString((bytes[i] & 0xff), 16));
			    }
            }
            catch (System.Exception e)
             {
                 ConsoleLog.showErrorInfo(0, "md5 :" + e.Message);
             }
            return  sb.ToString();
        }

        bool addProductOnServer(PayOrderPersion payData,HappyModeData hmd)
        {
            string hd = GameConfigMgr.Instance().getProductInfo(payData.ProductId, payData.ServerOrderId);
            string[] infos  = hd.Split('*');
            int itemID      = int.Parse(infos[0]);
            int itemNum     = int.Parse(infos[1]);
            string infoLog  = string.Format("add item:{0}num:{1}", itemID, payData.num * itemNum);
            ConsoleLog.showNotifyInfo(infoLog);
            TraceLog.WriteInfo(infoLog);
            hmd.ModifyLocked(() => {
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
                return true;
            }
          
            return false;
        }

        bool ProcessHMD(string orderId, Dictionary<string, string> dic, bool bSuccess, string jrRet)
        {
             // hmd 
            var hmdCache = new PersonalCacheStruct<HappyModeData>();
            int keyid = Action1005.getHappyIndex("YYS_CP360", app_uid);
            HappyModeData hmd = hmdCache.FindKey(keyid.ToString());
            if(null == hmd)
            {
                // errorcode
                ConsoleLog.showErrorInfo(0, "PayLog ProcessHMD hmd is null" + dic["app_uid"]+":"+dic["user_id"]);
                TraceLog.WriteError("PayLog ProcessHMD hmd is null" + dic["app_uid"] + ":" + dic["user_id"]);
                return false;
            }
            else
            {
                if(false == hmd.PayInfoDic.ContainsKey(server_orderid))
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
                    payDataPersion.typeUser = "360Pay";
                    payDataPersion.jrRet = jrRet;
                    payDataPersion.hasGetPayReward = hasGetPayReward;
                });
            }
            return hasGetPayReward;
        }

        bool processCache(string orderId, Dictionary<string,string> dic,bool bSuccess,string jrRet,bool hasGet)
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
                    payData.userParms = urlParams; // save ..
                    payData.the3rdOrderId = dic["order_id"];
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
        public override bool TakeAction()
        {
            ConsoleLog.showNotifyInfo("360 callBack . Action2500 call"+urlParams);
            // 回调后。
            // 验证订单是否ok
            Dictionary<string, string> dic = getDic();
            if (false == dic.ContainsKey("app_order_id") ||
                false == dic.ContainsKey("app_uid") ||
                false == dic.ContainsKey("order_id"))
            {
                ConsoleLog.showErrorInfo(0, "PayLog importent id NotFind:"+urlParams);
                TraceLog.WriteError("PayLog importent id NotFind:" + urlParams);
                return false;
            }
            server_orderid = dic["app_order_id"]; 
            order_id = dic["order_id"];
            app_uid = dic["app_uid"];
            
            //ConsoleLog.showErrorInfo(0,"order_id:" + orderId+" the3rdID:"+dic["app_uid"]);
         
            string p = geturlParam(dic);
            if ("" != p)
            {
                //ConsoleLog.showNotifyInfo("process order:" + urlParams);
                //ConsoleLog.showNotifyInfo("geturlParam  :" + p);
                //TraceLog.ReleaseWrite("geturlParam  :" + p);
                //TraceLog.ReleaseWrite("process order:" + urlParams);
                string result = utils.HttpPost(urlVerfily, p);
                jsonReturn jr = JsonHelper.ParseFromJson<jsonReturn>(result);
                bool bSuccess = jr.ret.Equals(verfilyOK, System.StringComparison.OrdinalIgnoreCase);
                bool hasGet = ProcessHMD(server_orderid, dic, bSuccess, jr.ret);
                processCache(server_orderid, dic, bSuccess, jr.ret, hasGet);
            }
         
            return true;
        }
        protected override byte[] BuildResponsePack()
        {
            return System.Text.Encoding.UTF8.GetBytes("");
        }
    }
  
}