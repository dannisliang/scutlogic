using System;
using ProtoBuf;
using System.Collections.Generic;

namespace GameRanking.Pack
{
   

    [ProtoContract]
    public class Response1004Pack
    {
        public enum EnumErrorCode
        {
            // normal
            ok = 0,
            action_error = 1,
            identify_not_match = 2,
            user_not_find = 3,
            ok_but_not_dimond = 4,
            status_error = 5,


            // pay
            order_not_find = 10,
            hasGetPayReward = 11,

            //
            compensation_not_findUser = 20,
            no_data01 = 21,
            no_data02 = 22,

        }
        public Response1004Pack()
        {
            Result = new List<int>();
            des = "";
        }
        /*
         0: check 
         1: getRewardSuccess.
         */
        [ProtoMember(101)]
        public int status { get; set; }

        [ProtoMember(102)]
        public int UserID { get; set; }

        [ProtoMember(103)]
        public List<int> Result { get; set; }

        [ProtoMember(104)]
        public string des { get; set; }

        [ProtoMember(105)]
        public byte errorCode { get; set; }

        [ProtoMember(106)]
        public byte actionID { get; set; }

        [ProtoMember(107)]
        public string extInfo { get; set; }
    }
}
