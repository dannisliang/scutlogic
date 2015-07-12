using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response2501Pack
    {
        public Response2501Pack()
        {
        }
        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(102)]
        public string result { get; set; }
    }
}
