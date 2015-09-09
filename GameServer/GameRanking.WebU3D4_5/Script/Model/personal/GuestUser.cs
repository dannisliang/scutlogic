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

namespace GameServer.Model
{
    [Serializable, ProtoContract]
    [EntityTable("ConnData")]
    public class GameUser : BaseUser
    {

        public GameUser()
        {
        }

        [ProtoMember(1)]
        [EntityField(true)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        [EntityField]
        public String NickName
        {
            get;
            set;
        }

        [ProtoMember(3)]
        [EntityField]
        public String PassportId
        {
            get;
            set;
        }

        [ProtoMember(4)]
        [EntityField]
        public String RetailId
        {
            get;
            set;
        }

        [ProtoMember(5)]
        [EntityField]
        public String Identify
        {
            get;
            set;
        }

        [ProtoMember(6)]
        [EntityField]
        public int Score
        {
            get;
            set;
        }

        [ProtoMember(7)]
        [EntityField]
        public DateTime CreateTime
        {
            get;
            set;
        }

        [ProtoMember(8)]
        [EntityField]
        public String version
        {
            get;
            set;
        }

        [ProtoMember(9)]
        [EntityField]
        public int state
        {
            get;
            set;
        }

        [ProtoMember(10)]
        [EntityField]
        public int Diamond
        {
            get;
            set;
        }

        [ProtoMember(11)]
        [EntityField]
        public int theTotal
        {
            get;
            set;
        }
        [ProtoMember(12)]
        [EntityField]
        public int preRanking
        {
            get;
            set;
        }

        [ProtoMember(13)]
        [EntityField]
        public DateTime CompensationDate
        {
            get;
            set;
        }
        public string SId { get; set; }

        protected override int GetIdentityId()
        {
            return UserId;
        }

    //   public override string GetSessionId()
    //   {
    //       return SId;
    //   }

        public override int GetUserId()
        {
            return UserId;
        }

        public override string GetNickName()
        {
            return NickName;
        }

        public override string GetPassportId()
        {
            return PassportId;
        }

        public override string GetRetailId()
        {
            return RetailId;
        }

        public override bool IsLock
        {
            get { return false; }
        }

    //   public override DateTime OnlineDate
    //   {
    //       get;
    //       set;
    //   }
    }

}