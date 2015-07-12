using System;
using ProtoBuf;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1006Pack
    {
        [ProtoMember(101)]
        public int UserID { get; set; }

        [ProtoMember(102)]
        public string identify { get; set; }

        [ProtoMember(103)]
        public string version { get; set; }

        [ProtoMember(104)]
        public string productId { get; set; }


        [ProtoMember(105)]
        public int num { get; set; }

        [ProtoMember(106)]
        public uint the3rdUserId { get; set; }

        [ProtoMember(107)]
        public string typeUser { get; set; }

        [ProtoMember(108)]
        public string strThe3rdUserId { get; set; }
    }
}