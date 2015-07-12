using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1011Pack
    {
        public enum EnumErrorCode
        {
            ok = 0,
            error_opt,
            has_data_for_add,
            not_find_for_modify,
            not_find_for_get
        }
        public Response1011Pack()
        {
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }
        // name
        [ProtoMember(102)]
        public string realName { get; set; }

        // 默认1
        [ProtoMember(103)]
        public string phoneNum { get; set; }


        [ProtoMember(104)]
        public string address { get; set; }
    }
}
