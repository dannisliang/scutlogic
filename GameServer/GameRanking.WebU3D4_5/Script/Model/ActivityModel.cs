using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ZyGames.Framework.Model;

namespace GameServer.Model
{
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Entity, "ConnData")]
    public class ActivityModel : ShareEntity
    {
        [ProtoMember(1)]
        [EntityField(true)]
        public int activityID
        {
            get;
            set;
        }
        [ProtoMember(2)]
        [EntityField]
        public DateTime begin 
        {
            get;
            set;
        }

        [ProtoMember(3)]
        [EntityField]
        public DateTime end 
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public string parms 
        {
            get;
            set;
        }
    }
}
