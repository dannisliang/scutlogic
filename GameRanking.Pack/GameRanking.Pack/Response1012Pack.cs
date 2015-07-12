using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1012Pack
    {
        public enum EnumErrorCode
        {
            ok = 0,
        }
        public Response1012Pack()
        {
            Data = new List<RealItemData>();
        }

        [ProtoMember(101)]
        public byte errorCode { get; set; }

        [ProtoMember(102)]
        public List<RealItemData> Data { get; set; }
    }


    [ProtoContract]
    public class RealItemData
    {

        [ProtoMember(101)]
        public int id { get; set; }

        [ProtoMember(102)]
        public int happyPoint { get; set; }

        [ProtoMember(103)]
        public int num { get; set; }

        [ProtoMember(104)]
        public string name { get; set; }

        [ProtoMember(105)]
        public TimeSpan timeForReflesh { get; set; }

        [ProtoMember(106)]
        public byte uiStatus { get; set; }
    }
}
