using System;
using ProtoBuf;

/*获取配置文件*/
namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1002Pack
    {
        [ProtoMember(101)]
        public string Version { get; set; }

        [ProtoMember(102)]
        public string Ip      { get; set; }
    }
}
