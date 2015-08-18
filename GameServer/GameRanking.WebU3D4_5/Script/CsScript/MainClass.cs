using System;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Runtime;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Script;
using ZyGames.Framework.Common.Configuration;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;
using Game.NSNS;

namespace Game.Script
{

    public class MainClass : GameHttpHost
    {
        static int cnt = 0;
        static int sessionCnt = 0;
        public void mainCB(object state)
        {
            sessionCnt += GameSession.Count;
            if(cnt++>10)//10min....
            {
                ConsoleLog.showNotifyInfo("SessionCnt:" + sessionCnt);
                TraceLog.WriteWarn("SessionCnt:"+sessionCnt);
                cnt = 0;
                sessionCnt = 0;
            }
        }

        void addAuthory()
        {
            var cache = new ShareCacheStruct<Authority>();
            Authority au =  new Authority();
            au.id = (int)cache.GetNextNo();
            au.name = "guccang";
            au.pwd = ZyGames.Framework.Common.Security.CryptoHelper.MD5_Encrypt("Sally@123456");
            au.level = 0xfffffff;
            cache.Add(au);
        }

        void LoadAuthoFromDb()
        {
            var cache = new ShareCacheStruct<Authority>();
            cache.TryRecoverFromDb(new ZyGames.Framework.Net.DbDataFilter(0));
        }

        public MainClass()
        {
            addAuthory();
            GameEnvironment.Setting.ActionDispatcher = new CustomActionDispatcher();
            LoadAuthoFromDb();
            GameSession.Timeout = 60; // 60s
            int t = ConfigUtils.GetSetting("MainTime", 1000 * 10 * 60); // 1/50s
            TimerMgr.Singleton().add("Main", mainCB, 1000, t);
        }

        void happyMapInit()
        {
            var happMapCache = new PersonalCacheStruct<The3rdUserIDMap>();
            var map = happMapCache.FindKey("888");
            if (null == map)
            {
                map = new The3rdUserIDMap();
                map.Index = 888;
                happMapCache.Add(map);
            }
        }
        protected override void OnStartAffer()
        {
            new ShareCacheStruct<UserRanking>();
            new ShareCacheStruct<HistoryUserRanking>();
            new ShareCacheStruct<ExchangeCode>();
            new ShareCacheStruct<UserRankingTotal>();
            new ShareCacheStruct<ActivityModel>();
            happyMapInit();

            GameConfigMgr.Instance().Start();

            RankingFactorNew.Singleton().add<UserRanking>(typeof(RankingScore).ToString(), new RankingScore());
            RankingFactorNew.Singleton().add<UserRankingTotal>(typeof(RankingTotal).ToString(), new RankingTotal());
            bool openDebug = GameConfigMgr.Instance().getInt("timer_open_debug", 0) == 1;
            if(openDebug)
            {
                string str = GameConfigMgr.Instance().getString("timer_user_ranking", "10000,20000");
                RankingFactorNew.Singleton().Start<UserRanking>(typeof(RankingScore).ToString(), sortMethod.sortType.Interval, str);
                RankingFactorNew.Singleton().Start<UserRankingTotal>(typeof(RankingTotal).ToString(), sortMethod.sortType.Interval, str);
            }
            else
            {
                string strTiming = GameConfigMgr.Instance().getString("timer_user_ranking_timming", "00:00");
                RankingFactorNew.Singleton().Start<UserRanking>(typeof(RankingScore).ToString(), sortMethod.sortType.Timing, strTiming);
                RankingFactorNew.Singleton().Start<UserRankingTotal>(typeof(RankingTotal).ToString(), sortMethod.sortType.Timing, strTiming);
                ConsoleLog.showNotifyInfo("timer_user_ranking_timming begin:" + strTiming);
            }

            RankingClear.Instance().Start();
            RealItemCntUpdate.Instance().Start();
        }

        protected override void OnRequested(ActionGetter actionGetter, BaseGameResponse response)
        {
            base.OnRequested(actionGetter, response);
        }
        public override void Stop()
        {
            OnServiceStop();
            try
            {
                EntitySyncManger.Dispose();
            }
            catch
            {
            }
            base.Stop();
        }
    }
}