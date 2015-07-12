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
using ZyGames.Framework.Model;
using Game.NSNS;

namespace GameServer.CsScript.Action
{
    // login
    // action1005上传token后获得第三方id
    // 上传第三方id，和商品id数量，
    // 返回服务器订单号。
    public class Action1006 : BaseAction
    {
        private Request1006Pack requestPack;
        private Response1006Pack responsePack;

        public Action1006(ActionGetter actionGetter)
            : base(1006, actionGetter)
        {
            responsePack = new Response1006Pack();
        }


        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1006Pack>(data);
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
           // 存入数据库
            var hmdCache = new PersonalCacheStruct<HappyModeData>();
            var cache    = new ShareCacheStruct<PayOrder>();
            int index = (int)cache.GetNextNo();
            PayOrder PayData    = new PayOrder();
            string ServerOrderId = System.Guid.NewGuid().ToString("N");
            int keyid = utils.KeyUInt2Int(requestPack.the3rdUserId);

            PayData.Index       = index;
            PayData.UserId      = requestPack.UserID;
            PayData.Identify    = requestPack.identify;
            PayData.typeUser    = requestPack.typeUser; // 360Pay..maybe
            PayData.ProductId   = requestPack.productId;
            PayData.num         = requestPack.num;
            PayData.the3rdUserId = keyid;// utils.KeyUInt2Int(requestPack.the3rdUserId);
            PayData.strThe3rdOrderId = requestPack.strThe3rdUserId;
            PayData.ServerOrderId = ServerOrderId;
            PayData.the3rdOrderId = "";
            cache.Add(PayData);

            // hmd persion
            HappyModeData hmd = hmdCache.FindKey(keyid.ToString());
            int happyPointMaxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum", 3);
            if (null == hmd)
            {
                responsePack.errorCode = 1;
                return true;
            }
            PayOrderPersion pop = new PayOrderPersion();
            pop.Index = index;
            pop.UserId = requestPack.UserID;
            pop.Identify = requestPack.identify;
            pop.typeUser = requestPack.typeUser; // 360Pay..maybe
            pop.ProductId = requestPack.productId;
            pop.num = requestPack.num;
            pop.the3rdUsrID = (int)requestPack.the3rdUserId;// utils.KeyUInt2Int(requestPack.the3rdUserId);
            pop.strThe3rdOrderId = requestPack.strThe3rdUserId;
            pop.ServerOrderId = ServerOrderId;
            pop.the3rdOrderId = "";
            hmd.PayInfoDic.Add(ServerOrderId, pop);

            // end return
            responsePack.errorCode = 0;
            responsePack.typeUser  = requestPack.typeUser;
            responsePack.result    = PayData.ServerOrderId; // 服务器订单号

            ConsoleLog.showErrorInfo(0,"create order success"+requestPack.the3rdUserId+":"+ServerOrderId);
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

    }
}