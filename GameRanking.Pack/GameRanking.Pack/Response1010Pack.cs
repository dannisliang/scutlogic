using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1010Pack
    {
        public enum EnumErrorCode
        {
            ok = 0,
            error_actionid=1,
            not_find_happyModeData=2,
            not_enought_enterNum=3,
            not_enought_happyReliveNum=4,
            not_open=5,
        }
        public Response1010Pack()
        {
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(103)]
        public int actionId { get; set; }

        [ProtoMember(104)]
        public int index { get; set; }
    }
}
