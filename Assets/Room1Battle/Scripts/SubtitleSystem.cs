using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.Room1
{
    public class SubtitleSystem : MonoBehaviour
    {
        public AudioSource subtitleSource;


        public AudioClip subtitleInit;
        public string[] subtitleInitToDisplay = {
            "^b地球指挥官:^w根据情报显示，开启电源室入口的^y智能感应芯片^w在仓库里的几个可能位置",
            "^b地球指挥官:^w你们要拿到它，小心里面的^r巡逻机器人"
        };
        public float[] secondsInitInEachLine =
        {
            6.0f,4.0f
        };
        public float[] secondsInitBetweenLine =
        {
            0.5f,0.5f
        };


        public string[] subtitleOpenDoorFalseToDisplay ={
            "^rAI:^w对不起，身份识别错误",
            "^b队友:^w门居然打不开，来点硬一点的方法吧"
        };

        public string[] subtitleEscapingToDisplay = {
            "^b地球指挥官:^w注意，仓库的警报启动",
            "^b地球指挥官:^w大量的^r防御机器人^w正在进入你们的房间"
        };

        private string[] subtitlesToDisplay;
        private float[] secondsInEachLine;
        private float[] secondsBetweenLine;
        
        // Use this for initialization
        void Start()
        {
            subtitleSource.loop = false;
        }

        // Update is called once per frame
        void Update()
        {
            switch (SceneController.state)
            {
                // 场景的初始状态
                case SceneController.Room1State.Initing:
                    subtitlesToDisplay = subtitleInitToDisplay;
                    secondsInEachLine = secondsInitInEachLine;
                    secondsBetweenLine = secondsInitBetweenLine;
                    subtitleSource.clip = subtitleInit;
                    subtitleSource.Play();
                    //subtit
                    break;
                // 玩家正在找第一个钥匙
                case SceneController.Room1State.FindingKey1:

                    break;
                // 玩家正在找第二个钥匙
                case SceneController.Room1State.FindingKey2:

                    break;
                // 玩家正准备尝试开最后一个门
                case SceneController.Room1State.TryingToOpenRoom:

                    break;
                // 玩家正在找必需品
                case SceneController.Room1State.FindingNeeded:
                    subtitlesToDisplay = subtitleOpenDoorFalseToDisplay;
                    break;
                // 玩家正在找ID卡
                case SceneController.Room1State.FindingIDCard:

                    break;
                // 玩家正在逃离房间
                case SceneController.Room1State.EscapingRoom:
                    subtitlesToDisplay = subtitleEscapingToDisplay;
                    break;
            }
        }

        void OnGUI()
        {
            GUIUtil.DisplaySubtitlesInGivenGrammarWithTimeStamp(
                subtitlesToDisplay,
                Util.GetCamera(),
                fontSize: 20,
                subtitleRatioHeight: 0.9f,
                secondsOfEachLine: secondsInEachLine,
                secondBetweenLine: secondsBetweenLine);
        }
    }
}
