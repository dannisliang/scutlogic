using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
    [ProtoContract]
    public class Response1002Pack
    {
      [ProtoMember(101)]

      public List<ConfigData> Datas { get; set; }
    }

    [ProtoContract]
    public class ConfigData
    {

        [ProtoMember(101)]
        public byte type;

        [ProtoMember(105)]
        public List<ushort> ext;
    }

}
