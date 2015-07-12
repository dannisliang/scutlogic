using System;
using ProtoBuf;

/*上传排行榜数据*/
namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1000Pack
    {
        [ProtoMember(101)]
        public string UserName { get; set; }

        [ProtoMember(102)]
        public int Score { get; set; }

        [ProtoMember(103)]
        public string Identify { get; set; }

        [ProtoMember(104)]
        public int UserID { get; set; }

        [ProtoMember(105)]
        public string version { get; set; }

        [ProtoMember(106)]
        public float ext01 { get; set; } // time

    }
}
