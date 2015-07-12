using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Game.NSNS
{
    public class SingletonClass<T> where T :  new()
    {
        static T ins = default(T);
        public static T Singleton()
        {
            if(null == ins)
            {
                ins = new T();
            }

            return ins;
        }
    }
    public class TimerMgr : SingletonClass<TimerMgr>
    {
        Dictionary<string, Timer> _timers;

        void Init()
        {
            Console.WriteLine("TimerMgr Init");
            _timers = new Dictionary<string, Timer>();
        }

        public TimerMgr(){ Init(); }
        // add , if alreadly have same timer, remove first then add.
        public void add(string id,TimerCallback cb, int dueTime,int period)
        {
            Timer t = null;
            if(_timers.ContainsKey(id))
            {
                t = _timers[id];
                if(null != t) t.Dispose();
                t = null;
                _timers.Remove(id);
            }

            _timers.Add(id,new Timer(cb,null,dueTime,period));
        }
    }
}
