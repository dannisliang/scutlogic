using System;
using ProtoBuf;

/*上传排行榜数据*/
namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request3000Pack
    {
        [ProtoMember(101)]
        public int UserID { get; set; }
        [ProtoMember(102)]
        public string Identify { get; set; }

        [ProtoMember(103)]
        public string version { get; set; }
    }
}
