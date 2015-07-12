using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZyGames.Framework.Cache.Generic;
using GameRanking.Pack;
using ZyGames.Framework.Common;
using ZyGames.Framework.Model;
using ZyGames.Framework.Data;
using ZyGames.Framework.Common.Configuration;
using ZyGames.Framework.Common.Timing;
using ZyGames.Framework.Common.Log;
using Game.NSNS;
using GameServer.Model;
namespace Game.Script
{
    class RankingTotal : Ranking<UserRankingTotal>
    {
        public RankingTotal() :
            base()
        {
        }

        protected override void beforeDoRefresh()
        {
            base.beforeDoRefresh();
            ConsoleLog.showNotifyInfo("beforeDoRefresh UserRankingTotal cnt:" + _lst.Count);
        }

        protected override void afterDoRefresh()
        {
            base.afterDoRefresh();
            ConsoleLog.showNotifyInfo("afterDoRefresh UserRankingTotal cnt:" + _lst.Count);
        }
        protected override int comp(UserRankingTotal t1, UserRankingTotal t2)
        {
            int result = t2.Total - t1.Total;
            if (result == 0)
            {
                result = t2.UserID - t1.UserID;
            }
            return result;
        }
        protected override UserRankingTotal copyT(UserRankingTotal t)
        {
            UserRankingTotal ur = new UserRankingTotal();
            ur.UserID = t.UserID;
            ur.Total = t.Total;
            return ur;
        }
    }
}
