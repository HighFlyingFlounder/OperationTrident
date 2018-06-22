using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationTrident.EventSystem
{
    // 记录Room1里面的所有游戏事件！
    static public class GameEvent
    {
        public const String Enemy_Start = "E_START";
        public const String Push_Button = "B_PUSH";
        public const String End = "END";
    }
}
