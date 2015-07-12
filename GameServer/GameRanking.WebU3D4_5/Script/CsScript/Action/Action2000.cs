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

using GameRanking.Pack;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using Game.Script;
using ZyGames.Framework.Common.Timing;
using ZyGames.Framework.Data;
using System.Collections.Generic;
using ZyGames.Framework.Common.Log;
using Game.NSNS;

namespace GameServer.CsScript.Action
{
    public class Action2000 : BaseAction
    {
        private ResponsePack responsePack;
        private Request2000Pack requestPack;

        public Action2000(ActionGetter actionGetter)
            : base(1000, actionGetter)
        {
            responsePack = new ResponsePack();
        }


        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request2000Pack>(data);
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            Request2000Pack.E_ACTION_TYPE t = requestPack.theActionType;
            switch (t)
            {
                case Request2000Pack.E_ACTION_TYPE.E_ACTION_TYPE_DELETE:
                    {
                        doDelete();
                    }break;
                case Request2000Pack.E_ACTION_TYPE.E_ACTION_TYPE_ADD:
                    {
                        doAdd();
                    }break;
            }
            return true;
        }
        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }

        void doAdd_black(string parm)
        {
            string[] usridStr = parm.Split(',');
            for (int i = 0; i < usridStr.Length; ++i)
            {
                try
                {
                    int UserId = int.Parse(usridStr[i]);
                    var cache = new ShareCacheStruct<UserRanking>();
                    UserRanking ur = cache.FindKey(UserId);
                    var blackCache = new ShareCacheStruct<BlackListData>();
                    if (ur != null)
                    {
                        blackCache.Add(UR2BLD(ur));
                        ConsoleLog.showNotifyInfo("add to black list id:" + UserId);
                    }
                    else
                    {
                        ConsoleLog.showErrorInfo(0, "not find userRanking id:" + UserId);
                    }
                }
                catch (System.Exception e)
                {
                    ConsoleLog.showErrorInfo(0, "black list exception:" + e.Message);
                }
            }
        }
        void doAdd_delById(string parm)
        {
            string[] ids = parm.Split(',');
            for (int i = 0; i < ids.Length; ++i)
            {
                var urCache = new ShareCacheStruct<UserRanking>();
                int id = int.Parse(ids[i]);
                UserRanking ur = urCache.FindKey(id);
                if (ur != null) urCache.Delete(ur); // delete form cache and redis.
            }
        }

        bool addProductOnServer(PayOrder payData)
        {
            var persionHMDCache = new PersonalCacheStruct<HappyModeData>();
            HappyModeData hmd = persionHMDCache.FindKey(payData.the3rdUserId.ToString());
            if (hmd == null)
            {
                hmd = new HappyModeData();
                hmd.the3rdUserId = payData.the3rdUserId;
                persionHMDCache.Add(hmd);
            }
            string hd = GameConfigMgr.Instance().getProductInfo(payData.ProductId, payData.ServerOrderId);
            string[] infos = hd.Split('*');
            int itemID = int.Parse(infos[0]);
            int itemNum = int.Parse(infos[1]);
            string infoLog = string.Format("add item:{0}num:{1}", itemID, payData.num * itemNum);
            ConsoleLog.showNotifyInfo(infoLog);
            TraceLog.WriteInfo(infoLog);
            if (payData.ProductId == "5100")
            {
                hmd.HappyPoint += payData.num * itemNum;
                return true;
            }
            if (payData.ProductId == "5101")
            {
                hmd.HappyReliveNum += payData.num * itemNum;
                return true;
            }
            return false;
        }

        void doAdd_enterCnt(string parm)
        {
            ConsoleLog.showNotifyInfo("doAdd_enterCnt:"+parm);
            var cache = new PersonalCacheStruct<HappyModeData>();
            int maxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum",2);
            string[] ppp = parm.Split(',');
            HappyModeData hmd = cache.FindKey(ppp[0]);
            int happyPoint = int.Parse(ppp[1]);
            if(hmd != null)
            {
                hmd.ModifyLocked(() => {
                    hmd.EnterNum   += maxEnterNum;
                    hmd.HappyPoint += happyPoint;
                });
                ConsoleLog.showNotifyInfo("doAdd_enterCnt End " + parm + ":" + hmd.EnterNum);
            }
            else
            {
                ConsoleLog.showNotifyInfo("doAdd_enterCnt failed hmd is null" + parm );
            }
        }

        void doAdd_reCreateHappy(string parm)
        {
            var cache = new PersonalCacheStruct<HappyModeData>();
            cache.LoadFrom(null);

            var mapCache = new PersonalCacheStruct<The3rdUserIDMap>();
            var mapData = mapCache.FindKey("888");
            if(null == mapData)
            {
                mapData = new The3rdUserIDMap();
                mapData.Index = 888;
                mapCache.Add(mapData);
            }
          string mapKey = "";
          cache.Foreach((string a,string b,HappyModeData hmd)=>{
              mapKey = Action1005.getMapKey(parm, hmd.the3rdUserId.ToString());
              mapData.the3rdMap.Add(mapKey, hmd.the3rdUserId);
              return true;
          });
        }
        
        void doAdd_clearRanking(string parm)
        {
            RankingClear.Instance().doIt();
        }

        void doAdd_allServerCompensation(string parm)
        {
            string[] strS = parm.Split(',');
            CompensationInfo ci = new CompensationInfo();
            var cache = new ShareCacheStruct<CompensationInfo>();
            int index = (int)cache.GetNextNo();
            byte type = byte.Parse(strS[0]);
            string message = strS[1];
            string ItemInfo = strS[2]; // 5012*1:5023*4
            
            //check

            ci.index = index;
            ci.message = message;
            ci.ItemInfo = ItemInfo;
            ci.type = type;

            cache.Add(ci);
        }
        void doAdd_finishOrder(string parm)
        {
            string [] theParms = parm.Split(',');
            string orderId = theParms[0];
            string typeUser = theParms[1];
            var payCache = new ShareCacheStruct<PayOrder>();
            PayOrder payData = payCache.Find((o) =>
            {
                if (orderId == o.ServerOrderId)
                    return true;
                return false;
            });
            if(null == payData)
            {
                ConsoleLog.showErrorInfo(0, "error doAdd_finishOrder:"+orderId);
            }

            payData.ModifyLocked(() =>
            {
                payData.state    = PayOrder.PayStatus.paySuccess;
                payData.process  = true;
                payData.typeUser = typeUser;
                payData.hasGetPayReward = true;
            });

            addProductOnServer(payData);
        }

        uint getid(int id)
        {
            if (id < 0)
            {
                uint tmp = (uint)int.MaxValue;
                return (uint)(tmp - id);
            }
            else
                return (uint)id;
        }
        void doAdd_PayUserInfo(string parm)
        {
            string[] parmSS = parm.Split(',');
            uint the3rdID = uint.Parse(parmSS[0]);
            var cache = new PersonalCacheStruct<PayUserInfoEx>();
            PayUserInfoEx pui = cache.FindKey(the3rdID.ToString());
            if(null == pui)
            {
                pui = new PayUserInfoEx();
                pui.the3rdUsrID = the3rdID;
                pui.UserId      = int.Parse(parmSS[1]);
                pui.identify    = the3rdID.ToString();
                pui.the3rdUsrName = parmSS[2];
                pui.typeUser = parmSS[3];
                Action1005.returnJson rj = new Action1005.returnJson();
                rj.id = the3rdID.ToString();
                rj.name = pui.the3rdUsrName;
                pui.InfoExt = JsonHelper.GetJson<Action1005.returnJson>(rj);
                cache.Add(pui);
            }
        }

        void doAdd_order(string parm)
        {
            string[] strS = parm.Split(',');
            string ordderId = strS[0];
            int stat = int.Parse(strS[1]);
            var cache = new ShareCacheStruct<PayOrder>();
            PayOrder payData = cache.Find((o) =>
            {
                return o.ServerOrderId == ordderId;
            });

            if(payData==null)
            {
                ConsoleLog.showErrorInfo(0,"doAdd_order:"+ordderId);
                return;
            }

            payData.ModifyLocked(() => {
                ConsoleLog.showNotifyInfo(string.Format("payData state change {0}=>{1}",payData.state,stat));
                payData.state = (PayOrder.PayStatus)stat;
            });
        }
        void doAdd_del(string parm)
        {
            // save
        //    RankingLst.Instance().UpdateUserRanking(null);
        //    RankingLst.Instance().getData(cbFunc);
        //
        //    // memoryData
        //    RankingLst.Instance().UserRankingClear();
        //
        //    // redis
        //    ZyGames.Framework.Redis.RedisConnectionPool.Process(client =>
        //    {
        //        string delKey = "$" + typeof(UserRanking).ToString();
        //        client.Del(delKey);
        //    });
        //
        //    // mysql
        //    var dbProvider = DbConnectionProvider.CreateDbProvider("ConnData");
        //    var command = dbProvider.CreateCommandStruct("UserRanking", CommandMode.Delete);
        //    command.Parser();
        //    dbProvider.ExecuteQuery(System.Data.CommandType.Text, command.Sql, command.Parameters);
        }
        void doAdd()
        {
            var urCatch = new ShareCacheStruct<UserRanking>();
            string paramStr = requestPack.param;
            string[] arrStr = paramStr.Split('#');
            string cmd = arrStr[0];
            string parm = arrStr[1];
            if ("delById" == cmd)
            {
                doAdd_black(parm);
                doAdd_delById(parm);
            }
            else if ("from" == cmd)
            {
            }
            else if ("save" == cmd)
            {
            }
            else if ("trans" == cmd)
            {
            }
            else if ("update" == cmd)
            {
                RankingFactorNew.Singleton().Refresh<UserRankingTotal>(typeof(RankingTotal).ToString());
                RankingFactorNew.Singleton().Refresh<UserRanking>(typeof(RankingScore).ToString());
            }
            else if ("black" == cmd)
            {
                doAdd_black(parm);
            }
            else if ("del" == cmd)
            {
                doAdd_del(parm);
            }
            else if("order"==cmd)
            {
                doAdd_order(parm);
            }
            else if("3rdid"==cmd)
            {
                doAdd_PayUserInfo(parm);
            }
            else if("finishOrder"==cmd)
            {
                doAdd_finishOrder(parm);
            }
            else if("allServerCompensation"==cmd)
            {
                doAdd_allServerCompensation(parm);
            }
            else if("enterCnt"==cmd)
            {
                doAdd_enterCnt(parm);
            }
            else if("clearRanking" == cmd)
            {
                doAdd_clearRanking(parm);
            }
            else if("reCreateHappy"==cmd)
            {
                doAdd_reCreateHappy(parm);
            }
        }

        BlackListData UR2BLD(UserRanking ur)
        {
            BlackListData bd = new BlackListData();
            bd.UserID       = ur.UserID;
            bd.UserName     = ur.UserName;
            bd.Score        = ur.Score;
            return bd;
        }
        Response1001Pack cbFunc(object obj)
        {
            List<object> objList = obj as List<object>;
            if (objList.Count != 2) return null;
            List<UserRanking> rankingList = objList[1] as List<UserRanking>;

            // save to ....where....
            var shareCache = new ShareCacheStruct<HistoryUserRanking>();
            int num = (int)shareCache.GetNextNo();
            var saveData = new HistoryUserRanking();
            saveData.ID = num;
            // get data
            if (rankingList.Count >= 3)
            {
                UserRanking first = rankingList[0];
                UserRanking second = rankingList[1];
                UserRanking thrid = rankingList[2];

                saveData.Items.Add(first);
                saveData.Items.Add(second);
                saveData.Items.Add(thrid);
            }
            shareCache.Add(saveData);
         
            return null;
        }
        void doDelete()
        {
            var urCatch = new ShareCacheStruct<UserRanking>();
            string paramStr = requestPack.param;
            string[] arrStr = paramStr.Split(',');
            if(arrStr[0] == "byName")
            {
             //  var deleteLst = urCatch.FindAll(o => o.UserName.Contains(arrStr[1]));
             //  foreach (UserRanking ur in deleteLst)
             //  {
             //      urCatch.Delete(ur);
             //  }
            }
            else if(arrStr[0] == "byScore")
            {
                var deleteLst = urCatch.FindAll(o => o.Score>int.Parse(arrStr[1]));
                System.Console.WriteLine("deleteLst Count:{0}",deleteLst.Count);
                foreach (UserRanking ur in deleteLst)
                {
                    urCatch.Delete(ur);
                }
            }
        
        }

    }
}