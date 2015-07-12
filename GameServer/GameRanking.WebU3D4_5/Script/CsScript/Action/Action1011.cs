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
    // 玩家上传个/获取/修改人数据。名字，电话，地址
    public class Action1011 : BaseAction
    {
        private Request1011Pack requestPack;
        private Response1011Pack responsePack;

        public Action1011(ActionGetter actionGetter)
            : base(1011, actionGetter)
        {
            responsePack = new Response1011Pack();
        }


        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1011Pack>(data);
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            Request1011Pack.EnumOptType opt = requestPack.optype;
            var cache = new PersonalCacheStruct<RealUserInfo>();
            RealUserInfo rui = null;
            int keyId = utils.KeyUInt2Int(requestPack.the3rdUserID);
            string keyIdString = keyId.ToString();
            if(opt == Request1011Pack.EnumOptType.add)
            {
                rui = cache.FindKey(keyIdString);
                if(null != rui)
                {
                    responsePack.errorCode = (byte)Response1011Pack.EnumErrorCode.has_data_for_add;
                    return true;
                }
                rui = new RealUserInfo();
                rui.the3rdUserI = utils.KeyUInt2Int( requestPack.the3rdUserID);
                rui.name = requestPack.realName;
                rui.PhoneNum = requestPack.phoneNum;
                rui.address = requestPack.address;
                cache.Add(rui);
            }
            else if (Request1011Pack.EnumOptType.modify == opt)
            {
                rui = cache.FindKey(keyIdString);
                if (null == rui)
                {
                    responsePack.errorCode = (byte)Response1011Pack.EnumErrorCode.not_find_for_modify;
                    return true;
                }
                rui.ModifyLocked(() => {
                    rui.name = requestPack.realName;
                    rui.PhoneNum = requestPack.phoneNum;
                    rui.address = requestPack.address;
                });
            }
            else if (Request1011Pack.EnumOptType.get == opt)
            {
                rui = cache.FindKey(keyIdString);
                if (null == rui)
                {
                    responsePack.errorCode = (byte)Response1011Pack.EnumErrorCode.not_find_for_get;
                    return true;
                }
            }
            else
            {
                responsePack.errorCode = (byte)Response1011Pack.EnumErrorCode.error_opt;
                return true;
            }

            responsePack.errorCode = (byte)Response1011Pack.EnumErrorCode.ok;
            responsePack.realName = rui.name;
            responsePack.phoneNum = rui.PhoneNum;
            responsePack.address  = rui.address;
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}