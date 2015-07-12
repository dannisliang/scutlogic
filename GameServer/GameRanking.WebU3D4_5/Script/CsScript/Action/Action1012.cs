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
    // 获得欢乐复活的实物数据。
    public class Action1012 : BaseAction
    {
        private Request1012Pack requestPack;
        private Response1012Pack responsePack;

        public Action1012(ActionGetter actionGetter)
            : base(1012, actionGetter)
        {
            responsePack = new Response1012Pack();
        }

        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1012Pack>(data);
                return true;
            }
            return false;
        }

        void doReflesh(HappyModeData hmd, List<int> happyKeys)
        {
            if (null == hmd) return;

            if (hmd.realItemBuyCntInRefleshTime.Count != happyKeys.Count)
            {
                hmd.ModifyLocked(() =>
                {
                    hmd.realItemBuyCntInRefleshTime.Clear();
                    foreach (int k in happyKeys)
                    {
                        RefleshCacheInfo info = new RefleshCacheInfo();
                        info.itemId = k;
                        info.cnt = 0;
                        hmd.realItemBuyCntInRefleshTime.Add(k, info);
                    }
                });
            }

            var shareCacheRealItemCnt = new ShareCacheStruct<ShareRealItemCnt>();
            hmd.ModifyLocked(() =>
            {
                for (int k = 0; k < happyKeys.Count; ++k)
                {
                    int id = happyKeys[k];
                    ShareRealItemCnt cntData = shareCacheRealItemCnt.FindKey(id.ToString());
                    if (cntData != null && hmd.realItemBuyCntInRefleshTime.ContainsKey(id))
                    {
                        if (hmd.realItemBuyCntInRefleshTime[id].preRefleshDate <= cntData.preUpdateTime)
                        {
                            hmd.realItemBuyCntInRefleshTime[id].cnt = 0;
                            hmd.realItemBuyCntInRefleshTime[id].preRefleshDate = cntData.preUpdateTime.AddSeconds(1);
                        }
                    }
                    else
                    {
                        ConsoleLog.showErrorInfo(0, "Refresh Acton1009");
                        TraceLog.WriteError("Refresh Acton1009");
                    }
                }
            });
        }

        public override bool TakeAction()
        {
            var cache = new ShareCacheStruct<ShareRealItemCnt>();
            var persionCache = new PersonalCacheStruct<HappyModeData>();
            int keyId = utils.KeyUInt2Int(requestPack.the3rdUserID);
            HappyModeData hmd = persionCache.FindKey(keyId.ToString());
            List<int> keys = GameConfigMgr.Instance().getHappyDataKeys();
            if(hmd!=null)
            {
                doReflesh(hmd, keys);
            }

            for (int i = 0; i < keys.Count; ++i)
            {
                GameConfigHappyPoint.HappyData hd = GameConfigMgr.Instance().getHappyData(keys[i]);
                ShareRealItemCnt sric = cache.FindKey(keys[i]);
                if(null != hd && null !=sric)
                {
                    RealItemData rid = new RealItemData();
                    rid.id   = hd.itemID;
                    rid.name = hd.name;
                    rid.happyPoint = hd.needHappyPoint;
                    rid.num = sric.num;
                    rid.timeForReflesh = (sric.preUpdateTime.AddMinutes(hd.MinuteForReflesh) - System.DateTime.Now);
                    rid.uiStatus = 0;

                    if (0 == rid.num) rid.uiStatus = 1;
                    if(null != hmd)
                    {
                        bool findIt = false;
                        if(hmd.realItemBuyCntInRefleshTime.ContainsKey(rid.id))
                        {
                            findIt = hmd.realItemBuyCntInRefleshTime[rid.id].cnt > 0;
                        }

                        bool canReplace = hd.canReplace == 1;
                        if (false == canReplace)
                        {
                            if (hmd.RealItemInfoLst.Exists((o) => { return o.realItemID == rid.id; }))
                                rid.uiStatus = 2;
                        }
                        else
                        {
                            if (findIt)
                                rid.uiStatus = 2;
                        }
                    }
                    responsePack.Data.Add(rid);
                }
                else
                {
                     ConsoleLog.showErrorInfo(0,"is null"+(null==hd).ToString()+":"+(null==sric).ToString());
                }
            }
            ConsoleLog.showErrorInfo(0, "responsePack cnt:"+responsePack.Data.Count);
            responsePack.errorCode = 0;
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}