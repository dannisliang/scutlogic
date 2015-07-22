using System;
using ProtoBuf;
using System.Collections.Generic;

namespace Game.YYS.Protocol
{
    [ProtoContract]
    public class Action3001Request : RequestPackDataBase
    {
        [ProtoMember(101)]
        public int index { get; set; }
    }

    [ProtoContract]
    public class Action3001Response : ResponsePackBase
    {
        [ProtoMember(101)]
        public int index { get; set; }
    }
}
