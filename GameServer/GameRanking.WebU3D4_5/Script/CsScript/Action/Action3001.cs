
/****************************************************************************
    File:		Action3001.cs
    Desc:	    服务器action分发配置文件。
    Date:		2015-7-21   19:28
    Author:		guccang
    URL:		http://guccang.github.io
    Email:		guccang@126.com
****************************************************************************/

using Game.YYS.Protocol;
using GameServer.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using System.Collections.Generic;
using ZyGames.Framework.Common.Log;
using Game.Script;
using System.IO;
using System.Text;
using System.Web;

namespace GameServer.CsScript.Action
{

    /*
     * Class:   Action3001
     * Desc:   	中心服务器，下发客户端配置文件
     *          for load balance base
     * Author：	guccang
     * Date：	2015-7-21 19:30:00
     */
    /// <summary>
    /// 中心服务器，下发客户端配置文件
    /// for load balance
    /// </summary>
    public class Action3001 : BaseAction
    {
        Action3001Response responsePack;
        Action3001Request requestPack;

        public Action3001(ActionGetter actionGetter)
            : base(3001, actionGetter)
        {
            responsePack = new Action3001Response();
        }

        public override bool GetUrlElement()
        {
            byte[] data = (byte[])actionGetter.GetMessage();
            if (data.Length > 0)
            {
                requestPack = ProtoBufUtils.Deserialize<Action3001Request>(data);
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            return true;
        }

        protected override byte[] BuildResponsePack()
        {
            return ProtoBufUtils.Serialize(responsePack);
        }
    }

}