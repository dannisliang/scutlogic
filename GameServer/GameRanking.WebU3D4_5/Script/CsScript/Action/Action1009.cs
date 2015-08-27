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
using System.Web;
using System.IO;
using System.Text;
using System.Data;
using Game.NSNS;

namespace GameServer.CsScript.Action
{
    // happyData get
    // 实物兑换申请
    public class Action1009 : BaseAction
    {
        private Request1009Pack requestPack;
        private Response1009Pack responsePack;

        public Action1009(ActionGetter actionGetter)
            : base(1009, actionGetter)
        {
            responsePack = new Response1009Pack();
        }


        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1009Pack>(data);
                return true;
            }
            return false;
        }

        bool checkRefleshReplace(HappyModeData hmd, memoryRealInfoDataModel.HappyData happyData)
        {
            List<int> happyKeys = GameConfigMgr.Instance().getHappyDataKeys();
            if (happyKeys.Count <= 0)
            {
                ConsoleLog.showErrorInfo(0, "checkRefleshReplace: happyKeys.Count");
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.inner_error;
                return false;
            }

          // clear the buyItemReflesh.
            int itemID = requestPack.realItemID;
            if (hmd.realItemBuyCntInRefleshTime.Count == happyKeys.Count)
            {
                // reflesh
                int buyNumInRefleshTime = int.MaxValue;
                if (hmd.realItemBuyCntInRefleshTime.ContainsKey(requestPack.realItemID))
                {
                    buyNumInRefleshTime = hmd.realItemBuyCntInRefleshTime[itemID].cnt;
                }
                if (happyData.timeRefleshCng <= buyNumInRefleshTime)
                {
                    responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.error_timeRefleshCnt;
                    return false;
                }

                if (0 == happyData.canReplace) // once time ~~~
                {
                    persionRealItemInfo prii = hmd.RealItemInfoLst.Find((o) =>
                    {
                        return (o.realItemID == requestPack.realItemID);
                    });
                    if (null != prii)
                    {
                        responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.error_replaceBuy;
                        return false;
                    }
                }
            }
            else
            {
                ConsoleLog.showErrorInfo(0, "checkRefleshReplace: hmd.realItemBuyCntInRefleshTime.Count!= happyKeys.Count");
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.inner_error02;
                return false;
            }
            return true;
        }

        public override bool TakeAction()
        {
            int itemIndex = requestPack.realItemID;
            // int 
            var happyPersionCache = new PersonalCacheStruct<HappyModeData>();
            int keyId = utils.KeyUInt2Int(requestPack.the3rdUserID);
            HappyModeData hmd =  happyPersionCache.FindKey(keyId.ToString());
            if(hmd == null)
            {
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.not_find_happymodedata;
                return true;
            }

            // happData 01
            memoryRealInfoDataModel.HappyData happyData = GameConfigMgr.Instance().getHappyData(requestPack.realItemID);
            if (null == happyData)
            {
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.not_find_happPointConfig;
                return true;
            }

            // share realitem modify 02
            var itemcntCache = new ShareCacheStruct<ShareRealItemCnt>();
            ShareRealItemCnt sric = itemcntCache.FindKey(requestPack.realItemID);
            if(null == sric)
            {
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.not_find_item_cnt_data;
                return true;
            }

            if(false == checkRefleshReplace(hmd,happyData))
            {
                return true;
            }

            int needHappyPoint = happyData.needHappyPoint; // 配置文件总获得
            if (hmd.HappyPoint < needHappyPoint)
            {
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.not_enought_happyPoint;
                return true;
            }

            if(sric.num <= 0)
            {
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.realitem_is_empty;
                return true;
            }

            bool buyOK = false;
            if(sric.num>0)
            {
                sric.ModifyLocked(() =>
                {
                    sric.num -= 1;
                    if (sric.num >= 0)
                        buyOK = true;
                });
            }

            if (false == buyOK)
            {
                responsePack.errorCode = (byte)Response1009Pack.EnumErrorCode.realitem_is_empty;
                return true;
            }

            persionRealItemInfo rii = new persionRealItemInfo();
            rii.Index = hmd.RealItemInfoLst.Count;
            rii.UserId = requestPack.UserID;
            rii.the3rdUserId = utils.KeyUInt2Int( requestPack.the3rdUserID);
            rii.Identify = requestPack.identify;
            rii.happyPoint = hmd.HappyPoint;
            rii.needHappyPoint = needHappyPoint;
            rii.realItemID = requestPack.realItemID;
            hmd.RealItemInfoLst.Add(rii);

            // save to db for ....
            var shareRealItemCache = new ShareCacheStruct<shareRealItemInfo>();
            shareRealItemInfo shareRII = new shareRealItemInfo();
            shareRII.Index = (int)shareRealItemCache.GetNextNo();
            shareRII.UserId = requestPack.UserID;
            shareRII.the3rdUserId = utils.KeyUInt2Int( requestPack.the3rdUserID);
            shareRII.Identify = requestPack.identify;
            shareRII.happyPoint = hmd.HappyPoint;
            shareRII.needHappyPoint = needHappyPoint;
            shareRII.realItemID = requestPack.realItemID;
            shareRealItemCache.Add(shareRII);

            hmd.ModifyLocked(() =>
            {
                hmd.HappyPoint -= needHappyPoint;
                hmd.realItemBuyCntInRefleshTime[itemIndex].cnt = hmd.realItemBuyCntInRefleshTime[itemIndex].cnt + 1;
            });
            responsePack.errorCode  = (byte)Response1009Pack.EnumErrorCode.ok;
            responsePack.realItemID = rii.realItemID;
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}