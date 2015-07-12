using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1008Pack
    {
        public enum EnumErrorCode
        {
            ok = 0,
            error_data=1,
            check_failed=2,
            not_find_data=3,
            parm_error=4,
            error_index=5,
            error_userActionIndex=6,
            error_has_sendThis=7,
        }
        public Response1008Pack()
        {
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(103)]
        public int happyPoint { get; set; }
    }
}
