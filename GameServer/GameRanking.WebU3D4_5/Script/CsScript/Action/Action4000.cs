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
using System.IO;
using System.Text;
using System.Web;

namespace GameServer.CsScript.Action
{

    /*
     * Class:   Action4000
     * Desc:   	web back office
     * Author： guccang	
     * Date：	2015-8-11:11:11
     */
    /// <summary>
    /// Action4000 Document
    /// </summary>
    
    public class Action4000 : BaseAction
    {
        private string urlParams;
        private string resultSTR;
    
        public Action4000(ActionGetter actionGetter)
            : base(4000, actionGetter)
        {
            urlParams = "";
        }

        public override bool GetUrlElement()
        {
            urlParams = (string)actionGetter.GetMessage();
            Game.NSNS.ConsoleLog.showErrorInfo(0, "Action4000 Process:" + urlParams); 
            return true;
        }

        string processActivity(string parm)
        {
            return "processActivity OK :"+parm;
        }
        string processOpenClose(string parm)
        {
            return "processOpenCLose OK :"+parm;
        }

        string processBlackInfo(string parm)
        {
            string res = "";
            try 
            {
                List<UserRanking> lst = RankingFactorNew.Singleton().get<UserRanking>(typeof(RankingScore).ToString());
                if (lst != null && lst.Count > 0)
                {
                    int cnt = int.Parse(parm);
                    for (int i = 0; i < lst.Count; ++i)
                    {
                        if (i >= cnt) break;
                        res += lst[i].UserName + "###" + lst[i].Score;
                        if (i + 1 < cnt) res += ",";
                    }
                }
                else
                {
                    res = "排行榜正在更新，或排行榜为空";
                }
            }
            catch(System.Exception e)
            {
                res = e.Message;
            }
            return res;
        }

        string processBlack(string parm)
        {
            string res = delUserRanking(parm);
            return "反馈数据:要删除的玩家排名{" + parm + "}#" + "#执行结果:{" + res + "}"; 
        }

      
        public override bool TakeAction()
        {
            string []parms = urlParams.Split(':');
            if(2!=parms.Length)
            {
                resultSTR = "Error Parms: cmd:parm";
                return true;
            }
            string cmd = parms[0];
            string parm = parms[1];
            if("openClose"==cmd)
            {
               resultSTR = processOpenClose(parm);
            }
            else if ("activity"==cmd)
            {
                resultSTR = processActivity(parm);
            }
            else if("black"==cmd)
            {
                resultSTR = processBlack(parm);
            }
            else if("blackInfo"==cmd)
            {
                resultSTR = processBlackInfo(parm);
            }
            else if("doFrom"==cmd)
            {
                resultSTR = processDoFrom(parm);
            }
            else
            {
                resultSTR = "你好GM not find cmd:"+cmd;
            }

            return true;
        }
        protected override byte[] BuildResponsePack()
        {
            return System.Text.Encoding.UTF8.GetBytes(resultSTR);
        }
        BlackListData UR2BLD(UserRanking ur)
        {
            BlackListData bd = new BlackListData();
            bd.UserID = ur.UserID;
            bd.UserName = ur.UserName;
            bd.Score = ur.Score;
            return bd;
        }
        void doAdd_black(string parm)
        {
            string[] usridStr = parm.Split(',');
            for (int i = 0; i < usridStr.Length; ++i)
            {
                try
                {
                    int index = int.Parse(usridStr[i]);
                    var cache = new ShareCacheStruct<UserRanking>();
                    List<UserRanking> lst = RankingFactorNew.Singleton().get<UserRanking>(typeof(RankingScore).ToString());
                    UserRanking ur = null;
                    if(lst!=null && lst.Count>index)
                    {
                        ur = lst[index]; 
                    }
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
        string delUserRanking(string parm)
        {
            doAdd_black(parm);
            List<UserRanking> lst = RankingFactorNew.Singleton().get<UserRanking>(typeof(RankingScore).ToString());
            List<UserRanking> delList = new List<UserRanking>(); 
            if(null != lst && lst.Count!=0)
            {
                string []ids = parm.Split(',');
                for(int i=0; i<ids.Length; ++i)
                {
                    int id = int.Parse(ids[i]) - 1;
                    if (id >= lst.Count) continue;

                    delList.Add(lst[id]);
                }
            }
            else
            {
                return "排行榜正在更新或为空";
            }
            if(delList.Count>0)
            {
                var cache = new ShareCacheStruct<UserRanking>();
                resultSTR = "删除的玩家数据:{";
                foreach (var v in delList)
                {
                    cache.Delete(v);
                    resultSTR += v.UserName + ":" + v.Score + ",";
                }
                resultSTR += "}";
                    
                RankingFactorNew.Singleton().Refresh<UserRanking>(typeof(RankingScore).ToString());
            }
            else
            {
                resultSTR = "要删除的数据不存在";
            }
            return resultSTR;
        }


        static void doFrom_Model_person<T>(object parm, string key = "UserId") where T : BaseEntity, new()
        {
            ZyGames.Framework.Model.SchemaTable schema = ZyGames.Framework.Model.EntitySchemaSet.Get<T>();
            string typeName = typeof(T).ToString();
            int max = int.Parse(parm as string);
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From Start:" + max);
            int Step = 1000;
            var cache = new PersonalCacheStruct<T>();
            for (int i = 0; i < max; i += Step)
            {
                var filter = new ZyGames.Framework.Net.DbDataFilter(0);
                filter.Condition = "where " + key + " >=@Key1 and " + key + " <@Key2";
                filter.Parameters.Add("Key1", i);
                filter.Parameters.Add("Key2", i + Step);
                cache.TryRecoverFromDb(filter);
                ConsoleLog.showNotifyInfo(typeName + ":" + i + " load");
            }
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From End");
        }

        static void doFrom_Model_share<T>(object parm, string key = "UserID") where T : ShareEntity, new()
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
                ConsoleLog.showNotifyInfo(typeName + ":" + i + " load");
            }
            ConsoleLog.showNotifyInfo("########" + typeName + "######## From End");
        }
        public void thread_DoFrom(object parms)
        {
            string parm = parms as string;
            string[] pp = parm.Split(',');
            string t = pp[0];
            string num = pp[1];

            if (t == "UserRankingTotal")
            {
                doFrom_Model_share<UserRankingTotal>(num as object,"UserID");
            }
            else if("UserRanking"==t)
            {
                doFrom_Model_share<UserRanking>(num as object, "UserID");
            }
            else if("HappyModeData" == t)
            {
                doFrom_Model_person<HappyModeData>(num as object, "the3rdUserId");
            }
            else if("GameUser"==t)
            {
                doFrom_Model_person<GameUser>(num as object, "UserId");
            }
        }

        string processDoFrom(string parm)
        {
            System.Threading.Thread thread = new System.Threading.Thread(thread_DoFrom);
            thread.Start(parm);
            return "反馈数据:执行成功";
        }
    }

}