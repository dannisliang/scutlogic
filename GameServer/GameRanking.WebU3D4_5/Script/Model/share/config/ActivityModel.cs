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
    public class ActivityModel : ShareEntity
    {
        public ActivityModel()
        {
        }
     
        [ProtoMember(1)]
        [EntityField(true, IsIdentity=true)]
        public int id
        {
            get;
            set;
        }

        [ProtoMember(2)]
        [EntityField]
        public ushort activityid
        {
            get;
            set;
        }

        [ProtoMember(3)]
        [EntityField]
        public string version
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public DateTime begin 
        {
            get;
            set;
        }

        [ProtoMember(5)]
        [EntityField]
        public DateTime end 
        {
            get;
            set;
        }

        [ProtoMember(6)]
        [EntityField]
        public string parms 
        {
            get;
            set;
        }

        [ProtoMember(7)]
        [EntityField]
        public string descript
        {
            get;
            set;
        }

        protected override int GetIdentityId()
        {
            return id;
        }
    }
}
