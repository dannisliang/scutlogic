using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

/*Log搜集*/
namespace GameRanking.Pack
{
    [ProtoContract]
    public class RequestLog80000Pack
    {
        public RequestLog80000Pack()
        {
            items = new List<logData>();
        }
        [ProtoMember(101)]
        public List<logData> items { get; set; }
    }

    [ProtoContract]
    public class logData
    {
         [ProtoMember(101)]
        public string DeviceID { get; set; }

        [ProtoMember(102)]
        public string Channel { get; set; }

        [ProtoMember(103)]
        public string SimType { get; set; }

        public enum E_ActionType
        {
            E_enterPay,
            E_PaySuccess,
            E_PayFailed,
        };
        [ProtoMember(104)]
        public E_ActionType ActionType { get; set; }

        [ProtoMember(105)]
        public string ProductionId   { get; set; }

        [ProtoMember(200)]
        public DateTime ActionTime { get; set; }
    }
}
