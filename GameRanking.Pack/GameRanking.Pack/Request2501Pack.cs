using System;
using ProtoBuf;


namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request2501Pack
    {
        [ProtoMember(101)]
        public int userID { get; set; }

        [ProtoMember(102)]
        public string identify { get; set; }

        [ProtoMember(103)]
        public string parm { get; set; }
    }
}
