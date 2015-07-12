using System;
using ProtoBuf;


namespace GameRanking.Pack
{
    [ProtoContract]
    public class Request2000Pack
    {
        public enum E_ACTION_TYPE
        {
            E_ACTION_TYPE_DELETE,
            E_ACTION_TYPE_ADD
        }
        [ProtoMember(101)]
        public E_ACTION_TYPE theActionType { get; set; }

        [ProtoMember(102)]
        public string param     { get; set; }
    }
}
