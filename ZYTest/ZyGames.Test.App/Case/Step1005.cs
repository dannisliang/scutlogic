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

using ZyGames.Framework.Common;
using ZyGames.Framework.RPC.IO;
using ZyGames.Test;
using GameRanking.Pack;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Common.Configuration;

namespace ZyGames.Quanmin.Test.Case
{
    /// <summary>
    /// 创角
    /// </summary>
    public class Step1005 : CaseStep
    {

        Response1005Pack responsePack;
        protected override void SetUrlElement()
        {
            Request1005Pack requestPack = new Request1005Pack();
            requestPack.token = "06433cd6e21d45f79a95c8e2ac9027c1-9d56aacb28bda0b9457bf9079bc715d9-20141222181929-ce629633cfaaf2bb1681719fc9b1f1ac-3b2927c8419cda02063bb7180deeb67a-f7ee2d74e5adcb026d457c2f7caee153";
            requestPack.typeUser = "YYS_CP360";
            requestPack.UserID = 222222;
            byte[] data = ProtoBufUtils.Serialize(requestPack);
            netWriter.SetBodyData(data);
        }

        protected override bool DecodePacket(MessageStructure reader, MessageHead head)
        {
            responsePack = ProtoBufUtils.Deserialize<Response1005Pack>(netReader.Buffer);
            string responseDataInfo = "";
            responseDataInfo = indentify + " acction success: " + responsePack.result + "3rdID:" + responsePack.the3rdUserId;
;
             System.Console.WriteLine(responseDataInfo);
            return true;
        }

    }
}