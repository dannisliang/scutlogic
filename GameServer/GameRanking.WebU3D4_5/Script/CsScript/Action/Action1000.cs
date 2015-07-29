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

namespace GameServer.CsScript.Action
{
    public class Action1000 : BaseAction
    {
        private Request1000Pack requestPack;
        private ResponsePack responsePack;
        private Stopwatch _watch;
        private List<string> _versionsNotSupport;

        enum ErrorCodeEx
        {
            None                = 0x0000000,
            Error_NotCare       = 0x0000001,
            DataCheck_BlackList = 0x0000010,
            DataCheck_Error     = 0x0000100,
        }
        public Action1000(ActionGetter actionGetter)
            : base(1000, actionGetter)
        {
            responsePack = new ResponsePack();
            _watch = new Stopwatch();
            _versionsNotSupport = new List<string>();
        }



        bool responsePackBuild(int errorCode,string errorInfo)
        {
            responsePack.ErrorCode = errorCode;
            responsePack.ErrorInfo = errorInfo;
            responsePack.ActionId = actionId;
            return true;
        }

        bool GameUserProcess(GameUser gu, bool checkDataOk)
        {
            var persionCache = new PersonalCacheStruct<GameUser>();
            if (requestPack.UserID <= 0)
            {
                 gu.UserId = (int)persionCache.GetNextNo();
                 persionCache.Add(gu);
            }
            else
            {
                if (checkDataOk)
                {
                    GameUser findGU = persionCache.FindKey(requestPack.UserID.ToString());
                    if (null == findGU) // maybe create by happyPoint
                    {
                        persionCache.Add(gu);
                    }
                    else if (requestPack.Score > findGU.Score)
                    {
                        findGU.ModifyLocked(() =>
                        {
                            findGU.NickName = gu.NickName;
                            findGU.Score    = gu.Score;
                            findGU.state    = gu.state;
                            findGU.version  = gu.version;
                        });
                    }
                }
            }
            return true;
        }
        void UserRankingProcess(GameUser gu)
        {
            var cache         = new ShareCacheStruct<UserRanking>();
            var cacheRanking  = cache.FindKey(gu.UserId);
            if (cacheRanking != null)
            {
                if(gu.Score>cacheRanking.Score)
                {
                    cacheRanking.ModifyLocked(() =>
                    {
                        cacheRanking.Score = gu.Score;
                    });
                }
            }
            else
            {
                // add
                UserRanking ur = new UserRanking();
                ur.UserID = gu.UserId;
                ur.Score = gu.Score;
                ur.UserName = gu.NickName;
                cache.Add(ur);
            }
        }
        private bool ProcessActionNew()
        {
            if (string.IsNullOrEmpty(requestPack.UserName))
            {
                return responsePackBuild(1,"UserName is null");
            }

             string[] pams = requestPack.Identify.Split('#');
             string Identify = requestPack.Identify;
             string version  = requestPack.version;
             if(false == GameConfigMgr.Instance().isSupportVersion(version))
             {
                 return responsePackBuild(2,"not support version");
             }

            int checkDataError = checkData();
            checkDataError    |= checkBlack();
            
            GameUser gu  = new GameUser();
            gu.UserId    = requestPack.UserID;
            gu.Score     = requestPack.Score;
            gu.version   = version;
            gu.state     = checkDataError;
            gu.NickName  = requestPack.UserName;
            gu.Identify  = Identify;

            bool checkDataOk = 0 == checkDataError;
            // update GameUser
            if(false == GameUserProcess(gu, checkDataOk))
            {
                return responsePackBuild(3, "User Id not Fund");
            }
            if (false == checkDataOk)
            {
                return responsePackBuild(4, "check data error");
            }
            
            // update and or into userRanking
            UserRankingProcess(gu);

            responsePack.MsgId = requestPack.UserID <= 0 ? gu.UserId : requestPack.UserID; // send to client.
            responsePack.ErrorCode = 0;
            responsePack.ErrorInfo = "save success";
            responsePack.ActionId = actionId;
            return true;
        }
       
        int checkBlack()
        {
            //01 blacklist
            var blackCache = new ShareCacheStruct<BlackListData>();
            BlackListData blackD = blackCache.Find((o) =>
            {
                return (o.Identify == requestPack.Identify);
            });
            if (null != blackD)
                return (int)ErrorCodeEx.DataCheck_BlackList;

            return (int)ErrorCodeEx.None;
        }
        int checkData()
        {
            //02 dataCheck
            //float minSpeed = 16.0f;
            return (int)ErrorCodeEx.None; // not support 1.06 waiting.....

     //     float maxSpeed = 32.0f;
     //     float timeDeviation = 10.0f;
     //     //float  maxTime = requestPack.Score / minSpeed;
     //     float  minTime = requestPack.Score / maxSpeed;
     //     float t = requestPack.ext01;
     //     minTime -= minTime * timeDeviation / 100.0f;
     //     //maxTime += maxTime * timeDeviation / 100.0f;
     //     if (t < minTime)
     //         return (int)ErrorCodeEx.DataCheck_Error;
     //     
     //     return (int)ErrorCodeEx.None;
        }
        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1000Pack>(data);
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            bool b = ProcessActionNew();
            return b;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}