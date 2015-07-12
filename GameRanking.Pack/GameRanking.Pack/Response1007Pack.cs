using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1007Pack
    {
        public enum EnumErrorCode
        {
            ok=0,
            no_data=1,
            not_findHMD = 2,
        }
        public Response1007Pack()
        {
            realItemsIds = new List<Action1007RealItem>();
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(102)]
        public int enterNum { get; set; }

        [ProtoMember(103)]
        public int happyPoint { get; set; }

        [ProtoMember(104)]
        public int happyReLiveNum { get; set; }


        [ProtoMember(105)]
        public int maxEnterNum { get; set; }

        [ProtoMember(106)]
        public List<Action1007RealItem> realItemsIds { get; set; }

    }

    [ProtoContract]
    public class Action1007RealItem
    {
        [ProtoMember(101)]
        public int id { get; set; }

        [ProtoMember(102)]
        public DateTime theTime { get; set; }
    }
}
