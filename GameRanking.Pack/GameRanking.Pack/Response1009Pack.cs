using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1009Pack
    {
        public enum EnumErrorCode
        {
            ok = 0,
            not_find_happymodedata=1,
            not_enought_happyPoint=2,
            not_find_item_cnt_data=3,
            realitem_is_empty=4,
            not_find_happPointConfig=5,
            error_realItemId=6,
            error_timeRefleshCnt=7,
            error_replaceBuy=8,
            inner_error = 9,
            not_find_happymodedata_whenReflesh = 10,
            inner_error02 = 11,
        }
        public Response1009Pack()
        {
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(103)]
        public int realItemID { get; set; }
    }
}
