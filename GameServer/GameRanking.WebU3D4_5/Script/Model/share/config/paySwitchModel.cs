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
    public class PaySwitchModel : ShareEntity
    {
        public PaySwitchModel()
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
        public string version
        {
            get;
            set;
        }

        [ProtoMember(3)]
        [EntityField]
        public string ipKey
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public byte ChinaMobile
        {
            get;
            set;
        }
        [ProtoMember(5)]
        [EntityField]
        public byte ChinaUnicom
        {
            get;
            set;
        }

        [ProtoMember(6)]
        [EntityField]
        public  byte ChinaTelecom
        {
            get;
            set;
        }

        [ProtoMember(7)]
        [EntityField]
        public byte ChinaMobile_360
        {
            get;
            set;
        }
        [ProtoMember(8)]
        [EntityField]
        public byte ChinaUnicom_360
        {
            get;
            set;
        }

        [ProtoMember(9)]
        [EntityField]
        public byte ChinaTelecom_360
        {
            get;
            set;
        }

        [ProtoMember(10)]
        [EntityField]
        public byte ChinaMobile_BD 
        {
            get;
            set;
        }

        [ProtoMember(11)]
        [EntityField]
        public byte ChinaUnicom_BD 
        {
            get;
            set;
        }

        [ProtoMember(12)]
        [EntityField]
        public byte ChinaTelecom_BD 
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
