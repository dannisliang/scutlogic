using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Log;
using GameServer.Model;
using Game.NSNS;

namespace Game.Script
{
    public class GameConfigExchangeCode : GameConfigBase
    {
        public enum  ExchangeCodeType
        {
            ASType_NSNSDuiHuan_XinShou_30000 = 35,				//	NSNS新手兑换码-30000
            ASType_NSNSDuiHuan_AnHuiTai_1 = 36,					//	NSNS安徽台兑换码-1
            ASType_NSNSDuiHuan_360_50000 = 37,					//	NSNS,360预留兑换码-50000

            ASType_NSNSDuiHuan_BuChang_A_100 = 38,
            ASType_NSNSDuiHuan_BuChang_B_100 = 39,
            ASType_NSNSDuiHuan_BuChang_C_100 = 40,
            ASType_NSNSDuiHuan_BuChang_D_100 = 41,
            ASType_NSNSDuiHuan_BuChang_E_100 = 42,
            ASType_NSNSDuiHuan_Super360_10000 = 43,
            ASType_NSNSDuiHuan_ChaoZhi_30000 = 44,
            ASType_NSNSDuiHuan_TuHao_30000 = 45,

            ASType_NSNSDuiHuan_TeQuan_30000 = 46,
            ASType_NSNSDuiHuan_ShuangKuai_30000 = 47,
            ASType_NSNSDuiHuan_KuaiGan_30000 = 48,

            ASType_NSNSDuiHuan_360NewPlayer_50000 = 49,
            ASType_NSNSDuiHuan_360Only_50000 = 50,
            ASType_NSNSDuiHuan_ErTongJie_50000 = 51,
            ASType_NSNSDuiHuan_DuanWuJie_50000 = 52,

            ASType_NSNSDuiHuan_KaiJu_50000 = 53,
            ASType_NSNSDuiHuan_BinFen_50000 = 54,
            ASType_NSNSDuiHuan_XiaRi_50000 = 55,
            ASType_NSNSDuiHuan_KaiXin_50000 = 56,
            ASType_NSNSDuiHuan_HuanLe_50000 = 57,
            ASType_NSNSDuiHuan_HuanXiang_50000 = 58,
            ASType_NSNSDuiHuan_JiSu_50000 = 59,
            ASType_NSNSDuiHuan_ChongCi_50000 = 60,
            ASType_NSNSDuiHuan_KuangBiao_50000 = 61,
            ASType_NSNSDuiHuan_BuChang_Role_30000 = 62,

            ASType_NSNSDuiHuan_BuChang_LittleRole_30000 = 63,
            ASType_NSNSDuiHuan_BuChang_F_30000 = 1,
            ASType_NSNSDuiHuan_BuChang_G_30000 = 2,
            ASType_NSNSDuiHuan_BuChang_H_30000 = 3,
            ASType_NSNSDuiHuan_BuChang_I_30000 = 4,

            ASType_NSNSDuiHuan_ZhuanShu_30000 = 5,
            ASType_NSNSDuiHuan_ZhuanXiang_30000 = 6,
            ASType_NSNSDuiHuan_DingZhi_30000 = 7,
            ASType_NSNSDuiHuan_wifi_30000 = 8,
            ASType_NSNSDuiHuan_JiaRi_1 = 9,
            ASType_NSNSDuiHuan_ChaoHang_10000 = 10,
            ASType_NSNSDuiHuan_TuCao_10000 = 11,

            ASType_NSNSDuiHuan_TeQuan2_5w = 12,
            ASType_NSNSDuiHuan_TeQuan3_5w = 13,
            ASType_NSNSDuiHuan_ShuangKuai2_5w = 14,
            ASType_NSNSDuiHuan_ShuangKuai3_5w = 15,
            ASType_NSNSDuiHuan_KuaiGan2_5w = 16,
            ASType_NSNSDuiHuan_KuaiGan3_5w = 17,

            ASType_NSNSDuiHuan_BuChang_Gold = 18,
            ASType_NSNSDuiHuan_BuChang_ShuxingLevelMax = 19,
            ASType_NSNSDuiHuan_BuChang_RoleLevelMax = 20,
            ASType_NSNSDuiHuan_BuChang_Diamond6 = 21,
            ASType_NSNSDuiHuan_BuChang_Diamond12 = 22,
            ASType_NSNSDuiHuan_BuChang_Diamond18 = 23,
            ASType_NSNSDuiHuan_BuChang_Diamond30 = 24,
            ASType_NSNSDuiHuan_BuChang_Diamond60 = 25,
            ASType_NSNSDuiHuan_BuChang_Diamond120 = 26,

            ASType_NSNSDuiHuan_Unlock_Nvchongke = 27,
            ASType_NSNSDuiHuan_Unlock_Dama = 28,
            ASType_NSNSDuiHuan_Unlock_Lgn = 29,
            ASType_NSNSDuiHuan_Unlock_Dabai = 30,
            ASType_NSNSDuiHuan_Unlock_Xihanan = 31,
            ASType_NSNSDuiHuan_Unlock_Newgirl = 32,
            ASType_NSNSDuiHuan_Unlock_Tu = 33,

        }
        public override void init()
        {
            base.init();
        }

        public GameConfigExchangeCode() { }

        public GameConfigExchangeCode(string path)
        {
            _path = path;
            _name = path + "//";
        }
        public override void getAllConfigFiles(List<string> versions)
        {
            DirectoryInfo folder = new DirectoryInfo(_path);

            foreach (FileInfo file in folder.GetFiles())
            {
                string name = file.Name;
                name = name.Remove(name.LastIndexOf('.'));
                if (versions.Contains(name)) continue;
                versions.Add(name);
            }
        }

        protected override void gameConfigCreate(string fileName, string name)
        {
            var cache = new ShareCacheStruct<ExchangeCode>();
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName);
            string line = "";
            ExchangeCodeType type = (ExchangeCodeType)System.Enum.Parse(typeof(ExchangeCodeType), name, true);
            if ((byte)type < 0 || (byte)type > byte.MaxValue)
            {
                ConsoleLog.showErrorInfo(0, "exchangeCode type:" + fileName);
            }
            int index = 0;
            while ((line = stream.ReadLine()) != null)
            {
                // TODO
                if (false == ExchangeCodeMgr.Instance().Add((int)type, index++, line))
                {
                    ConsoleLog.showNotifyInfo(name + ":has been added",ConsoleLog.ConsoleLogType.Debug);
                    break;

                }
            }
            stream.Close();
            ConsoleLog.showNotifyInfo(fileName+" is update to new version.");
        }
        protected override string getFullConfigName(string version)
        {
            return _name + version + ".txt";
        }
    }

}
