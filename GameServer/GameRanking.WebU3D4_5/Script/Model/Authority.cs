using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ZyGames.Framework.Model;

namespace GameServer.Model
{
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Entity, "ConnData")]
    public class Authority : ShareEntity
    {
        [ProtoMember(1)]
        [EntityField(true)]
        public int id
        {
            get;
            set;
        }
        [ProtoMember(2)]
        [EntityField]
        public string name
        {
            get;
            set;
        }

        [ProtoMember(3)]
        [EntityField]
        public string pwd
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public int level
        {
            get;
            set;
        }
    }
}
