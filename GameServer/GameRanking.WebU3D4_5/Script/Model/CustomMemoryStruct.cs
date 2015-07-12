using System;
using ProtoBuf;
using ZyGames.Framework.Model;
using System.Collections.Generic;

namespace GameServer.Model
{
    public class memeroyString : MemoryEntity
    {
        public string str { get; set; }
    }

    public class MyEntity : MemoryEntity
    {
        public string ID { get; set; }
        public List<UserRanking> Data { get; set; }
    }


    public class UserAddMemoryEntity : MemoryEntity
    {
        public int UserID { get; set; }
        public int Score     { get; set; }
        public string UserName { get; set; }
        public string Identify { get; set; }
    }
}
