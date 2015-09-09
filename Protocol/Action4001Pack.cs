using System;
using ProtoBuf;
using System.Collections.Generic;

namespace Game.YYS.Protocol
{
    [ProtoContract]
    public class Action4001Request : RequestPackDataBase
    {
        [ProtoMember(101)]
        public string identify{ get; set; }

    }

    [ProtoContract]
    public class Action4001Response : ResponsePackBase
    {
        public Action4001Response()
        {
        }
        [ProtoMember(101)]

        int UserID{ get; set; }
    }
}
