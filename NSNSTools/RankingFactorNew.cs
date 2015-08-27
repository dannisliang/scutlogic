using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.Threading;
using ZyGames.Framework.Common.Timing;
using ZyGames.Framework.Common;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Model;
using ZyGames.Framework.Common.Log;
using Game.NSNS;

namespace Game.NSNS
{

   
    public class sortMethod
    {
        public enum sortType 
        {
            None,
            Interval,
            Timing,
            week,
        }

        Timer _timer;
        PlanCallback _f = null;

        public void timeCb(object o)
        {
            if (null != _f) _f(null);
        }
        public sortMethod(sortType t,string parm,PlanCallback f)
        {
            _f = f;
            if(sortType.None == t)
            {
 
            }
            else if(sortType.Interval == t)
            {
                string []p = parm.Split(',');
                int p1 = 0, p2 = 0;
                try
                {
                    p1 = int.Parse(p[0]);
                    p2 = int.Parse(p[1]);
                }
                catch(Exception e)
                {
                    p1 = 1000;
                    p2 = 1000 * 60 * 60;
                }
                _timer = new Timer(timeCb, null, p1,p2);
            }
            else if(sortType.Timing == t)
            {
                string[] timers = parm.Split(',');
                for (int i = 0; i < timers.Length; ++i)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(timers[i]);
                    }
                    catch(Exception e)
                    {
                        ConsoleLog.showErrorInfo(i, e.Message+":datePase Error;"+timers[i]);
                        continue;
                    }
                    TimeListener.Append(PlanConfig.EveryDayPlan(_f, "EveryDayTask", timers[i]));
                }
            }
            else if(sortType.week == t)
            {
            
            }
        }
    }

    public class RankingBase 
    {
        protected ReaderWriterLockSlim _rwLock = null;
        protected const int _writeWaiteTime = 300000;
        protected const int _readWaiteTime = 1000;
        protected sortMethod _sortMethod;
        public void Start(sortMethod.sortType t, string tp)
        {
            _sortMethod = new sortMethod(t, tp, refresh);
            refresh(null);
        }
        public RankingBase()
        {
            _rwLock = new ReaderWriterLockSlim();
        }

        protected virtual void doRefresh()
        { }

        public virtual void Clear()
        { }

        public void refresh(object o)
        {
            try
            {
                    doRefresh();
            }
            catch (Exception e) { ConsoleLog.showErrorInfo(0, "refresh:" + e.Message); }
        }
    
    }

    public class Ranking<T> :RankingBase where T : ShareEntity ,new()
    {
        protected List<T> _lst = null;
        protected int limitIndex = 100000;
        protected int delOneTime = 1000;
        protected  virtual int comp(T t1, T t2)
        {
            return -1;
        }
        // override..this...
        protected virtual T copyT(T t)
        {
            return null;
        }

        public void Loop<T>(System.Func<List<T>,bool> cb) where T : ShareEntity , new()
        {
            if(_rwLock.TryEnterReadLock(_readWaiteTime))
            {
                try
                {
                    if(null !=cb ) 
                        cb(_lst as List<T>);
                }
                catch(Exception e)
                {
                    ConsoleLog.showErrorInfo(0,"error @ Lopp"+typeof(T).ToString()+":"+e.Message);
                }
                finally
                {
                    if (_rwLock.IsReadLockHeld)
                        _rwLock.ExitReadLock();
                }
            }
        }
        protected bool addData(string key, T t)
        {
            T d = copyT(t);
            if(null == d)
            {
                ConsoleLog.showErrorInfo(0, "Error addData:"+typeof(T).ToString());
                return false;
            }
            _lst.Add(d);
            return true;
        }

        public override void Clear()
        {
            try
            {
                if (_rwLock.TryEnterWriteLock(_writeWaiteTime))
                {
                    try
                    {
                        ConsoleLog.showNotifyInfo("Ranking Clear");
                        _lst.Clear();
                        //var cache = new ShareCacheStruct<T>();
                        //cache.UnLoad();
                    }
                    catch (System.Exception e)
                    {
                        string info = string.Format("raning factory clear failed:{0}#{1}", typeof(T).ToString(), e.Message);
                        ConsoleLog.showErrorInfo(0, info);
                        TraceLog.WriteError(info);

                    }
                    finally
                    {
                        if (_rwLock.IsWriteLockHeld) _rwLock.ExitWriteLock();
                        ConsoleLog.showNotifyInfo("Ranking Clear End");
                    }
                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.showErrorInfo(0, e.Message);
                string info = "Clear Lock:"+e.Message;
                ConsoleLog.showErrorInfo(0, info);
                TraceLog.WriteError(info);
            }
        }

        protected virtual void beforeDoRefresh()
        {
            if (_lst.Count > limitIndex)
            {
                var cache = new ShareCacheStruct<T>();
                for (int i = _lst.Count - 1; i > _lst.Count - delOneTime; i--)
                {
                    cache.Delete(_lst[i]);
                }
            }
        }

        protected  override void doRefresh()
        {
            try
            {
                if (_rwLock.TryEnterWriteLock(_writeWaiteTime))
                {
                    try
                    {
                        beforeDoRefresh();
                        _lst.Clear();
                        var cache = new ShareCacheStruct<T>();
                        cache.Foreach(addData);
                        MathUtils.QuickSort<T>(_lst, comp);
                        afterDoRefresh();
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.showErrorInfo(0,"DoRefresh Error:"+e.Message);
                    }
                    finally
                    {
                        if (_rwLock.IsWriteLockHeld)
                                  _rwLock.ExitWriteLock();
                    }
                }
            }
            catch (Exception e)
            {
                TraceLog.WriteError("DoRefresh Error:" + e.Message);
            }
        }    
        protected virtual void afterDoRefresh() { }

        public List<T> get()
        {
            if( _rwLock.TryEnterReadLock(_readWaiteTime) )
            {
                try
                {
                    return _lst;
                }
                catch (Exception e)
                {
                    string info = typeof(T).ToString() + ":Ranking Get:" + e.Message;
                    ConsoleLog.showErrorInfo(0, info);
                    TraceLog.WriteError(info);
                    return null;
                }
                finally
                {
                    if (_rwLock.IsReadLockHeld) _rwLock.ExitReadLock();
                }
            }
            else
            {
                return null;
            }
        }

        public Ranking()
        {
            _lst = new List<T>();
        }
   
    }

    public class RankingFactorNew
    {
        static RankingFactorNew _ins = null;
        Dictionary<string, RankingBase> _dic;
        public static RankingFactorNew Singleton()
        {
            if(_ins == null)
            {
               _ins = new RankingFactorNew();
                _ins.Init();
            }
            return _ins;
        }

        // T:UserRanking,       ST:RankingScore
        // T:UserRankingTotal,  ST:RankingTotal
        public T getRankingData<T,ST>(int index) where T : ShareEntity,new()
        {
            List<T> d = get<T>(typeof(ST).ToString());
            if (index < d.Count)
                return d[index];
            else
                return null;
        }

        void Init()
        {
            _dic = new Dictionary<string, RankingBase>();
        }
        /*
            key     -> Ranking<T>
         *  t       -> how to user sort
         *  tp      -> the string of t's parm.the parse is according to sortType
         *  rt      -> add sort update raningdata
         */
        public void add<T>(string key, Ranking<T> rt) where T : ShareEntity, new()
        {
            try
            {
                _dic.Add(key, rt);
            }catch(System.Exception e)
            {
                ConsoleLog.showErrorInfo(0,"e:"+e.Message + " key:" + key+" T:"+typeof(T).ToString());
            }
            finally
            {
            }
        }

        public void Start<T>(string key,sortMethod.sortType t,string parm) where T :ShareEntity,new()
        {
            try
            {
                _dic[key].Start(t, parm);
            }
            catch
            { }
            finally
            { }
        }
        public void Refresh<T>(string key) where T : ShareEntity ,new ()
        {
            try
            {
                _dic[key].refresh(null);
            }
            catch (System.Exception e)
            {
                ConsoleLog.showErrorInfo(0, "e:" + e.Message + " key:" + key + " T:" + typeof(T).ToString());
            }
            finally
            { }
        }

        public void Clear<T>(string key) where T : ShareEntity, new()
        {
            try
            {
                _dic[key].Clear();
            }
            catch(System.Exception e)
            {
                ConsoleLog.showErrorInfo(0, "e:" + e.Message + " key:" + key + " T:" + typeof(T).ToString());
            }
            finally
            {

            }
        }

        public void  Loop<T>(string key,System.Func<List<T>,bool> cb) where T : ShareEntity,new()
        {
            try
            {
                if (_dic.ContainsKey(key))
                {
                    Ranking<T> rt = _dic[key] as Ranking<T>;
                    rt.Loop<T>(cb);
                }
             }
            finally
            {
            }
        }
     
       public List<T> get<T>(string key) where T : ShareEntity ,new()
        {
            try
            {
                if (_dic.ContainsKey(key))
                {
                    Ranking<T> rt =  _dic[key] as Ranking<T>;
                    return rt.get();
                }
                return null;
            }
            finally
            {
            }
        }
    }
}
