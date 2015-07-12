using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1013Pack
    {
        public enum EnumErrorCode
        {
            ok = 0,
            token_error = 1,
        }
        public Response1013Pack()
        {
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(102)]
        public string typeUser { get; set; }

        [ProtoMember(103)]
        public string result { get; set; }

        [ProtoMember(104)]
        public uint the3rdUserId { get; set; }

        [ProtoMember(104)]
        public int HappyIndex { get; set; }
    }
}
