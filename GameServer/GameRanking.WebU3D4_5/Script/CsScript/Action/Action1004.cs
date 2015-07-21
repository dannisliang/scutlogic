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
using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using GameRanking.Pack;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.RPC.IO;
using System.Linq;
using Game.Script;
using System.Diagnostics;

namespace GameServer.CsScript.Action
{
    public class Action1004 : BaseAction
    {
        private Request1004Pack requestPack;
        private Response1004Pack responsePack;

        void addLog(ShareCacheStruct<DataLog> cache, int userid, int dimond)
        {
            int index = (int)cache.GetNextNo();
            DataLog dl = new DataLog();
            dl.Index = index;
            dl.UserID = userid;
            dl.Dimond = dimond;
            cache.Add(dl);
        }

        enum theActionId
        {
            rank_dimond = 0,
            check_order = 1,
            check_compensation=2,
        }

        public Action1004(ActionGetter actionGetter)
            : base(1004, actionGetter)
        {
            responsePack = new Response1004Pack();
        }

        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1004Pack>(data);
                return true;
            }
            return false;
        }

         bool PermissionValidation()
        {
            int userID = requestPack.UserID;
            var personCache = new PersonalCacheStruct<GameUser>();
            var user = personCache.FindKey(userID.ToString());
            if (null == user)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.user_not_find;
                return false;
            }
            if (user.Identify != requestPack.identify)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.identify_not_match;
                return false;
            }

            return true;
        }
     
        void bugForVersion()
         {
             int userID = requestPack.UserID;
             var userCache = new PersonalCacheStruct<GameUser>();
             var gu = userCache.FindKey(userID.ToString());
             if (null != gu)
             {
                 gu.ModifyLocked(() =>
                 {
                     gu.version = requestPack.version;
                 });
             }
         }
        public override bool TakeAction()
        {
            bugForVersion();
            responsePack.status = requestPack.status+1;
            responsePack.actionID = requestPack.actionID;
            responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.ok;

            if (PermissionValidation())
            {
                if (0 == requestPack.status)
                {
                    doActionCheck();
                }
                else if (1 == requestPack.status)
                {
                    doActionEnd();
                }
                else
                {
                    responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.status_error;
                }
            }
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

        //       ugly

        void doActionCheck()
        {
            if (requestPack.actionID == (byte)theActionId.rank_dimond)
            {
                doCheck_rank_dimond();
            }

            if(requestPack.actionID == (byte)theActionId.check_order)
            {
                doCheck_order();
            }

            if(requestPack.actionID == (byte)theActionId.check_compensation)
            {
                doCheck_compensation();
            }

        }

        void doActionEnd()
        {
            if (requestPack.actionID == (byte)theActionId.rank_dimond)
            {
                doEnd_rank_dimond();
            }

            if(requestPack.actionID == (byte)theActionId.check_order)
            {
                doEnd_order();
            }

            if((byte)theActionId.check_compensation == requestPack.actionID)
            {
                doEnd_compensation();
            }
        }

        void doEnd_order()
        {
            string orderId = requestPack.infoExt;
            var payCache = new ShareCacheStruct<PayOrder>();
            PayOrder payData = payCache.Find((o) =>
            {
                if (orderId == o.ServerOrderId)
                    return true;
                return false;
            });

            if (payData == null)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.order_not_find;
            }

            payData.ModifyLocked(() => {
                payData.hasGetPayReward = true;
            });
            responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.ok;
        }

        void doCheck_compensation()
        {
            var persionCache = new PersonalCacheStruct<GameUser>();
            GameUser gu = persionCache.FindKey(requestPack.UserID.ToString());
            if(null == gu)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.compensation_not_findUser;
                return;
            }

            var compensationCache = new ShareCacheStruct<CompensationInfo>();
            List<CompensationInfo> Lstcsi = compensationCache.FindAll((o) => {
                return (gu.CompensationDate < o.CreateDate);
            },true);

            if(Lstcsi.Count <= 0)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.no_data01;
                return;
            }
            CompensationInfo csi = Lstcsi[Lstcsi.Count-1];
            if(null==csi)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.no_data02;
                return;
            }

            // has got the new Compensation
            gu.ModifyLocked(() => {
                gu.CompensationDate = csi.CreateDate;
            });

            responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.ok;
            string info = csi.type+","+csi.CreateDate.ToString()+","+csi.message+","+csi.ItemInfo;
            responsePack.extInfo   = info;
        }

        void doEnd_compensation()
        {
            // do nothing...
            responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.ok;
        }
        void doCheck_order()
        {
            string orderId = requestPack.infoExt;
            var payCache = new ShareCacheStruct<PayOrder>();
            PayOrder payData = payCache.Find((o) =>
            {
                if (orderId == o.ServerOrderId)
                    return true;
                return false;
            });

            if(payData == null)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.order_not_find;
                return;
            }

            if (payData.hasGetPayReward)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.hasGetPayReward;
                return;
            }

            string productId = payData.ProductId;
            responsePack.extInfo = GameConfigMgr.Instance().getProductInfo(productId,orderId); // add 道具发放信息。
            responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.ok;
        }
        void doCheck_rank_dimond()
        {
            int userID = requestPack.UserID;
            var personCache = new PersonalCacheStruct<GameUser>();
            var user = personCache.FindKey(userID.ToString());
            if (null == user) return;
           
            responsePack.UserID = requestPack.UserID;
            int dimond = user.Diamond;
            if (dimond > 0)
            {
                responsePack.Result.Add(dimond); // 钻石

                int afterGetDimondScore = user.Score;
                int  reduceScore = GameConfigMgr.Instance().getInt("rank_score_redice", 1);
                if (user.Score>reduceScore)
                {
                    afterGetDimondScore = GameConfigMgr.Instance().getInt("rank_clear_after", 500);
                }
                responsePack.Result.Add(afterGetDimondScore); // 消减后的名次
                responsePack.Result.Add(user.theTotal);     // 积分
                int rank = user.preRanking;
                int MaxRank = 99999;
                if (rank > MaxRank) rank = MaxRank;
                responsePack.Result.Add(rank);   // 上次排名
            }
            else
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.ok_but_not_dimond;
            }
        }
        void doEnd_rank_dimond()
        {
            int userID = requestPack.UserID;
            var personCache = new PersonalCacheStruct<GameUser>();
            var user = personCache.FindKey(userID.ToString());
            if (null == user)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.user_not_find;
                return;
            }
            if (user.Identify != requestPack.identify)
            {
                responsePack.errorCode = (byte)Response1004Pack.EnumErrorCode.identify_not_match;
                return;
            }
            responsePack.UserID = userID;
            if(user.Diamond>0)
            {
                addLog(new ShareCacheStruct<DataLog>(), user.UserId, user.Diamond);
                user.ModifyLocked(() =>
                {
                    user.Diamond  = 0;
                    user.theTotal = 0;
                });
            }
        }
    }
}
