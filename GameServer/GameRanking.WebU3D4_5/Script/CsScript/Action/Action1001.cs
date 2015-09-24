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
using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using GameRanking.Pack;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.RPC.IO;
using System.Linq;
using Game.Script;
using System.Diagnostics;
using Game.NSNS;
using ZyGames.Framework.Common.Log;

namespace GameServer.CsScript.Action
{
    public class comp : IComparer<UserRanking>
    {
        public int Compare(UserRanking x, UserRanking y)
        {
            return y.Score.CompareTo(x.Score);
        }
    }

    public class compRD : IComparer<RankData>
    {
        public int Compare(RankData t1, RankData t2)
        {
            int result = t2.Score - t1.Score;
            if (result == 0)
            {
                result = t2.UserID - t1.UserID;
            }
            return result;
        }
    }

    public class ModelComparer : IEqualityComparer<UserRanking>  
    {
        public bool Equals(UserRanking a, UserRanking b)
        {
            return a.Score == b.Score;
        }
        public int GetHashCode(UserRanking obj)
        {
            return obj.Score.GetHashCode();
        }
    }
    public class Action1001 : BaseAction
    {
        private Request1001Pack requestPack;
        private Response1001Pack responsePack;
        private Stopwatch _watch;

        public Action1001(ActionGetter actionGetter)
            : base(1001, actionGetter)
        {
            _watch = new Stopwatch();
            responsePack = new Response1001Pack();
        }

        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Request1001Pack>(data);
                return true;
            }
            return false;
        }

        int formatPos(int pos)
        {
            if (pos < 0)
            {
                pos = ~pos;
                if (pos + 1 > 99999) pos = 99998;
            }
            if (pos + 1>= 99999) pos = 99998;
            return pos;
        }

        bool cbFuncRankingTotal(List<UserRankingTotal> rankingList)
        {
            if (null == rankingList || rankingList.Count == 0)
            {
                return false;
            }
            // self 
            UserRanking ur = new UserRanking();
            var cache = new ShareCacheStruct<UserRankingTotal>();
            UserRankingTotal selfURT = new UserRankingTotal();
            UserRankingTotal findURT = cache.FindKey(requestPack.UserID);
            int selfPos = rankingList.FindIndex((o) =>
            {
                if(o.UserID == requestPack.UserID)
                        return true;
                return false;
            });

            if (findURT == null)
            {
                selfURT.UserID = requestPack.UserID;
                selfURT.Total = -1;
            }
            else
            {
                selfURT.UserID = findURT.UserID;
                selfURT.Total = findURT.Total;
            }

            selfPos = formatPos(selfPos);
            responsePack.ItemsExScore.Add(new RankData() { pos = selfPos, UserName = selfPos+","+selfURT.Total, Score = selfURT.Total, UserID = selfURT.UserID });

            int maxSend = GameConfigMgr.Instance().getInt("rank_send_num_total", 10);
            var personCache = new PersonalCacheStruct<GameUser>();
            var person = personCache.FindKey(selfURT.UserID.ToString());
       
            for (int i = 0; i < rankingList.Count; ++i)
            {
                if (i >= maxSend) break;
                person = personCache.FindKey(rankingList[i].UserID.ToString());
                if (null == person) continue;
                responsePack.ItemsExScore.Add(new RankData() { pos = formatPos(i), UserName = person.NickName, Score = rankingList[i].Total, UserID = rankingList[i].UserID });
            }

            return true;
        }
        
        bool cbFuncDuang(object obj)
        {
            List<UserRanking> objList = obj as List<UserRanking>;
            List<UserRanking> rankingList = null;
            if (null == objList)
            {
                return false;
            }
            else
            {
                rankingList = objList;
            }

            if (null == rankingList || rankingList.Count == 0)
            {
                return false;
            }

            int selfScore = requestPack.PageIndex;
            UserRanking ur = new UserRanking();
            ur.Score = selfScore;
            int pos = rankingList.BinarySearch(ur, new comp());
            int h, l, maxPaiHangNum;
            maxPaiHangNum = objList.Count;

            //self
            pos = formatPos(pos);
            h = rankingList[0].Score;
            l = rankingList[rankingList.Count - 1].Score > selfScore ? selfScore : rankingList[rankingList.Count - 1].Score;
            responsePack.hightScore = h;
            responsePack.lowScore = l;
            responsePack.totalPlayer = rankingList.Count;
            responsePack.youPos = pos+1;
            RankData self = null;
            if(requestPack.UserID>0)
            {
                self = new RankData() { pos = responsePack.youPos, UserName = "self", Score = selfScore, UserID = requestPack.UserID };
                responsePack.Items.Add(self);
            }

            string sendIndex = GameConfigMgr.Instance().getString("rank_send_num_index", "1,2,3,500,1000,2000,5000,10000,20000,30000");
            string[] sendS = sendIndex.Split(',');
            for (int i = 0; i < sendS.Length; ++i)
            {
                int index = int.Parse(sendS[i])-1;
                if (index<0 || index > rankingList.Count - 1) continue;
                if (requestPack.UserID == rankingList[index].UserID)
                {
                    if (index+1 < responsePack.youPos)
                        responsePack.youPos = index + 1;
                    foreach(var v in responsePack.Items)
                    {
                        if(v.UserID == requestPack.UserID)
                        {
                            v.pos = index + 1;
                            break;
                        }
                    }
                    continue;
                }
                RankData rd = new RankData();
                rd.Score = rankingList[index].Score;
                rd.UserName = rankingList[index].UserName;
                rd.UserID = rankingList[index].UserID;
                rd.pos = formatPos(index+1);
                responsePack.Items.Add(rd);
            }
            //responsePack.Items.Sort(new compRD());
            return true;
        }
        bool cbFunc8self8(object obj)
        {
            List<UserRanking> objList = obj as List<UserRanking>;
            List<UserRanking> rankingList = null;
            if (null == objList)
            {
                return true;
            }
            else
            {
                rankingList = objList;
            }

            if (null == rankingList || rankingList.Count == 0)
            {
                return true;
            }

            int selfScore = requestPack.PageIndex;
            UserRanking ur = new UserRanking(); ur.Score = selfScore;
            int pos = rankingList.BinarySearch(ur, new comp());
            int h, l, maxPaiHangNum;
            maxPaiHangNum = objList.Count;

            //self
            responsePack.Items.Add(new RankData() { UserName = "", Score = int.MaxValue });
            h = rankingList[0].Score;
            l = rankingList[rankingList.Count - 1].Score > selfScore ? selfScore : rankingList[rankingList.Count - 1].Score;

            int maxSend = GameConfigMgr.Instance().getInt("rank_send_num", 10);
            if (maxSend > rankingList.Count) maxSend = 20;

            if (pos < 0)
            {
                pos = ~pos;
            }

            int theNumOf3 = (maxSend-3);
            int half =  theNumOf3 / 2;

            int begin = pos  - half;
            int end   = pos  + half;
            
            if(begin<0)
            {
                begin = 0;
                end   = maxSend;
            }

            if(end>rankingList.Count)
            {
                end = rankingList.Count;
                begin = end - theNumOf3;
                if(begin<0)
                {
                    begin = 0;
                    ConsoleLog.showErrorInfo(0,"Action1001,begin<0.UserID:"+requestPack.UserID);
                }
                if(pos == rankingList.Count)
                {
                    begin += 1;
                }
            }
            
                int cnt = Math.Min(begin, 3);
                for (int i = 0; i < cnt; ++i)
                {
                    if (rankingList[i].UserID == requestPack.UserID)
                    {
                        continue;
                    }
                    RankData rd = new RankData();
                    rd.Score = rankingList[i].Score;
                    rd.UserName = rankingList[i].UserName;
                    rd.UserID = rankingList[i].UserID;
                    responsePack.Items.Add(rd);
                }

            for (int i = begin; i < end; ++i)
            {
                if(rankingList[i].UserID == requestPack.UserID)
                {
                    continue;
                }
                RankData rd = new RankData();
                rd.Score = rankingList[i].Score;
                rd.UserName = rankingList[i].UserName;
                rd.UserID = rankingList[i].UserID;
                responsePack.Items.Add(rd);
            }

            RankData self = new RankData();
            self.UserID = requestPack.UserID;
            self.UserName = "self";
            self.Score = selfScore;
            responsePack.Items.Add(self);
            responsePack.Items.Sort(new compRD());

            if (pos < 0)
            {
                pos = ~pos;
                if (pos + 1 > 99999) pos = 99998;
            }
            if (pos >= 99999) pos = 99998;
            if (begin >= 99999) begin = 99998 - maxSend;
            if (end >= 99999) end = 99998;
            responsePack.Items[0].UserName = h + "," + l + "," + rankingList.Count + "," + pos+","+begin+","+end; // ��߷֣���ͷ֣��������Լ�����
            //ConsoleLog.showNotifyInfo(responsePack.Items[0].UserName);
            return true;
        }

        public override bool TakeAction()
        {
            RankingFactorNew.Singleton().Loop<UserRankingTotal>(typeof(RankingTotal).ToString(), cbFuncRankingTotal);

            var cache = new PersonalCacheStruct<GameUser>();
            GameUser p = null;
            if(requestPack.UserID>0)
            {
                p = cache.FindKey(requestPack.UserID.ToString());
            }
            if(p==null)
            {
                RankingFactorNew.Singleton().Loop<UserRanking>(typeof(RankingScore).ToString(), cbFuncDuang);
            }
            else
            {
                if(p.version != "1.08")
                {
                    RankingFactorNew.Singleton().Loop<UserRanking>(typeof(RankingScore).ToString(), cbFuncDuang);
                }
                else
                {
                    RankingFactorNew.Singleton().Loop<UserRanking>(typeof(RankingScore).ToString(), cbFunc8self8);
                }
            }
           return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);           
        }

        private int compareTo(UserRanking x, UserRanking y)
        {
            int result = y.Score - x.Score;
            if (result == 0)
            {
                result = y.UserID - x.UserID;
            }
            return result;
        }
    }
}
