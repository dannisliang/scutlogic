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
using System.Collections.Generic;

namespace ZyGames.Quanmin.Test.Case
{
    /// <summary>
    /// 登录
    /// </summary>
    public class Step1002 : CaseStep
    {
        static int index = 0;
        Response1002Pack responsePack = null;
        List<string> version;
        
        protected override void SetUrlElement()
        {
            version = new List<string>();
            version.Add("1.01");
            version.Add("1.02");
            version.Add("1.03");
            version.Add("1.04");
            version.Add("1.05");
            version.Add("1.06");
            version.Add("1.07");
            int index = RandomUtils.GetRandom(0, 2) ;
            Request1002Pack req = new Request1002Pack();
            req.Version = "1.07";// version[index];
           // req.Ip = "123.57.73.204";
            byte[] data = ProtoBufUtils.Serialize(req);
            netWriter.SetBodyData(data);

        }

        protected override bool DecodePacket(MessageStructure reader, MessageHead head)
        {
            responsePack = ProtoBufUtils.Deserialize<Response1002Pack>(netReader.Buffer);
            string responseDataInfo = "";
            responseDataInfo = System.DateTime.Now.ToString() + " acction success 1002:" + netReader.Buffer.Length + "\nInfo###########\n";
            foreach(var d in responsePack.Datas)
            {
                responseDataInfo += d.type +  "->";
                if (d.ext == null) responseDataInfo = "ffffxxx=====" + responseDataInfo;
                if (d.ext != null)
                {
                    foreach (var v in d.ext)
                    {
                        responseDataInfo += v + ",";
                    }
                }
              
                  responseDataInfo += "\n";
              }
              System.Console.WriteLine(responseDataInfo);
            return true;
        }

    }
}