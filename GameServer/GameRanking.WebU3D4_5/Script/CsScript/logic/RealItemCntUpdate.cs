using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Script;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using System.Threading; // Timer
using System.Diagnostics;
using ZyGames.Framework.Common;
using ZyGames.Framework.Common.Configuration;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Common.Timing;
using Game.NSNS;


namespace Game.Script
{
    class RealItemCntUpdate
    {

        static RealItemCntUpdate ins = null;
        public static RealItemCntUpdate Instance()
        {
            if (null == ins)
            {
                ins = new RealItemCntUpdate();
                ins.Init();
            }
            return ins;
        }

        public void Start(){}
        void Init()
        {
            var cache = new ShareCacheStruct<ShareRealItemCnt>();
            List<int> itemIds = GameConfigMgr.Instance().getHappyDataKeys();
            GameConfigHappyPoint.HappyData hd = null;
            for(int i=0;i<itemIds.Count; ++i)
            {
               if( null ==  cache.FindKey(itemIds[i])) // first add
               {
                   ShareRealItemCnt sric = new ShareRealItemCnt();
                   hd = GameConfigMgr.Instance().getHappyData(itemIds[i]);
                   sric.itemID  = hd.itemID;
                   sric.num     = hd.RefleshNum;
                   cache.Add(sric);
               }
            }
            
            string timestr = GameConfigMgr.Instance().getString("timer_item_cnt", "600000,600000");
            string[] tt = timestr.Split(',');
            int b0 = int.Parse(tt[0]);
            int b1 = int.Parse(tt[1]);
            TimerMgr.Singleton().add(typeof(RealItemCntUpdate).ToString(), actionrealItemCnt,b0,b1);

            bool openDebug = GameConfigMgr.Instance().getInt("timer_open_debug",0)==1;
            if(openDebug)
            {
                timestr = GameConfigMgr.Instance().getString("time_hdm_cnt", "600000,600000");
                tt = timestr.Split(',');
                 b0 = int.Parse(tt[0]);
                 b1 = int.Parse(tt[1]);
                TimerMgr.Singleton().add("HappyModeData_EnterNum", action_HappyModeData_enterNum, b0, b1);
            }
            else
            {
                //每天05点执行
                try
                {
                    timestr = GameConfigMgr.Instance().getString("time_hdm_cnt_timming", "05:15");
                }
                catch
                {
                    timestr = "05:15";
                }
                TimeListener.Append(PlanConfig.EveryDayPlan(DoEveryDayExecute, "EveryDayTask", timestr));
                ConsoleLog.showNotifyInfo("HappyModeData_EnterNum begin:" + timestr);
            }

        }

        public bool update_HappyModeData_enterNum(string s0,string s1,HappyModeData hmd)
        {
            int maxEnterNum = GameConfigMgr.Instance().getInt("happyPointMaxEnterNum", 3);
            hmd.ModifyLocked(() => {
                hmd.EnterNum = maxEnterNum;
            });
            return true;
        }

        public  static void DoEveryDayExecute(PlanConfig planconfig)
        {
            RealItemCntUpdate.Instance().action_HappyModeData_enterNum(null);
        }

        bool updateRealItemCnt(string s,ShareRealItemCnt sric)
        {
            GameConfigHappyPoint.HappyData hd = GameConfigMgr.Instance().getHappyData(sric.itemID);
            if(hd!=null)
            {
                int MinuteForReflesh = hd.MinuteForReflesh;
                DateTime now = DateTime.Now;
                TimeSpan ts = now - sric.preUpdateTime;
                if (ts.TotalMinutes > MinuteForReflesh)
                {
                    sric.ModifyLocked(() =>
                    {
                        sric.preUpdateTime = now;
                        sric.num = hd.RefleshNum;
                    });
                }
            }
            else
            {
                ConsoleLog.showErrorInfo(0,"RealItemCntUpdate error."+sric.itemID);
                TraceLog.WriteError("RealItemCntUpdate error." + sric.itemID);
            }
            
            // todo
            return true;
        }
        public void actionrealItemCnt(object state)
        {
            var cache = new ShareCacheStruct<ShareRealItemCnt>();
            cache.Foreach(updateRealItemCnt);
        }

        public void action_HappyModeData_enterNum(object state)
        {
            ConsoleLog.showNotifyInfo("action_HappyModeData_enterNum begin.");
            var cache = new PersonalCacheStruct<HappyModeData>();
            cache.Foreach(update_HappyModeData_enterNum);
            ConsoleLog.showNotifyInfo("action_HappyModeData_enterNum end.");
        }
    }
}
