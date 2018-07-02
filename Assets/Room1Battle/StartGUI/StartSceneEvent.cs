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
            Debug.Log("HonShu");
            Messenger.Broadcast(GAME_START);
        }
    }
}