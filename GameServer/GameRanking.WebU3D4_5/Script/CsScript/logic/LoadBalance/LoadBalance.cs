
/****************************************************************************
    File:		LoadBalance.cs
    Desc:		负载均衡模块实现
    Date:		2015-7-22 11:11:11
    Author:		guccang
    URL:		http://guccang.github.io
    Email:		guccang@126.com
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.NSNS;
using ZyGames.Framework.Common.Configuration;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;

namespace Game.Script
{

    /*
     * Class:   ServerInfo
     * Desc:   	服务器描述数据
     * Author：	guccang
     * Date：	2015-7-21 11:11:11
     */
    /// <summary>
    /// ServerInfo Document
    /// </summary>
    
    public class ServerInfo
    {
        public int serverID { get; private set; }
        public int serverType { get; private set; }
        public uint serverHost { get; private set; }
        public string serverHostStr { get; private set; }
        public int serverPort { get; private set; }

        private int getID()
        {
            var cache = new ShareCacheStruct<ServerInfoMode>();
            var info = cache.Find((o) =>
            {
                if (o.userd == false && o.type == serverType)
                    return true;
                return false;
            });
            int id = -1;
            if(info!=null)
            {
                info.ModifyLocked(() => { 

                    if(info.userd==false)
                    {
                        id = info.id;
                        info.userd = true;
                    }
                });
            }
            return id<0 ? (int)cache.GetNextNo() : id;
        }
        public ServerInfo()
        {
            // read config
            serverType      = GameConfigMgr.Instance().getInt("serverType", 0x0010000);
            serverID        = getID();
            serverHostStr   = ConfigUtils.GetSetting("Game.Http.Host", "0.0.0.0");
            serverHost      = utils.getIpUnit(serverHostStr);
            serverPort      = ConfigUtils.GetSetting("Game.Http.Port", 0);
        }
    }
    
    /*
     * Class:   LoadBalance
     * Desc:   	负载均衡实现
     * Author：	guccang
     * Date：	2015-7-22 11:11:11
     */
    /// <summary>
    /// LoadBalance Document
    /// </summary>
    
    public class LoadBalance
    {
        public enum EnumServerType
        {
            E_ServerType_Center     = 0x0000001,
            E_ServerType_Login      = 0x0000010,
            E_ServerType_Pay        = 0x0000100,
            E_ServerType_Exchange   = 0x0001000,
            E_ServerType_GameServer = 0x0010000,
        }
        static LoadBalance ins = null;
        static int offLineCnt  = 30;
        ServerInfo si;
        static LoadBalance Singleton()
        {
            if(null == ins)
            {
                ins = new LoadBalance();
                ins.init();
            }
            return ins;
        }

        /*
         * Method:	init
         * Desc:   	初始化服务器负载均衡相关配置
         * Author：	guccang
         * Date：	2015-7-22 
         */
        /// <summary>
        /// init  Document
        /// desc:	 初始化服务器负载均衡相关配置 
        /// </summary>
        void init()
        {
            si = new ServerInfo();
            TimerMgr.Singleton().add("balanceWrite", write, 0, 5000);

            if (0!=(si.serverType & (int)EnumServerType.E_ServerType_Center))
            {
                TimerMgr.Singleton().add("balanceDo", balance, 0, 10000);
                TimerMgr.Singleton().add("balanceHeart", heart, 0, 3000);
            }
        }


        /*
         * Method:	heart
         * Desc:   	检测服务器在线情况
         * Author：	guccang
         * Date：	2015-7-21
         */
        /// <summary>
        /// heart	Document
        /// desc:		检测服务器在线情况 
        /// </summary>
        public void heart(object o)
        {
            var cache = new ShareCacheStruct<ServerInfoMode>();
            cache.Foreach((string s1, ServerInfoMode sim) =>
            {
                if(sim.offLineCnt<offLineCnt)
                {
                    sim.ModifyLocked(() =>
                    {
                        sim.offLineCnt += 1;
                    });
                }
                return true;
            });
        }
        
        /*
         * Method:	write
         * Desc:   	将服务器数据写入,负载均衡维护表中
         * Author：	guccang
         * Date：	2015-7-22
         */
        /// <summary>
        /// write Document
        /// desc:   将服务器数据写入,负载均衡维护表中
        /// </summary>
        
        public void write(object obj)
        {
            var cache = new ShareCacheStruct<ServerInfoMode>();
            var info = cache.FindKey(si.serverID.ToString());
            if(info != null)
            {
                info.ModifyLocked(() => {
                    info.offLineCnt = 0;
                });
            }
            else
            {

            }
        }


        /*
         * Method:	balance
         * Desc:   	center server 读取负载数据,分发actionid。
         *          以及健壮性维护,当某台服务器宕机后,将业务转移
         *          到其他服务器上.
         * Author：	guccang
         * Date：	2015-7-21 11:11:11
         */
        /// <summary>
        /// balance Document
        /// desc:    center server 搜集负载数据,用于分发actionid。
        ///          以及健壮性维护,当某台服务器宕机后,将业务转移
        ///           到其他服务器上.
        /// </summary>

        public void balance(object obj)
        {
            
        }
    }
}
