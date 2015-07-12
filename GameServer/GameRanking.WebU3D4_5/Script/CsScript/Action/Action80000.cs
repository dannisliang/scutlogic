using GameRanking.Pack;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Common;
using ZyGames.Framework.Net;
using System;
using ZyGames.Framework.Common.Log;

namespace GameServer.CsScript.Action
{
    class Action80000 : BaseAction
    {

        RequestLog80000Pack requestPack;
        ResponsePack responsePack;

        public Action80000(ActionGetter actionGetter)
            : base(80000, actionGetter)
        { }

        public override bool GetUrlElement()
        {
            try
            {
                byte[] data = (byte[])actionGetter.GetMessage();
                if (data.Length > 0)
                {
                    requestPack = ProtoBufUtils.Deserialize<RequestLog80000Pack>(data);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("GetUrlElement:{0} error:{1}", actionId, ex);
                return false;
            }
        }

        public override bool TakeAction()
        {
            try
            {
                var UA = new UserAnalysis();
                foreach (logData d in requestPack.items)
                {
                    UA.DeviceId = d.DeviceID;
                    UA.Channel = d.Channel;
                    UA.SimType = d.SimType;
                    UA.ActionType = (UserAnalysis.E_ActionType)d.ActionType;
                    UA.ProductionId = d.ProductionId;
                    UA.ActionTime = System.DateTime.Now;
                    //DataSyncQueueManager.SendToDb(UA);
                    Console.WriteLine("{0}GameSession:{1}", DateTime.Now.ToString("HH:mm:ss"), GameSession.Count);
                }
                responsePack = new ResponsePack();
                responsePack.ActionId = 80000;
                responsePack.ErrorCode = 101;
                responsePack.ErrorInfo = "Success";
                return true;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("TakeAction:{0} error:{1}", actionId, ex);
                return false;
            }
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }
    }
}
