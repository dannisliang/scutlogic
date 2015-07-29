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
using ZyGames.Framework.Model;

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

       bool authoriyCheck()
       {
           var cache = new ShareCacheStruct<Authority>();
           var auth = cache.Find((o)=>{
                   return  (o.name==requestPack.name && o.pwd == requestPack.pwd);
           });
     
           if(null == auth)
               return false;
     
           return 0 != (auth.level&getCmdLevel(requestPack.param));
       }

        int getCmdLevel(string cmd)
        {
            return 0xfffffff;
        }

        public override bool TakeAction()
        {
            if(false == authoriyCheck())
            {
                ConsoleLog.showErrorInfo(0,"authoriy not enought");
            }

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
            RankingFactorNew.Singleton().Refresh<UserRanking>(typeof(RankingScore).ToString());
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

        CacheDictionary<int,RefleshCacheInfo> copy(CacheDictionary<int,RefleshCacheInfo> cacheDIC)
        {
            return null;
        }

        static T clone<T>(T t) where T : new()
        {
            string json = JsonHelper.GetJson<T>(t);
            return (T)JsonHelper.ParseFromJson<T>(json);
        }

        static HappyModeData copyHMD(HappyModeData hmd)
        {
            var cache = new PersonalCacheStruct<HappyModeData>();
            HappyModeData d = new HappyModeData();

            d.the3rdUserId   = (int)cache.GetNextNo();
            d.EnterNum       = hmd.EnterNum;
            d.HappyPoint     = hmd.HappyPoint;
            d.HappyReliveNum = hmd.HappyReliveNum;
            d.PreRefleshTime = hmd.PreRefleshTime;

            d.realItemBuyCntInRefleshTime   = clone<CacheDictionary<int, RefleshCacheInfo>>(hmd.realItemBuyCntInRefleshTime);
            d.ActionEnterHappyPoint         = clone<CacheDictionary<int, UserActionInfo>>(hmd.ActionEnterHappyPoint);
            d.RealItemInfoLst               = clone<CacheList<persionRealItemInfo>>(hmd.RealItemInfoLst);
            d.PayInfoDic                    = clone<CacheDictionary<string, PayOrderPersion>>(hmd.PayInfoDic);
            return d;
        }

        static bool  addHappyDataMap(HappyModeData hmd,int newId)
        {
            var happMapCache = new PersonalCacheStruct<The3rdUserIDMap>();
            var map = happMapCache.FindKey("888");
            string type = "YYS_CP360";
            uint id = utils.KeyInt2Uint(hmd.the3rdUserId);
            int mapID = newId;
            if (hmd.the3rdUserId > 0)
                mapID = hmd.the3rdUserId;
            string mapKey = Action1005.getMapKey(type, id.ToString());
            if (map.the3rdMap.ContainsKey(mapKey))
            {
                return false;
            }
            map.ModifyLocked(() => {
                map.the3rdMap.Add(mapKey, mapID);
            });
            if (hmd.the3rdUserId > 0)
                return false;
            return true;
        }

        static void doFrom_Model_person<T>(object parm,string key="UserId") where T : BaseEntity, new()
        {
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<T>();
            string typeName = typeof(T).ToString();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" +  typeName + "######## From Start:" + max);
            int Step = 1000;
            var cache = new PersonalCacheStruct<T>();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where "+key+" >=@Key1 and "+ key +" <@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo(typeName+":" + i + " load");
            }
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From End");
        }

        static void doFrom_Model_share<T>(object parm,string key="UserID") where T : ShareEntity , new()
        {
            string typeName = typeof(T).ToString();
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<T>();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From Start:" + max);
            int Step = 1000;
            var cache = new ShareCacheStruct<T>();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where " + key + " >=@Key1 and " + key + " <@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo(typeName+ ":" + i + " load");
            }
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From End");
        }
        static void doFrom_UserRankingTotal(object parm)
        {
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<UserRankingTotal>();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" + typeof(UserRankingTotal).ToString() + "######## From Start:" + max);
            int Step = 1000;
            var cache = new ShareCacheStruct<UserRankingTotal>();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where UserID>=@Key1 and UserID<@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo("UserRanking:    " + i + "     load");
            }
            ConsoleLog.showNotifyInfo("########" + typeof(UserRankingTotal).ToString() + "######## From End");
        }
        static void doFrom_UserRanking(object parm)
        {
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<UserRanking>();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" + typeof(UserRanking).ToString() + "######## From Start:" + max);
            int Step = 1000;
            var cache = new ShareCacheStruct<UserRanking>();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where UserID>=@Key1 and UserID<@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo("UserRanking:    " + i + "     load");
            }
            ConsoleLog.showNotifyInfo("########" + typeof(UserRanking).ToString() + "######## From End");
        }
        static void doFrom_GameUser(object parm)
        {
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<GameUser>();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" + typeof(GameUser).ToString() + "######## From Start:" + max);
            int Step = 1000;
            var cache = new PersonalCacheStruct<GameUser>();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where UserId>=@Key1 and UserId<@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo("gameUser:    "+ i + "     load");
            }
            ConsoleLog.showNotifyInfo("########" + typeof(GameUser).ToString() + "######## From End");
        }

        static void th()
        {
            //doFrom_Model_person<HappyModeData>("200000", "the3rdUserId");
            //doFrom_Model_person<The3rdUserIDMap>("20000", "Index");
           
        }

        void test()
        {
            HappyModeData hmd = new HappyModeData();
            PayOrderPersion pop = new PayOrderPersion();
            pop.Index = 1;
            pop.UserId = 2;
            pop.Identify = "1";
            pop.typeUser = "1"; // 360Pay..maybe
            pop.ProductId = "1";
            pop.num = 1;
            pop.the3rdUsrID = 1;// utils.KeyUInt2Int(requestPack.the3rdUserId);
            pop.strThe3rdOrderId = "1";
            pop.ServerOrderId = "1";
            pop.the3rdOrderId = "1";
            hmd.PayInfoDic.Add("1", pop);
            HappyModeData d = new HappyModeData();

            persionRealItemInfo prii = new persionRealItemInfo();
            prii.Index = 1;
            hmd.RealItemInfoLst.Add(prii);

            UserActionInfo uai = new UserActionInfo();
            uai.index = 1;
            hmd.ActionEnterHappyPoint.Add(1, uai);

            RefleshCacheInfo rci = new RefleshCacheInfo();
            rci.itemId = 1;
            hmd.realItemBuyCntInRefleshTime.Add(1, rci);


            d.realItemBuyCntInRefleshTime = clone<CacheDictionary<int, RefleshCacheInfo>>(hmd.realItemBuyCntInRefleshTime);
            d.ActionEnterHappyPoint = clone<CacheDictionary<int, UserActionInfo>>(hmd.ActionEnterHappyPoint);
            d.RealItemInfoLst = clone<CacheList<persionRealItemInfo>>(hmd.RealItemInfoLst);
            d.PayInfoDic = clone<CacheDictionary<string, PayOrderPersion>>(hmd.PayInfoDic);
        }
        void doAdd_HappyDataFormat(string parm)
        {
            System.Threading.Thread thread = new System.Threading.Thread(th);
            thread.Start();
        }

        static void checkMapRepeat()
        {
        }

        static void checkMap()
        {
            var cache = new PersonalCacheStruct<The3rdUserIDMap>();
            var theData = cache.FindKey("888");
            if (theData == null) return;

            var happyDataCache = new PersonalCacheStruct<HappyModeData>();

            List<string> delKey = new List<string>();
            theData.the3rdMap.Foreach((string key,int val) => {
                if (happyDataCache.FindKey(val.ToString()) == null)
                {
                    delKey.Add(key);
                }
                return true;
            });

            foreach(var k in delKey)
            {
                if(theData.the3rdMap.ContainsKey(k))
                {
                    theData.the3rdMap.Remove(k);
                    ConsoleLog.showErrorInfo(0,"map del:"+k);
                }
            }

            var payCache = new PersonalCacheStruct<PayUserInfoEx>();
            payCache.LoadFrom(null);

            List<int> delPUILst = new List<int>();
            payCache.Foreach((string s1,string s2,PayUserInfoEx pui)=> {
                delPUILst.Add(pui.UserId);
                return true;
            });

            foreach(var v in delPUILst)
            {
                var pui = payCache.FindKey(v.ToString());
                if(null != pui)
                    payCache.Delete(pui);
                ConsoleLog.showErrorInfo(0,"del pui:"+v);
            }
        }
        void doAdd_checkMap()
        {
            System.Threading.Thread thread = new System.Threading.Thread(checkMap);
            thread.Start();
        }

        void doAdd_HMD(string parm)
        {
            var happyCache = new PersonalCacheStruct<HappyModeData>();
            var hmd = new HappyModeData();
            hmd.the3rdUserId = int.Parse(parm);
            int maxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum", 3);
            hmd.EnterNum = maxEnterNum;
            happyCache.Add(hmd);
        }
        void doAdd_RemoveMap(string parm)
        {
            string[] strs = parm.Split(',');
            string opt      = strs[0];
            string type     = strs[1];
            string the3rdID = strs[2];
            string key = Action1005.getMapKey(type, the3rdID);

            var cache = new PersonalCacheStruct<The3rdUserIDMap>();
            var Da = cache.FindKey("888");
            
            if(opt=="del")
            {
                if(Da.the3rdMap.ContainsKey(key))
                {
                    Da.the3rdMap.Remove(key);
                    ConsoleLog.showErrorInfo(0, "del map key:" + key);
                }
                else
                {
                    ConsoleLog.showErrorInfo(0, "del not find map key:" + key);
                }
            }
            else if(opt=="add")
            {
                if (Da.the3rdMap.ContainsKey(key))
                {
                    ConsoleLog.showErrorInfo(0, "add map key had find:" + key);
                }
                else
                {
                    int happyID = int.Parse(strs[3]);
                    Da.the3rdMap.Add(key, happyID);
                    ConsoleLog.showErrorInfo(0, "add map key add :" + key);
                }
            }
            else if(opt=="addNew")
            {
                if (Da.the3rdMap.ContainsKey(key))
                {
                    ConsoleLog.showErrorInfo(0, "addNew map key had find:" + key);
                }
                else
                {
                    int happyID = Action1005.getHappyIndex(type, the3rdID);
                    Da.the3rdMap.Add(key, happyID);
                    ConsoleLog.showErrorInfo(0, "addNew map key add :" + key+":"+the3rdID);
                }
            }
           
        }

        void doAdd_HappyDataAddItem(string parm)
        {
            string[] stringS = parm.Split(',');
            int UserID = int.Parse(stringS[0]);
            int the3rdUserID = int.Parse(stringS[1]);
            int itemId = int.Parse(stringS[2]);

            var cache = new PersonalCacheStruct<HappyModeData>();
            var hmd = cache.FindKey(stringS[1]);
            if (null == hmd) return;

            persionRealItemInfo rii = new persionRealItemInfo();
            rii.Index = hmd.RealItemInfoLst.Count;
            rii.UserId = UserID;
            rii.the3rdUserId = the3rdUserID;
            rii.Identify = "GM_ADD";
            rii.happyPoint = hmd.HappyPoint;
            rii.needHappyPoint = 0;
            rii.realItemID = itemId;
            hmd.RealItemInfoLst.Add(rii);

            // save to db for ....
            var shareRealItemCache = new ShareCacheStruct<shareRealItemInfo>();
            shareRealItemInfo shareRII = new shareRealItemInfo();
            shareRII.Index = (int)shareRealItemCache.GetNextNo();
            shareRII.UserId = UserID;
            shareRII.the3rdUserId = the3rdUserID;
            shareRII.Identify = "GM_ADD";
            shareRII.happyPoint = hmd.HappyPoint;
            shareRII.needHappyPoint = 0;
            shareRII.realItemID = itemId;
            shareRealItemCache.Add(shareRII);
        }
        void doAdd_HappyDataMap(string parm)
        {
            doAdd_checkMap();
            int id = int.Parse(parm);

            var cache = new PersonalCacheStruct<The3rdUserIDMap>();
            var theData = cache.FindKey("888");
            if (theData == null) return;

            foreach(var k in theData.the3rdMap.Keys)
            {
                if(theData.the3rdMap[k] == id)
                {
                    theData.the3rdMap.Remove(k);
                    ConsoleLog.showErrorInfo(0,"map del:"+k+":"+id);
                    break;
                }
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
                int id = Action1005.getHappyIndex(pui.typeUser, the3rdID.ToString());
                pui.the3rdUsrID = (uint)id;
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
            else if("happyDataFormat" == cmd)
            {
                doAdd_HappyDataFormat(parm);
            }
            else if ("happyDataMap" == cmd)
            {
                doAdd_HappyDataMap(parm);
            }
            else if ("happyDataAddItem" == cmd)
            {
                doAdd_HappyDataAddItem(parm);
            }
            else if("optMapKey"==cmd)
            {
                doAdd_RemoveMap(parm);
            }
            else if("addHMD"==cmd)
            {
                doAdd_HMD(parm);
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