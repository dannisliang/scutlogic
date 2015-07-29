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

namespace ZyGames.Quanmin.Test.Case
{
    /// <summary>
    /// 登录
    /// </summary>
    public class Step1001 : CaseStep
    {
        static int index = 0;
        Response1001Pack responsePack = null;
        protected override void SetUrlElement()
        {
            Request1001Pack req = new Request1001Pack();
            req.PageIndex = 76367;// RandomUtils.GetRandom(1, 10000);
            req.PageSize  = 1;
            req.UserID = 111111;
            req.version = "1.09";
            byte[] data = ProtoBufUtils.Serialize(req);
            netWriter.SetBodyData(data);
        }

        protected override bool DecodePacket(MessageStructure reader, MessageHead head)
        {
            responsePack = ProtoBufUtils.Deserialize<Response1001Pack>(netReader.Buffer);
             string responseDataInfo = "";
             responseDataInfo = indentify + " acction success: item.count:" + responsePack.Items.Count;
             foreach(var d in responsePack.Items)
             {
                 //break;
                 responseDataInfo += "\nuserid:" + d.UserID;
                 responseDataInfo += " UserName:" + d.UserName;
                 responseDataInfo += " pos:" + d.pos;
                 responseDataInfo += " Score:" + d.Score;
             }
             responseDataInfo += "\nExScore:" + responsePack.ItemsExScore.Count;
             foreach (var d in responsePack.ItemsExScore)
             {
                 responseDataInfo += "\nuserid:" + d.UserID;
                 responseDataInfo += " UserName:" + d.UserName;
                 responseDataInfo += " pos:" + d.pos;
                 responseDataInfo += " Score:" + d.Score;
             }
             //System.Console.WriteLine(responseDataInfo);
             DecodePacketInfo = responseDataInfo;
            return true;
        }

    }
}