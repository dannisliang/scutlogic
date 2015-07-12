using System;
using ProtoBuf;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request1011Pack
    {
        public enum EnumOptType
        {
            add,
            modify,
            get,
        }
        [ProtoMember(101)]
        public int UserID { get; set; }

        [ProtoMember(102)]
        public string identify { get; set; }

        [ProtoMember(103)]
        public string version { get; set; }

        [ProtoMember(104)]
        public uint the3rdUserID { get; set; }

        // 0 add
        // 1 check
        // 2 modify
        [ProtoMember(105)]
        public EnumOptType optype { get; set; }

        // name
        [ProtoMember(106)]
        public string realName { get; set; }

        // 默认1
        [ProtoMember(107)]
        public string phoneNum { get; set; }


        [ProtoMember(108)]
        public string address { get; set; }

        [ProtoMember(109)]
        public string strThe3rdUserID { get; set; }
        [ProtoMember(110)]
        public string typeUser { get; set; }
    }
}