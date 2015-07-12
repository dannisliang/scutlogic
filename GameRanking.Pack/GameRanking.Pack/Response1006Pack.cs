using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1006Pack
    {
        public Response1006Pack()
        {
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(102)]
        public string typeUser { get; set; }

        [ProtoMember(103)]
        public string result { get; set; }
    }
}
