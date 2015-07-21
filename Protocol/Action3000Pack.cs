using System;
using ProtoBuf;
using System.Collections.Generic;

namespace Game.YYS.Protocol
{
    [ProtoContract]
    public class Action3000Request : RequestPackDataBase
    {
         [ProtoMember(101)]
         public int type { get; set; }
    }

    [ProtoContract]
    public class Action3000Response : ResponsePackBase
    {
        [ProtoMember(101)]
        public string string_test { get; set; }

        [ProtoMember(102)]
        public int int_test { get; set; }

        [ProtoMember(103)]
        public uint uint_test { get; set; }

        [ProtoMember(104)]
        public float float_test { get; set; }

        [ProtoMember(105)]
        public List<int> list_int_test { get; set; }

        [ProtoMember(106)]
        public List<Action3000Data> list_class_test { get; set; }


        [ProtoMember(107)]
        public Dictionary<int, int> dic_int_test { get; set; }

        [ProtoMember(108)]
        public Dictionary<int, Action3000Data> dic_class_test { get; set; }
    }

     [ProtoContract]
    public class Action3000Data
    {
         [ProtoMember(101)]
         public int data { get; set; }
    }
}
