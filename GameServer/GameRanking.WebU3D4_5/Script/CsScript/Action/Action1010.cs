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
    // 进入欢乐复活，或者使用欢乐服务相关道具
    public class Action1010 : BaseAction
    {
        private Request1010Pack requestPack;
        private Response1010Pack responsePack;

        public Action1010(ActionGetter actionGetter)
            : base(1010, actionGetter)
        {
            responsePack = new Response1010Pack();
        }


        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1010Pack>(data);
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            if(false == GameConfigMgr.Instance().ActivityIsOpen(requestPack.version,106))
            {
                responsePack.errorCode = (byte)Response1010Pack.EnumErrorCode.not_open;
                return true;
            }

            Request1010Pack.EnumOptType opt = requestPack.actionID;
            if (opt != Request1010Pack.EnumOptType.use_enterNum &&
                opt != Request1010Pack.EnumOptType.use_happyRelive)
            {
                responsePack.errorCode = (byte)Response1010Pack.EnumErrorCode.error_actionid;
                return  true;
            }

            var cache = new PersonalCacheStruct<HappyModeData>();
            int keyId = utils.KeyUInt2Int(requestPack.the3rdUserID);
            HappyModeData hmd = cache.FindKey(keyId.ToString());
            if(null == hmd)
            {
                responsePack.errorCode = (byte)Response1010Pack.EnumErrorCode.not_find_happyModeData;
                return true;
            }

            int actionIndexForHappy = -1;
            if (opt == Request1010Pack.EnumOptType.use_enterNum)
            {
                if(hmd.EnterNum<=0)
                {
                    responsePack.errorCode = (byte)Response1010Pack.EnumErrorCode.not_enought_enterNum;
                    return true;
                }
                hmd.ModifyLocked(() =>
                {
                    hmd.EnterNum -= 1;
                    int cnt = hmd.ActionEnterHappyPoint.Count;
                    UserActionInfo uai = new UserActionInfo();
                    uai.index = cnt;
                    uai.type = 0;
                    hmd.ActionEnterHappyPoint.Add(cnt,uai);
                    actionIndexForHappy = cnt;
                });
            }

            if (opt == Request1010Pack.EnumOptType.use_happyRelive)
            {
                if(hmd.HappyReliveNum<=0)
                {
                    responsePack.errorCode = (byte)Response1010Pack.EnumErrorCode.not_enought_happyReliveNum;
                    return true;
                }
                hmd.ModifyLocked(() => {
                    hmd.HappyReliveNum -= 1;
                });
            }

            responsePack.errorCode = (byte)Response1010Pack.EnumErrorCode.ok;
            responsePack.actionId  = (int)requestPack.actionID;
            responsePack.index     = actionIndexForHappy;
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}