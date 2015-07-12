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
using ZyGames.Framework.Event;

namespace GameServer.Model
{
    /// <summary>
    /// shareRealItemInfo
    /// 
    /// </summary>
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Entity, "ConnData")]
    public class shareRealItemInfo : ShareEntity
    {
        public shareRealItemInfo()
            : base(false)
        {
            CreateDate = DateTime.Now;
            state = 0;
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
        public int the3rdUserId
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
        public int realItemID
        {
            get;
            set;
        }


        [ProtoMember(7)]
        [EntityField]
        public string extInfo
        {
            get;
            set;
        }

        /*是否领取0没有领取，1领取了*/
        [ProtoMember(8)]
        [EntityField]
        public int state
        {
            get;
            set;
        }

        /*领取前的happy点数*/
        [ProtoMember(9)]
        [EntityField]
        public int happyPoint
        {
            get;
            set;
        }

        // 领取需要花费的点数
        [ProtoMember(10)]
        [EntityField]
        public int needHappyPoint
        {
            get;
            set;
        }
        protected override int GetIdentityId()
        {
            return the3rdUserId;
        }
    }
    /// <summary>
    /// persionRealItemInfo
    /// 
    /// </summary>
    [Serializable, ProtoContract]
    public class persionRealItemInfo : EntityChangeEvent
    {
        public persionRealItemInfo()
            : base(false)
        {
            CreateDate = DateTime.Now;
            state = 0;
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
        public int the3rdUserId
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
        public int realItemID
        {
            get;
            set;
        }


        [ProtoMember(7)]
        public string extInfo
        {
            get;
            set;
        }

     /*是否领取0没有领取，1领取了*/
        [ProtoMember(8)]
        public int state
        {
            get;
            set;
        }

        /*领取前的happy点数*/
        [ProtoMember(9)]
        public int happyPoint
        {
            get;
            set;
        }

        // 领取需要花费的点数
        [ProtoMember(10)]
        public int needHappyPoint
        {
            get;
            set;
        }
    }

}