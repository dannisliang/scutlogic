using System;
using ProtoBuf;

/*兑换码获取*/
namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1003Pack
    {
        [ProtoMember(101)]
        public byte type { get; set; }
        
        [ProtoMember(102)]
        public string code { get; set; }
        
        [ProtoMember(103)]
        public int index { get; set; }
    }
}
