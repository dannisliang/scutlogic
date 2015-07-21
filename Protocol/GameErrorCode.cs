using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.YYS.Protocol
{
    // range of error for every action is 
    // ErrorCode = [actID*100 ,actID*100+20] 
    // the max number of errorcode for action is 100

    // less 1000*100, for ScutGame Framework.
    public class ErrorCodeL
    {
        public  const int L = 100;
    }
    
    //
    // name = Error_ActionID_Desc
    // val  = actionID * ErrorCodeL.L + codeNum
    //
    public enum GameErrorCode
    {
        Error_OK = 0,
        
       //action3000 [300000,300100)
       Error_3000_Test01 = 3000 * ErrorCodeL.L + 1,
       Error_3000_Test02 = 3000 * ErrorCodeL.L + 2,
    }
}
