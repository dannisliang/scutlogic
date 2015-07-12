/****************************************************************************
Copyright (c) 2013-2015 scutgame.com

http://www.scutgame.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using ProtoBuf;
using ZyGames.Framework.Game.Context;
using ZyGames.Framework.Model;
using System.Collections.Generic;
using ZyGames.Framework.Event;
using ZyGames.Framework.Cache.Generic;

namespace GameServer.Model
{
    [Serializable, ProtoContract]
    [EntityTable("ConnData")]
    public class HappyModeData : BaseEntity
    {
        public HappyModeData()
        {
            HappyPoint = 0;
            HappyReliveNum = 0;
            realItemBuyCntInRefleshTime = new CacheDictionary<int,RefleshCacheInfo>();
            PreRefleshTime = DateTime.Now;
            ActionEnterHappyPoint = new CacheDictionary<int,UserActionInfo>();
            RealItemInfoLst = new CacheList<persionRealItemInfo>();
            PayInfoDic = new CacheDictionary<string, PayOrderPersion>();
        }

        [ProtoMember(1)]
        [EntityField(true)]
        public int the3rdUserId { get; set; }

        [ProtoMember(2)]
        [EntityField]
        public int EnterNum
        {
            get;
            set;
        }

        [ProtoMember(3)]
        [EntityField]
        public int HappyPoint
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public int HappyReliveNum
        {
            get;
            set;
        }

        [ProtoMember(5)]
        [EntityField(true, ColumnDbType.LongText)]
        public CacheDictionary<int,RefleshCacheInfo> realItemBuyCntInRefleshTime
        {
            get;
            set;
        }

        [ProtoMember(6)]
        [EntityField]
        public DateTime PreRefleshTime
        {
            get;
            set;
        }

        [ProtoMember(7)]
        [EntityField(true, ColumnDbType.LongText)]
        public CacheDictionary<int,UserActionInfo> ActionEnterHappyPoint { get; set; }

        [ProtoMember(8)]
        [EntityField(true, ColumnDbType.LongText)]
        public CacheList<persionRealItemInfo> RealItemInfoLst { get; set; }

        [ProtoMember(9)]
        [EntityField(true, ColumnDbType.LongText)]
        public CacheDictionary<string,PayOrderPersion> PayInfoDic { get; set; }
        
        protected override int GetIdentityId()
        {
            return the3rdUserId;
        }



    }


    [Serializable, ProtoContract]
    public class UserActionInfo : EntityChangeEvent
    {
        public UserActionInfo()
        {
            createTime = DateTime.Now;
            status = 0;
        }

        // 0 for happyPoint
        // 1 for ...endless
        [ProtoMember(1)]
        public byte type { get; set; }
        [ProtoMember(2)]
        public int index { get; set; }

        [ProtoMember(3)]
        public DateTime createTime { get; set; }

        [ProtoMember(4)]
        public byte status { get; set; }
    }


    [Serializable, ProtoContract]
    public class PayOrderPersion : CacheItemChangeEvent
    {
        public enum PayStatus
        {
            payCreate = 0,
            paySuccess = 1,
            payFailed = 2,
        }
        public PayOrderPersion()
        {
            CreateDate = DateTime.Now;
            ServerOrderId = System.Guid.NewGuid().ToString();
            state = PayStatus.payCreate;
            process = false; // 未处理
            hasGetPayReward = false;// 未领取
        }

        [ProtoMember(1)]
        public int Index
        {
            get;
            set;
        }

        [ProtoMember(2)]
        public int UserId
        {
            get;
            set;
        }


        [ProtoMember(3)]
        public string Identify // uuid ,deviceId
        {
            get;
            set;
        }

        [ProtoMember(4)]
        public string ServerOrderId
        {
            get;
            set;
        }


        [ProtoMember(5)]
        public DateTime CreateDate
        {
            get;
            set;
        }

        [ProtoMember(6)]
        public string ProductId
        {
            get;
            set;
        }

        [ProtoMember(7)]
        public int num
        {
            get;
            set;
        }

        [ProtoMember(8)]
        public string typeUser
        {
            get;
            set;
        }

        /*
          0 , 创建订单
          1 ，支付失败
          2 ，支付成功
         */
        [ProtoMember(9)]
        public PayStatus state
        {
            get;
            set;
        }

        [ProtoMember(10)]
        public bool process
        {
            get;
            set;
        }

        [ProtoMember(11)]
        public bool hasGetPayReward
        {
            get;
            set;
        }

        [ProtoMember(12)]
        public int the3rdUsrID
        {
            get;
            set;
        }

        [ProtoMember(13)]
        public string userParms
        {
            get;
            set;
        }

        [ProtoMember(14)]
        public string jrRet
        {
            get;
            set;
        }

        [ProtoMember(15)]
        [EntityField]
        public string the3rdOrderId
        {
            get;
            set;
        }

        [ProtoMember(16)]
        [EntityField]
        public string strThe3rdOrderId
        {
            get;
            set;
        }
    }

    [Serializable, ProtoContract]
    public class RefleshCacheInfo : EntityChangeEvent
    {
        public RefleshCacheInfo()
        {
            preRefleshDate = DateTime.Now;
        }

        [ProtoMember(1)]
        public int itemId { get; set; }
        [ProtoMember(2)]
        public int cnt { get; set; }

        [ProtoMember(3)]
        public DateTime preRefleshDate { get; set; }
    }
}