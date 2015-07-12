using System;
using ProtoBuf;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1004Pack
    {
        [ProtoMember(101)]
        public int UserID { get; set; }

        [ProtoMember(102)]
        public string identify { get; set; }

        [ProtoMember(103)]
        public string version { get; set; }


        /*
        0: check 
        1: getRewardSuccess
        */
        [ProtoMember(104)]
        public int status { get; set; }

        /*
         0 = rank for dimond
         1 = check for order
         */
        [ProtoMember(105)]
        public byte actionID { get; set; }


        [ProtoMember(106)]
        public string infoExt { get; set; }
    }
}