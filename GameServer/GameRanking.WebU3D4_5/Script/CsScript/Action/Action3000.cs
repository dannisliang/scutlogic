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
using Game.YYS.Protocol;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using System.Diagnostics;
using System.Collections.Generic;
using ZyGames.Framework.Common.Log;
using Game.Script;
using System.IO;
using System.Text;
using System.Web;

namespace GameServer.CsScript.Action
{
    // login
    public class Action3000 : BaseAction
    {
        Action3000Response responsePack;
        Action3000Request  requestPack;

        public Action3000(ActionGetter actionGetter)
            : base(3000, actionGetter)
        {
            responsePack = new Action3000Response();
        }

        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Action3000Request>(data);
                return true;
            }
            return false;
        }
        public override bool TakeAction()
        {
            if(requestPack.UserID <0 )
            {
                var cache = new PersonalCacheStruct<GameUser>();
                int UserId = (int)cache.GetNextNo();
                GameUser gu = new GameUser();
                gu.UserId = UserId;
                gu.Identify = requestPack.identify;
                gu.version = requestPack.version;
            }

            Action3000Data d = new Action3000Data();
            d.data = int.MaxValue;
            responsePack.int_test = int.MaxValue;
            responsePack.float_test = 1.0f;
            responsePack.uint_test = uint.MaxValue;
            responsePack.list_int_test = new List<int>();
            responsePack.list_class_test = new List<Action3000Data>();
            responsePack.dic_int_test = new Dictionary<int, int>();
            responsePack.dic_class_test = new Dictionary<int, Action3000Data>();
            responsePack.list_int_test.Add(int.MaxValue);
            responsePack.list_class_test.Add(d);
            responsePack.dic_int_test.Add(0, int.MaxValue);
            responsePack.dic_class_test.Add(0, d);
            responsePack.ErrorCode = (int)GameErrorCode.Error_3000_Test01;
            //responsePack.dic_class_test[1].data = 2;
            return true;
        }
        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }
    }

}