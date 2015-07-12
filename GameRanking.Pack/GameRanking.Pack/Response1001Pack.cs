using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1001Pack
    {
        public Response1001Pack()
        {
            Items = new List<RankData>();
            ItemsExScore = new List<RankData>();
        }

        [ProtoMember(101)]
        public int PageCount { get; set; }


        [ProtoMember(102)]
        public List<RankData> Items { get; set; }

        [ProtoMember(103)]
        public List<RankData> ItemsExScore { get; set; }

        [ProtoMember(104)]
        public int totalPlayer { get; set; }


        [ProtoMember(105)]
        public int youPos { get; set; }

        [ProtoMember(106)]
        public int hightScore { get; set; }

        [ProtoMember(107)]
        public int lowScore { get; set; }

    }

    [ProtoContract]
    public class RankData
    {

        [ProtoMember(101)]
        public string UserName { get; set; }

        [ProtoMember(102)]
        public int Score { get; set; }

        [ProtoMember(103)]
        public int UserID { get; set; }

        [ProtoMember(104)]
        public int pos { get; set; }
    }
}
