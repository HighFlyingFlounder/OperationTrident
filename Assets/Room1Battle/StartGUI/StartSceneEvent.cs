using OperationTrident.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OperationTrident.StartScene {
    public class StartSceneEvent : MonoBehaviour
    {
        public static readonly string GAME_START = "GAME_START";
        public static readonly string HELP = "HELP";
        public static readonly string GIVE_MONEY = "GIVE_MONEY";
        public static readonly string EXIT_GAME = "EXIT_GAME";

        /// <summary>
        /// 开始游戏按钮回调
        /// </summary>
        public void OnGameStart()
        {
            Messenger.Broadcast(GAME_START);
        }

        /// <summary>
        /// 帮助界面按钮回调
        /// </summary>
        public void OnHelp()
        {
            Messenger.Broadcast(HELP);
        }

        /// <summary>
        /// 打赏界面按钮回调
        /// </summary>
        public void OnGiveMoney()
        {
            Messenger.Broadcast(GIVE_MONEY);
        }

        /// <summary>
        /// 退出界面按钮回调
        /// </summary>
        public void OnExitGame()
        {
            Messenger.Broadcast(EXIT_GAME);
        }
    }
}