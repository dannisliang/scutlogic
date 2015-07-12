using System;
using ProtoBuf;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1003Pack
    {
        
        [ProtoMember(101)]
        public int result  {get;set;}

        [ProtoMember(102)]
        public byte type { get; set; }

        [ProtoMember(103)]
        public string code { get; set; }

        [ProtoMember(104)]
        public int Index { get; set; }
    }
}
