using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ZyGames.Framework.Model;
using ZyGames.Framework.Cache.Generic;

namespace GameServer.Model
{
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Entity, "ConnData")]
    public class ServerInfoMode : ShareEntity
    {
        public ServerInfoMode()
        {
            wight = 0;
            offLineCnt = -1;
            userd = false;
        }
        [ProtoMember(1)]
        [EntityField(true)]
        public int id
        {
            get;
            set;
        }
        [ProtoMember(2)]
        [EntityField]
        public int type
        {
            get;
            set;
        }

        [ProtoMember(3)]
        [EntityField]
        public string ip
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public uint ipUint
        {
            get;
            set;
        }

        [ProtoMember(5)]
        [EntityField]
        public int wight
        {
            get;
            set;
        }

        [ProtoMember(6)]
        [EntityField]
        public int offLineCnt
        {
            get;
            set;
        }

        [ProtoMember(7)]
        [EntityField]
        public bool userd
        {
            get;
            set;
        }

    }
}
