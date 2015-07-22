using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;
using ZyGames.Framework.Common.Log;

namespace Game.Script
{
    public class ExchangeCodeMgr
    {
        static ExchangeCodeMgr _instance = null;
        public static ExchangeCodeMgr Instance()
        {
            if (null == _instance)
            {
                _instance = new ExchangeCodeMgr();
                _instance.Init();
            }
            return _instance;
        }

        public int isOk(int type,int index,string code)
        {
            var cache = new ShareCacheStruct<ExchangeCode>();
            int key = UniqueKey(type, index);
            ExchangeCode ec = cache.FindKey(key);
            if (null == ec)        return 1;    // not find
            if (type  != ec.Type)  return 2;    
            if (index != ec.Index) return 3;
            if (code  != ec.Code)  return 4;
            if (ec.cnt <= 0)       return 5;    // used...

            ec.ModifyLocked(() =>
            {
                ec.cnt -= 1;
            });
            return 0;
        }

        public bool Add(int type,int index,string code)
        {
            var cache = new ShareCacheStruct<ExchangeCode>();
            int key = UniqueKey(type, index);
            //if (null != cache.FindKey(key)) return false; // this type has been added
            ExchangeCode ec = cache.FindKey(key);
            if (null != ec)
            {
                return true;
            }
            else
            {
                ec = new ExchangeCode();
                ec.key = key;
                ec.Type = (byte)type;
                ec.Index = index;
                ec.Code = code;
                if (ec.Type == (byte)GameConfigExchangeCode.ExchangeCodeType.ASType_NSNSDuiHuan_AnHuiTai_1 ||
                    ec.Type == (byte)GameConfigExchangeCode.ExchangeCodeType.ASType_NSNSDuiHuan_JiaRi_1)
                {
                    ec.cnt = int.MaxValue;

                }
                cache.Add(ec);
            }
            return true;
        }
        public int UniqueKey(int type, int index)
        {
            type    = (type  &  0x00000FF)<<20;
            index   = index &  0x00FFFFF;
            return (type | index);
        }


        void Init()
        {
            var cache = new ShareCacheStruct<ExchangeCode>();
        }
    }
}
