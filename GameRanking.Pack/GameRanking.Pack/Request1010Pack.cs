using System;
using ProtoBuf;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1010Pack
    {
        public enum EnumOptType
        {
            use_enterNum=0,
            use_happyRelive=1,
        }
        [ProtoMember(101)]
        public int UserID { get; set; }

        [ProtoMember(102)]
        public string identify { get; set; }

        [ProtoMember(103)]
        public string version { get; set; }

        [ProtoMember(104)]
        public uint the3rdUserID { get; set; }

        // 0,enternum
        // 1,happyrelive
        [ProtoMember(105)]
        public int actionID { get; set; }

        // 默认1
        [ProtoMember(106)]
        public int num { get; set; }

        [ProtoMember(107)]
        public string strThe3rdUserID { get; set; }
        [ProtoMember(108)]
        public string typeUser { get; set; }
    }
}