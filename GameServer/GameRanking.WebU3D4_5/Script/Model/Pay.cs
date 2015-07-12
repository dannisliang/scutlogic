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
using ZyGames.Framework.Model;
using System.Collections.Generic;

namespace GameServer.Model
{
    /// <summary>
    /// 玩家排行榜实体类
    /// </summary>
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Entity, "ConnData")]
    public class PayOrder : ShareEntity
    {
        public enum PayStatus
        {
            payCreate=0,
            paySuccess=1,
            payFailed=2,
        }
        public PayOrder()
            : base(false)
        {
            CreateDate = DateTime.Now;
            state = PayStatus.payCreate; 
            process = false; // 未处理
            hasGetPayReward = false;// 未领取
        }

        [ProtoMember(1)]
        [EntityField(true)]
        public int Index
        {
            get;
            set;
        }

        [ProtoMember(2)]
        [EntityField]
        public int UserId 
        {
            get;
            set;
        }


        [ProtoMember(3)]
        [EntityField]
        public string Identify // uuid ,deviceId
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public string ServerOrderId
        {
            get;
            set;
        }


        [ProtoMember(5)]
        [EntityField]
        public DateTime CreateDate
        {
            get;
            set;
        }

        [ProtoMember(6)]
        [EntityField]
        public string ProductId
        {
            get;
            set;
        }

        [ProtoMember(7)]
        [EntityField]
        public int num
        {
            get;
            set;
        }

        [ProtoMember(8)]
        [EntityField]
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
        [EntityField]
        public PayStatus state
        {
            get;
            set;
        }

        [ProtoMember(10)]
        [EntityField]
        public bool process
        {
            get;
            set;
        }

        [ProtoMember(11)]
        [EntityField]
        public bool hasGetPayReward
        {
            get;
            set;
        }

        [ProtoMember(12)]
        [EntityField]
        public int the3rdUserId
        {
            get;
            set;
        }

        [ProtoMember(13)]
        [EntityField]
        public string userParms
        {
            get;
            set;
        }

        [ProtoMember(14)]
        [EntityField]
        public string the3rdOrderId
        {
            get;
            set;
        }

        [ProtoMember(15)]
        [EntityField]
        public string strThe3rdOrderId
        {
            get;
            set;
        }
        protected override int GetIdentityId()
        {
            return Index;
        }
    }

}