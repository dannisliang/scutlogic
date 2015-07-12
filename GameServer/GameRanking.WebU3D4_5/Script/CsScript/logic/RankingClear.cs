using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Common.Timing;
using GameServer.Model;
using ZyGames.Framework.Data;
using GameRanking.Pack;
using ZyGames.Framework.Cache.Generic;
using Game.Script;
using ZyGames.Framework.Redis;
using Game.NSNS;

namespace Game.Script
{
    class RankingClear
    {
        static RankingClear instance = null;
        public static RankingClear Instance()
        {
            if (null == instance)
            {
                instance = new RankingClear();
                instance.init();
            }
            return instance;
        }

        public void Start()
        { 
        }

        void init()
        {
            bool openDebug = GameConfigMgr.Instance().getInt("timer_open_debug", 0) == 1;
            if(openDebug)
            {
                string debugParm = GameConfigMgr.Instance().getString("timer_user_ranking_clear", "330000,330000");
                string[] words = debugParm.Split(',');
                TimerMgr.Singleton().add("RankingClear", ccccc, int.Parse(words[0]), int.Parse(words[1]));
            }
            else
            {
                string theStr = GameConfigMgr.Instance().getString("timer_user_ranking_clear_timming", "05:00");
                //每天0点执行
                TimeListener.Append(PlanConfig.EveryDayPlan(DoEveryWeekExecute, "EveryDayTask", theStr));
                ConsoleLog.showNotifyInfo("timer_user_ranking_clear_timming begin:" + theStr);
            }
        }

        void ccccc(object obj)
        {
            doIt();
        }

        public  void doIt()
        {
            ConsoleLog.showNotifyInfo("RankingClear Begin:");
            RankingFactorNew.Singleton().Refresh<UserRanking>(typeof(RankingScore).ToString());
            RankingFactorNew.Singleton().Loop<UserRanking>(typeof(RankingScore).ToString(), addRankingReward);

            // get fake data.
            // getFake();

            // memoryData,clear lst and unload sharecache
            RankingFactorNew.Singleton().Clear<UserRanking>(typeof(RankingScore).ToString());

            // redis delete
            List<int> keysremove = new List<int>();
            var urcache = new ShareCacheStruct<UserRanking>();
            float percent = GameConfigMgr.Instance().getInt("rankclear_perscent", 1) / 100.0f;
            int reduceScore = GameConfigMgr.Instance().getInt("rank_score_redice", 10000);
            int reduceAfterScorre = GameConfigMgr.Instance().getInt("rank_clear_after", 500);
            urcache.Foreach((string str, UserRanking ur) =>
            {
                if(ur.Score > reduceScore)
                {
                    ur.ModifyLocked(() =>
                    {
                        ur.Score = reduceAfterScorre;
                    });
                }
                return true;
            });

         //  ZyGames.Framework.Redis.RedisConnectionPool.Process(client =>
         //  {
         //      string delKey = "$" + typeof(UserRanking).ToString();
         //      for(int i=0; i<keysremove.Count; ++i)
         //      {
         //          byte[] byteKeys = Encoding.UTF8.GetBytes(keysremove[i].ToString());
         //           client.HDel(delKey, byteKeys);
         //      }
         //  });

            // mysql delete
         //  var dbProvider = DbConnectionProvider.CreateDbProvider("ConnData");
         //  var command = dbProvider.CreateCommandStruct("UserRanking", CommandMode.Delete);
         //  command.Parser();
         //  dbProvider.ExecuteQuery(System.Data.CommandType.Text, command.Sql, command.Parameters);

            // update UserRankingTotal
            RankingFactorNew.Singleton().Refresh<UserRankingTotal>(typeof(RankingTotal).ToString());

            // add fake data mybe this is has not data ....
           // setFake();

            RankingFactorNew.Singleton().Refresh<UserRanking>(typeof(RankingScore).ToString());
            ConsoleLog.showNotifyInfo("RankingClear End");
        }

