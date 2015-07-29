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
        public Action3001Response()
        {
            data = new Dictionary<int, Response3001RouteData>();
        }
        [ProtoMember(101)]
        public Dictionary<int, Response3001RouteData> data { get; set; }
    }

    [ProtoContract]
    public class Response3001RouteData
    {
        [ProtoMember(1)]
        public int id { get; set; }

    }
}
