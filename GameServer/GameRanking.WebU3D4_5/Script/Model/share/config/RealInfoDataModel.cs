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
    public class RealInfoDataModel : ShareEntity
    {
        public RealInfoDataModel()
        {
        }
        [ProtoMember(1)]
        [EntityField(true, IsIdentity = true)]
        public int id
        {
            get;
            set;
        }
        [ProtoMember(2)]
        [EntityField]
        public int itemID { get; set; }

        [ProtoMember(3)]
        [EntityField]
        public string name { get; set; }

        [ProtoMember(4)]
        [EntityField]
        public int needHappyPoint { get; set; }

        [ProtoMember(5)]
        [EntityField]
        public int RefleshNum { get; set; }

        [ProtoMember(6)]
        [EntityField]
        public int MinuteForReflesh { get; set; }

        [ProtoMember(7)]
        [EntityField]
        public int timeRefleshCng { get; set; }

        [ProtoMember(8)]
        [EntityField]
        public int canReplace { get; set; }

        [ProtoMember(9)]
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
