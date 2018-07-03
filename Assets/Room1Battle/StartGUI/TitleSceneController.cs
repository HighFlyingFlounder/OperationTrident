using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.StartScene
{
    public class TitleSceneController : MonoBehaviour
    {
        [SerializeField]
        private Canvas loginCanvas; // 登录界面

        [SerializeField]
        private Canvas titleCanvas; // 主题界面

        [SerializeField]
        private Canvas roomCanvas; // 房间界面

        void Awake()
        {
            Messenger.AddListener(StartSceneEvent.GAME_START,OnGameStart);
        }

        void Destroy()
        {
            Messenger.RemoveListener(StartSceneEvent.GAME_START, OnGameStart);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGameStart()
        {

        }
    }
}
