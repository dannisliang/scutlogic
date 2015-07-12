using System;
using ProtoBuf;

/*下载排行榜数据*/
namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1001Pack
    {
        [ProtoMember(101)]
        public int PageIndex { get; set; }

        [ProtoMember(102)]
        public int PageSize { get; set; }

        [ProtoMember(103)]
        public int UserID { get; set; }
    }
}
