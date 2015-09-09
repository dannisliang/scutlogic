using System;
using ProtoBuf;

namespace Game.YYS.Protocol
{
    [ProtoContract]
    [ProtoInclude(3000,typeof(Action3000Response))]
    [ProtoInclude(3001,typeof(Action3001Response))]
    [ProtoInclude(4000,typeof(Action4000Response))]
    [ProtoInclude(4001,typeof(Action4001Response))]
    public class ResponsePackBase
    {
        [ProtoMember(1)]
        public int ErrorCode{ get; set; }
    }

    [ProtoContract]
    public class ResponsePack
    {
        [ProtoMember(1)]
        public int MsgId { get; set; }

        [ProtoMember(2)]
        public int ActionId { get; set; }

        [ProtoMember(3)]
        public int ErrorCode { get; set; }
        [ProtoMember(4)]
        public string ErrorInfo { get; set; }

        [ProtoMember(5)]
        public string St { get; set; }
    }

}
