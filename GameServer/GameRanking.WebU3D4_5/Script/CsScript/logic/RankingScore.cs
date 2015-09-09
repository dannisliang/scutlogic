using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZyGames.Framework.Cache.Generic;
using GameRanking.Pack;
using GameServer.Model;
using ZyGames.Framework.Common;
using ZyGames.Framework.Model;
using ZyGames.Framework.Data;
using ZyGames.Framework.Common.Configuration;
using ZyGames.Framework.Common.Timing;
using ZyGames.Framework.Common.Log;
using Game.NSNS;

namespace Game.Script
{
    public class RankingScore  : Ranking<UserRanking>
    {
        static public int limitScoreAdd { get; private set; }
       
        public RankingScore():
            base()
        { }

        protected override void beforeDoRefresh()
        {
            base.beforeDoRefresh();
            ConsoleLog.showNotifyInfo("beforeDoRefresh  UserRanking cnt:" + _lst.Count);
        }
        protected override void afterDoRefresh()
        {
            ConsoleLog.showNotifyInfo("afterDoRefresh UserRanking cnt:" + _lst.Count);
            if(_lst.Count<limitIndex)
            {
                limitScoreAdd = 100;
            }
            else
            {
                limitScoreAdd = _lst[limitIndex].Score;
            }
        }

        protected override int comp(UserRanking t1, UserRanking t2)
        {
            int result = t2.Score - t1.Score;
            if (result == 0)
            {
                result = t2.UserID - t1.UserID;
            }
            return result;
        }

        protected override UserRanking copyT(UserRanking t)
        {
            UserRanking ur = new UserRanking();
            ur.UserID = t.UserID;
            ur.Score = t.Score;
            ur.UserName = t.UserName;
            return ur;
        }
    }
}
