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
    // 上传happyPint 以及验证
    public class Action1008 : BaseAction
    {
        private Request1008Pack requestPack;
        private Response1008Pack responsePack;

        public Action1008(ActionGetter actionGetter)
            : base(1008, actionGetter)
        {
            responsePack = new Response1008Pack();
        }


        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1008Pack>(data);
                return true;
            }
            return false;
        }

        bool check()
        {
            //data
            int check_1008_result = GameConfigMgr.Instance().getInt("check_1008_result", 400);
            int check_1008_dis = GameConfigMgr.Instance().getInt("check_ext_dis", 500);
            if(requestPack.Rate <=0 ||
               requestPack.Distance<=0 ||
               requestPack.happyPoint<=0)
            {
                responsePack.errorCode = (byte)Response1008Pack.EnumErrorCode.parm_error;
                return false;
            }

            float rate = requestPack.Rate / 100.0f;
            int ret = (int)(10000 * requestPack.happyPoint / rate / (requestPack.Distance + check_1008_dis));
            if (ret < check_1008_result)
            {
                return true;
            }
            //string infos = "ret:"+ret+"#checkResult:"+check_1008_result+"#happPoint:" + requestPack.happyPoint + "#Rate:" + rate + "#Distance:" + requestPack.Distance + "#check_1008_dis:" + check_1008_dis;
            //ConsoleLog.showErrorInfo(0, infos);
            responsePack.errorCode = (byte)Response1008Pack.EnumErrorCode.check_failed;
            return false;
        }
        public override bool TakeAction()
        {
            int happyPointFromClient = requestPack.happyPoint;
            if (happyPointFromClient < 0)
            {
                responsePack.errorCode = (byte)Response1008Pack.EnumErrorCode.error_data;
                return true;
            }

            var cache = new PersonalCacheStruct<HappyModeData>();
            int keyid = utils.KeyUInt2Int(requestPack.the3rdUserID);
            HappyModeData hmd = cache.FindKey(keyid.ToString());
            if (null == hmd)
            {
                responsePack.errorCode = (byte)Response1008Pack.EnumErrorCode.not_find_data;
                return true;
            }

            int happyPointIndex = requestPack.index;
            if(false == hmd.ActionEnterHappyPoint.ContainsKey(happyPointIndex))
            {
                responsePack.errorCode = (byte)Response1008Pack.EnumErrorCode.error_userActionIndex;
                return true;
            }
            UserActionInfo uai = hmd.ActionEnterHappyPoint[happyPointIndex];
            if(0!=uai.status)
            {
                responsePack.errorCode = (byte)Response1008Pack.EnumErrorCode.error_has_sendThis;
                return true;
            }

            // check 
            if (false == check())
            {
                return true;
            }

           

            hmd.ModifyLocked(() => {
                hmd.HappyPoint += happyPointFromClient;
                uai.status = 1;
            });
            responsePack.errorCode = (byte)Response1008Pack.EnumErrorCode.ok;
            responsePack.happyPoint = hmd.HappyPoint;
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}