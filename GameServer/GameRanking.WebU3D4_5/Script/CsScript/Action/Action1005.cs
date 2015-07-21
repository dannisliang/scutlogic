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
using Game.NSNS;
using System.Web;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GameServer.CsScript.Action
{
    // login
    // 上传token ，userid
    // 返回the3rdUserId
    // 
    public class Action1005 : BaseAction
    {
        private Request1005Pack requestPack;
        private Response1005Pack responsePack;
        static private int theUserId = -1;
        static private string version = "1.09";

        enum ErrorCodeEx
        {
            None = 0x0000000,
            Error_NotCare = 0x0000001,
            DataCheck_BlackList = 0x0000010,
            DataCheck_Error = 0x0000100,
        }

        public Action1005(ActionGetter actionGetter)
            : base(1005, actionGetter)
        {
            responsePack = new Response1005Pack();
        }

        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1005Pack>(data);
                return true;
            }
            return false;
        }

        public class LoginStateReturn
        {
            /// <summary>
            /// Uid
            /// </summary>
            public long UID { get; set; }
        }

        public class returnJson
        {
            public string id { get; set; }
            public string name { get; set; }
            public string avatar { get; set; }
            public string sex { get; set; }
            public string area { get; set; }
        }

        public static string getMapKey(string type, string id)
        {
            return (type + "_" + id);
        }

        // caret map . the3rdUserid and happyData....
        public static int getHappyIndex(string type, string id)
        {
            var happMapCache = new PersonalCacheStruct<The3rdUserIDMap>();
            var map = happMapCache.FindKey("888");
            int index = -1;
            string mapKey = getMapKey(type, id);
            if (false == map.the3rdMap.ContainsKey(mapKey))
            {
                var happyCache = new PersonalCacheStruct<HappyModeData>();
                var hmd = new HappyModeData();
                hmd.the3rdUserId = (int)happyCache.GetNextNo();
                int maxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum", 3);
                hmd.EnterNum = maxEnterNum;
                happyCache.Add(hmd);
                map.ModifyLocked(() =>
                {
                    map.the3rdMap.Add(mapKey, hmd.the3rdUserId);
                });

                index = hmd.the3rdUserId;
            }
            else
            {
                index = map.the3rdMap[mapKey];
            }
            return index;
        }

    //   public static int getHappyIndexBug(string type, string id)
    //   {
    //       uint sdkID = uint.Parse(id);
    //       int key = utils.KeyUInt2Int(sdkID);
    //       var happyCache = new PersonalCacheStruct<HappyModeData>();
    //       var hmd = happyCache.FindKey(key.ToString());
    //       if(hmd == null)
    //       {
    //           hmd = new HappyModeData();
    //           hmd.the3rdUserId = key;
    //           int maxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum", 3);
    //           hmd.EnterNum = maxEnterNum;
    //           happyCache.Add(hmd);
    //       }
    //       return key;
    //   }

        public static string getReturnInfo(string id, string name, string url)
        {
            if (version == "1.08")
            {
                return (id + "," + name + "," + url);
            }
            else
            {
                return (id + "," + name + "," + url + "," + theUserId);
            }
        }

        PayUserInfoEx getPUI(string theExInfo, int the3rdUsrID)
        {
            var pui = new PayUserInfoEx();
            pui.UserId = theUserId;
            pui.typeUser = requestPack.typeUser;
            pui.the3rdUsrID = utils.KeyInt2Uint(the3rdUsrID);
            pui.InfoExt = theExInfo;
            return pui;
        }
        void Pay360()
        {
            const string urlLogin = "https://openapi.360.cn/user/me.json?";
            var cache = new PersonalCacheStruct<PayUserInfoEx>();
            PayUserInfoEx pui = cache.FindKey(theUserId.ToString());
            string json = "";
            returnJson jr = null;
            int happyIndex = -1;
            if (pui == null)
            {
                // 回调后。
                string sendURL = urlLogin +
                                 HttpUtility.UrlEncode("access_token", Encoding.UTF8) + "=" +
                                 HttpUtility.UrlEncode(requestPack.token, Encoding.UTF8) + "&" +
                                 HttpUtility.UrlEncode("fields=id,name,avatar,sex,area", Encoding.UTF8);
                try
                {
                    json = utils.HttpPost(sendURL, "");
                    jr = JsonHelper.ParseFromJson<returnJson>(json);
                    happyIndex = getHappyIndex(requestPack.typeUser, jr.id);
                    pui = getPUI(json, happyIndex);
                    cache.Add(pui);
                }
                catch (System.Exception e)
                {
                    json = "{}";
                    jr = JsonHelper.ParseFromJson<returnJson>(json);
                    ConsoleLog.showErrorInfo(0, e.Message);
                }

            }
            else
            {
                json = pui.InfoExt;
                jr = JsonHelper.ParseFromJson<returnJson>(json);
                happyIndex = getHappyIndex(requestPack.typeUser, jr.id);
            }

            if (json == "{}")
            {
                responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.token_error;
            }
            else
            {
                responsePack.typeUser = requestPack.typeUser;
                responsePack.result = getReturnInfo(jr);
                responsePack.the3rdUserId = (uint)happyIndex;
                responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.ok;
            }
        }

        void PayBaiDu()
        {
            int appID = 6368606;
            var secretkey = "NuWnKNHmDndw3HxuGT8ovxfgSp8dbGDq";
            var url = "http://querysdkapi.91.com/CpLoginStateQuery.ashx";

            var accessToken = requestPack.token;//客户端SDK返回的登陆令牌
            var sign = Utility.Encrypt_MD5_UTF8(appID + accessToken + secretkey);//签名

            var cache = new PersonalCacheStruct<PayUserInfoEx>();
            PayUserInfoEx pui = cache.FindKey(theUserId.ToString());
            if(null == pui)
            {
                var postdata = string.Empty;
                List<string> pairs = new List<string>();
                pairs.Add(string.Format("{0}={1}", "AppID", appID));
                pairs.Add(string.Format("{0}={1}", "AccessToken", accessToken));
                pairs.Add(string.Format("{0}={1}", "Sign", sign));
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
                    responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.bd_result_error;
                    return;
                }
                //获取返回数据
                if (result.ResultCode == 1 && result.Sign == Utility.Encrypt_MD5_UTF8(appID + result.ResultCode.ToString() + HttpUtility.UrlDecode(result.Content) + secretkey))//成功
                {
                    //业务处理

                    //获取数据，业务处理
                    var content = HttpUtility.UrlDecode(result.Content);//Content参数需要UrlDecode
                    // 【BASE64解码--》JSON解析】
                    var item = JsonConvert.DeserializeObject<LoginStateReturn>(Utility.Base64StringDecode(content));
                    if (item != null && item.UID > 0)
                    {
                        int index = getHappyIndex(requestPack.typeUser, item.UID.ToString());
                        //业务处理
                        responsePack.result = getReturnInfo(item.UID.ToString(), "", "");
                        responsePack.the3rdUserId = utils.KeyInt2Uint(index);
                        responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.ok;
                        pui = getPUI(responsePack.result, index);
                        cache.Add(pui);
                    }
                    else
                    {
                        responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.bd_item_error;
                    }
                    //Response.Write("接口处理成功！");
                    return;
                }
                else//失败
                {
                    //Response.Write("接口请求出错！");
                    responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.bd_token_error;
                    return;
                }
            }
            else
            {
                responsePack.result = pui.InfoExt;
                responsePack.the3rdUserId = pui.the3rdUsrID;
                responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.ok;
            }

           
        }
        public override bool TakeAction()
        {
             if(requestPack.UserID<=0)
             {
                 var persionCache = new PersonalCacheStruct<GameUser>();
                  GameUser gu = new GameUser();
                  gu.UserId = (int)persionCache.GetNextNo();
                  gu.CreateTime = System.DateTime.Now;
                  gu.CompensationDate = gu.CreateTime;
                  gu.NickName = "";
                  gu.Identify = requestPack.identify;
                  gu.version = "1.09";
                  persionCache.Add(gu);
                  //
                  theUserId = gu.UserId;
             }
             else
            {
                theUserId = requestPack.UserID;
            }
          version = requestPack.version;
          ConsoleLog.showErrorInfo(0, "acton1005:userid:"+requestPack.UserID+"#version:"+requestPack.version);

          if (requestPack.typeUser == "YYS_CP360")
            {
                Pay360();
            }
             else if (requestPack.typeUser == "YYS_BaiDu")
             {
                 PayBaiDu();
             }
             else
             {
                 responsePack.errorCode = (byte)Response1005Pack.EnumErrorCode.not_find_typeUser;
             }
            return true;
        }


        public static string getReturnInfo(returnJson jr)
        {
            string url = GameConfigMgr.Instance().getString("360UrlCb", "http://www.youyisigame.com:8036/Service.aspx/Pay360");
            string info = getReturnInfo(jr.id,jr.name,url);
            return info;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}