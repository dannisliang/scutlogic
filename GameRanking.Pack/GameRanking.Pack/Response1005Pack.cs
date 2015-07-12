using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1005Pack
    {
        public enum EnumErrorCode
        {
            ok = 0,
            token_error = 1,
            bd_token_error = 2,
            bd_item_error = 3,
            bd_result_error = 4,
            userid_isNULL = 5,
            not_find_typeUser=6,
        }
        public Response1005Pack()
        {
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(102)]
        public string typeUser { get; set; }

        [ProtoMember(103)]
        public string result  { get; set; }

        [ProtoMember(104)]
        public uint the3rdUserId { get; set; }

    }
}
