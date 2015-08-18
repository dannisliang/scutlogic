using System;
using ProtoBuf;
using System.Collections.Generic;

namespace Game.YYS.Protocol
{
    [ProtoContract]
    public class Action4000Request : RequestPackDataBase
    {
        [ProtoMember(101)]
        public int index { get; set; }
    }

    [ProtoContract]
    public class Action4000Response : ResponsePackBase
    {
        public Action4000Response()
        {
            data = new Dictionary<int, Response4000RouteData>();
        }
        [ProtoMember(101)]
        public Dictionary<int, Response4000RouteData> data { get; set; }
    }

    [ProtoContract]
    public class Response4000RouteData
    {
        [ProtoMember(1)]
        public int id { get; set; }
    }
}
