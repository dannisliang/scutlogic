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
            else if("updateConfig"==cmd)
            {
                resultSTR = processUpdateConfig(parm);
            }
            else if("userInfo"==cmd)
            {
                resultSTR = processUserInfo(parm);
            }
            else if("delete"==cmd)
            {
                resultSTR = processDelete(parm);
            }
            else if("add"==cmd)
            {
                resultSTR = processAdd(parm);
            }
            else if("modify"==cmd)
            {
                resultSTR = processModify(parm);
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


       
        public void thread_DoFrom(object parms)
        {
            string parm = parms as string;
            string[] pp = parm.Split(',');
            string t = pp[0];
            string num = pp[1];

            if (t == "UserRankingTotal")
            {
                utils.doFrom_Model_share<UserRankingTotal>(num as object,"UserID");
            }
            else if("UserRanking"==t)
            {
                utils.doFrom_Model_share<UserRanking>(num as object, "UserID");
            }
            else if("HappyModeData" == t)
            {
                utils.doFrom_Model_person<HappyModeData>(num as object, "the3rdUserId");
            }
            else if("GameUser"==t)
            {
                utils.doFrom_Model_person<GameUser>(num as object, "UserId");
            }
        }

        string processDoFrom(string parm)
        {
            System.Threading.Thread thread = new System.Threading.Thread(thread_DoFrom);
            thread.Start(parm);
            return "反馈数据:执行成功";
        }

        string processAdd(string parm)
        {
            string info = "";
            string[] p = parm.Split(',');
            string addWhich = p[0];
            string name = p[1];
            int score = int.Parse(p[2]) ;
            if("ranking"==addWhich)
            {
                var cache = new PersonalCacheStruct<GameUser>();
                var urCache =new ShareCacheStruct<UserRanking>();
                
                var gu = new GameUser();
                gu.UserId = (int)cache.GetNextNo();
                gu.NickName = name;
                gu.Score = score;
                gu.Identify = "identify_"+name;
                var ur = new UserRanking();
                ur.UserID = gu.UserId;
                ur.UserName = name;
                ur.Score = score;

                cache.Add(gu);
                urCache.Add(ur);
                info = "增加排行榜数据成功";
            }
            return info;
        }
        string processModify(string parm)
        {
            string info = "";
            string[] p = info.Split(',');
            string modifyWhich= p[0];
            int index = int.Parse(p[1]);
            int score = int.Parse(p[2]);
            
            if(""==modifyWhich)
            {
                UserRanking ur = RankingFactorNew.Singleton().getRankingData<UserRanking, RankingScore>(index);
                if(null==ur)
                {
                    info = "要修改的数据不存在";
                }
                else
                {
                    var cache = new ShareCacheStruct<UserRanking>();
                    UserRanking theUR = cache.FindKey(ur.UserID);
                    theUR.ModifyLocked(() => {
                        ur.Score = score;
                     });

                    info = "修改数据成功";
                }
            }
            return info;
        }

        void thread_processSort(object theParms)
        {
            string parm = theParms as string;
            string[] p = parm.Split(',');
            string sortWhich = p[0];
            if("ranking"==sortWhich)
            {
                RankingFactorNew.Singleton().Refresh<UserRanking>(typeof(RankingScore).ToString());
            }
            else if("rankingtotal"==sortWhich)
            {
                RankingFactorNew.Singleton().Refresh<UserRankingTotal>(typeof(RankingTotal).ToString());
            }
        }
        string processSort(string parm)
        {
            string info = "";
            System.Threading.Thread thread = new System.Threading.Thread(thread_processSort);
            thread.Start(parm);
            info = "排序结束";
            return info;
        }
        string processDelete(string parm)
        {
            string info = "";
            string[] p = parm.Split(',');
            string deleteWhitch = p[0];
            int deleteIndex = int.Parse(p[1]);

            if("ranking"==deleteWhitch)
            {
                  UserRanking ur =   RankingFactorNew.Singleton().getRankingData<UserRanking, RankingScore>(deleteIndex);
                  if(ur==null)
                  {
                      info = "没有找到要删除的数据";
                  }
                  else
                  {
                      var cache = new ShareCacheStruct<UserRanking>();
                      cache.Delete(ur);
                      info = "删除数据成功";
                  }
            }
            return info;
        }
        string processUserInfo(string parm)
        {
            string[] p = parm.Split(',');
            string subcmd = p[0];
            string userid = p[1];

            var gu_cache = new PersonalCacheStruct<GameUser>();
            var ur_cache = new ShareCacheStruct<UserRanking>();
            var urt_cache = new ShareCacheStruct<UserRankingTotal>();

            var gu = gu_cache.FindKey(userid);
            var ur = ur_cache.FindKey(userid);
            var urt = urt_cache.FindKey(userid);

            string info = "";
            string name = "";
            
            if(gu!=null)
            {
                
                if(subcmd=="get")
                {
                    info += "#{id:"+gu.UserId+",name:"+gu.NickName+",idf:"+gu.Identify+"}" ;
                }
                else if(subcmd=="set")
                {
                    gu.ModifyLocked(() => {
                        gu.NickName = p[2];
                    });
                }
            }
            else
            {
                info += "未找到GameUser数据";
            }

            info += "\n";

            if(null != ur)
            {
                if(subcmd=="get")
                {

                    info += "id:" + ur.UserID+ ",name:" + ur.UserName + ",Score:"+ur.Score+"}";
                }
                else if(subcmd=="set")
                {
                    ur.ModifyLocked(() => {
                        ur.Score = int.Parse(p[3]);
                        ur.UserName = p[2]; 
                    });
                }
            }
            else
            {
                info += "未找到UserRanking数据";
            }

            info += "\n";

            if(null != urt)
            {
                if(subcmd=="get")
                {
                    info += "#{id:" + urt.UserID + ",total:" + urt.Total;
                }
                else if(subcmd=="set")
                {
                    urt.ModifyLocked(() => {

                        urt.Total = int.Parse(p[4]);
                    
                    });

                }
            }
            else
            {
                info += "未找到积分数据";
            }

            return info;
        }
        string processUpdateConfig(string parm)
        {
            string[] pp = parm.Split(',');
            string cmd = pp[0];
            string p = pp[1];

            string result = "";
            bool ret = NewGameConfig.Singleton().restore(cmd, p);
            if (ret)
            {
                result = parm + ":" + "执行成功";
            }
            else
            {
                result = "未找到更新表：" + parm;
            }
            return "反馈数据:" + result;
        }
    }

}