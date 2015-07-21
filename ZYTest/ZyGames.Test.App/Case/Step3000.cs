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
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Common.Configuration;
using Game.YYS.Protocol;

namespace ZyGames.Quanmin.Test.Case
{
    /// <summary>
    /// 登录
    /// </summary>
    public class Step3000 : CaseStep
    {
        public Action3000Response responsePack = null;
        protected override void SetUrlElement()
        {
            Action3000Request req = new Action3000Request();
            req.type = 1;
            byte[] data = ProtoBufUtils.Serialize(req);
            netWriter.SetBodyData(data);
        }

        protected override bool DecodePacket(MessageStructure reader, MessageHead head)
        {
            try
            {
                responsePack = ProtoBufUtils.Deserialize<Action3000Response>(netReader.Buffer);
                string responseDataInfo = "";
                responseDataInfo = "acction success: " + responsePack.ErrorCode;
                // responseDataInfo += "\nint:"+responsePack.int_test;
                // responseDataInfo += "\nfloat:"+responsePack.float_test;
                // responseDataInfo += "\nuint:" + responsePack.uint_test;
                // responseDataInfo += "\nlist_int:" + responsePack.list_int_test[0];
                // responseDataInfo += "\nlist_class:" + responsePack.list_class_test[0].data;
                // responseDataInfo += "\ndic_int:" + responsePack.dic_int_test[0];
                // responseDataInfo += "\ndic_class:" + responsePack.dic_class_test[0].data;
                System.Console.WriteLine(responseDataInfo);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
     
            return true;
        }

    }
}