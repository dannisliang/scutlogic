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
using System.Xml;
using System.Threading;
using System.IO;
using Game.Script;
using System.Collections.Generic;

namespace GameServer.CsScript.Action
{
    public class Action1002 : BaseAction
    {
        private Response1002Pack responsePack;
        private Request1002Pack requestPack;
        public Action1002(ActionGetter actionGetter)
            : base(1000, actionGetter)
        {
            responsePack = new Response1002Pack();
        }

        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1002Pack>(data);
                return true;
            }
            return false;
        }

        void cbFunc(List<ConfigData> configLst)
        {
             //responsePack.ConfigStr = configStr;
            List<ConfigData> d = configLst as List<ConfigData>;
            responsePack.Datas = d;
        }

        public override bool TakeAction()
        {
            string version = requestPack.Version;
            string ip      = actionGetter.Session.RemoteAddress;
            {
             //   ActionConfigMgr.Instance().getData(cbFunc, version,ip);
                  GameConfigMgr.Instance().getData(cbFunc, version, ip);
            }
            return true;
        }
        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }


    }
}