        List<UserRanking> fakeLst; 
        void getFake()
        {
            ConsoleLog.showNotifyInfo("get fake begin");
            fakeLst = new List<UserRanking>();
            int num = GameConfigMgr.Instance().getInt("rank_send_num", 10);
            float percent = GameConfigMgr.Instance().getInt("rankclear_perscent", 1) / 100.0f;
            int reduceScore = GameConfigMgr.Instance().getInt("rank_score_redice", 1);
            List<UserRanking> urlst = RankingFactorNew.Singleton().get<UserRanking>(typeof(RankingScore).ToString());
            if(null == urlst)
            {
                TraceLog.WriteError("getFake()");
                ConsoleLog.showErrorInfo(0,"getFake()");
                return;
            }
            var rankCache = new ShareCacheStruct<UserRanking>();
            var userCache = new PersonalCacheStruct<GameUser>();
            for (int i = 0; i < urlst.Count; ++i)
            {
                if (i > num) break; 
                UserRanking ur = urlst[i];
                if (ur.Score > reduceScore)
                {
                    ur.Score = (int)((float)urlst[i].Score * percent);
                    UserRanking cacheUR = rankCache.FindKey(ur.UserID);
                    if(null != cacheUR) cacheUR.ModifyLocked(()=> {cacheUR.Score=ur.Score;});
                    GameUser cacheGU = userCache.FindKey(ur.UserID.ToString());
                    if (null != cacheGU) cacheGU.ModifyLocked(() => { cacheGU.Score = ur.Score; });
                }
                fakeLst.Add(copy(ur));
            }
            ConsoleLog.showNotifyInfo("getFake:" + fakeLst.Count);
            ConsoleLog.showNotifyInfo("get fake end");
        }

        UserRanking copy(UserRanking ur)
        {
            UserRanking newUr = new UserRanking();
            newUr.UserID    = ur.UserID;
            newUr.Score     = ur.Score;
            return newUr;
        }
        void setFake()
        {
            // add fake UserRanking Data at 
            ConsoleLog.showNotifyInfo("setFake:" + fakeLst.Count);
            var cache = new ShareCacheStruct<UserRanking>();
            for(int i=0; i<fakeLst.Count; ++i)
            {
                cache.Add(copy(fakeLst[i]));
            }
            fakeLst.Clear();
            fakeLst = null;
            RankingFactorNew.Singleton().Refresh<UserRanking>(typeof(RankingScore).ToString());
        }

        private static void DoEveryWeekExecute(PlanConfig planconfig)
        {
            RankingClear.Instance().doIt();
        }

        static int getScore(int index)
        {
            GameConfigRankingReward.rewardData rd = GameConfigMgr.Instance().getRankReward(index);
            if(rd!=null)
            {
                return rd.Score;
            }
            return 0;
        }

        static int getDiamond(int index)
        {
            GameConfigRankingReward.rewardData rd = GameConfigMgr.Instance().getRankReward(index);
            if (rd != null)
            {
                return rd.Diamonds;
            }
            return 0;
        }

        static bool addRankingReward(List<UserRanking> rankingList)
        {
            if(null == rankingList)
            {
                ConsoleLog.showErrorInfo(135, "addRankingReward:Error");
                TraceLog.WriteError("Error : addRankingReward");
                return false;
            }
            int scoreNum = GameConfigMgr.Instance().getInt("rankclear_scoreNum",20);
            int DemoNum = GameConfigMgr.Instance().getInt("rankclear_dimondNum", 10000);

            int max = Math.Max(scoreNum, DemoNum);
            // get Score accounding to Ranking
            var totalCache = new ShareCacheStruct<UserRankingTotal>();
            var personCache = new PersonalCacheStruct<GameUser>();
            for (int i = 0; i < rankingList.Count; ++i )
            {
                if (i < DemoNum)
                {
                    UserRanking ur = rankingList[i];
                    int score = getScore(i);
                    UserRankingTotal urt = totalCache.FindKey(ur.UserID);
                    if (null == urt)
                    {
                        UserRankingTotal newUrt = new UserRankingTotal();
                        newUrt.UserID = ur.UserID;
                        newUrt.Total = score;
                        totalCache.Add(newUrt);
                    }
                    else
                    {
                        urt.ModifyLocked(() =>
                        {
                            urt.Total += score;
                        });
                    }
                }
                int UserId = rankingList[i].UserID;
                GameUser gu = personCache.FindKey(UserId.ToString());
                if(i<DemoNum)
                {
                    if(null != gu)
                    {
                        gu.ModifyLocked(() =>
                            {
                                gu.Diamond  += getDiamond(i);
                                gu.theTotal += getScore(i);
                            });

                    }
                }

                if (null != gu)
                {
                    gu.ModifyLocked(() =>
                    {
                        gu.preRanking = i+1;
                    });
                }
            }

            return true;
        }
        static Response1001Pack cbFunc(object obj)
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
    }
}
