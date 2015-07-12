using System;
using ProtoBuf;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1008Pack
    {
        [ProtoMember(101)]
        public int UserID { get; set; }

        [ProtoMember(102)]
        public string identify { get; set; }

        [ProtoMember(103)]
        public string version { get; set; }

        [ProtoMember(104)]
        public uint the3rdUserID { get; set; }


        [ProtoMember(105)]
        public int happyPoint { get; set; }


        [ProtoMember(106)]
        public int Rate { get; set; }

        [ProtoMember(107)]
        public int Distance { get; set; }

        [ProtoMember(108)]
        public int index { get; set; }

        [ProtoMember(109)]
        public string strThe3rdUserID { get; set; }
        [ProtoMember(110)]
        public string typeUser { get; set; }
    }
}