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
     // 通过the3rdUserID 获取活动数据
    public class Action1007 : BaseAction
    {
        private Request1007Pack requestPack;
        private Response1007Pack responsePack;

        public Action1007(ActionGetter actionGetter)
            : base(1007, actionGetter)
        {
            responsePack = new Response1007Pack();
        }


        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1007Pack>(data);
                return true;
            }
            return false;
        }
        void doRefleshEnterTimer(HappyModeData hmd)
        {
            if (null == hmd) return;

            string timestr = GameConfigMgr.Instance().getString("time_hdm_cnt_timming", "05:15");
            System.DateTime tody = System.Convert.ToDateTime(timestr);
            int maxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum", 3);
            hmd.ModifyLocked(() =>
            {
                if (System.DateTime.Now > tody && hmd.PreRefleshTime < tody)
                {
                    hmd.PreRefleshTime = tody;
                    hmd.EnterNum = maxEnterNum;
                }
            });
        }
        public override bool TakeAction()
        {
            // 存入数据库
            var cache = new PersonalCacheStruct<HappyModeData>();
            
            // todo test
            // test -1
            HappyModeData testHMD = new HappyModeData();
            testHMD.the3rdUserId = -1;// (int)cache.GetNextNo();
            testHMD.HappyPoint = 888;
            cache.Add(testHMD);

            testHMD = cache.FindKey("-1");
            // test

            
            int keyid = utils.KeyUInt2Int(requestPack.the3rdUserID);
            HappyModeData hmd = cache.FindKey(keyid.ToString());
            int happyPointMaxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum", 3);
            if (null == hmd)
            {
                responsePack.errorCode = (byte)Response1007Pack.EnumErrorCode.not_findHMD;
                return true;
            }
            ConsoleLog.showErrorInfo(0, "hmd:" + hmd.the3rdUserId + "req:" + requestPack.the3rdUserID);

            doRefleshEnterTimer(hmd); // 刷新enterNum
            responsePack.errorCode      = (byte)Response1007Pack.EnumErrorCode.ok;
            responsePack.enterNum       = hmd.EnterNum;
            responsePack.happyPoint     = hmd.HappyPoint;
            responsePack.happyReLiveNum = hmd.HappyReliveNum;
            responsePack.maxEnterNum    = happyPointMaxEnterNum;

            
            if (requestPack.dateType != 1) return true; // not return realitem infos
            
            foreach (var rii in hmd.RealItemInfoLst)
            {
                Action1007RealItem timeInfo = new Action1007RealItem();
                timeInfo.id = rii.realItemID;
                timeInfo.theTime = rii.CreateDate;
                responsePack.realItemsIds.Add(timeInfo);
            }

            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}