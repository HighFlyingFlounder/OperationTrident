using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.Elevator
{
    public class Subtitle : MonoBehaviour
    {
        public string[] subtitles;
        //public Camera camera = Camera.current;
        public float[] secondsOfEachLine;
        public float[] secondBetweenLine;
        // Use this for initialization
        void Start()
        {
            GUIUtil.ResetSubtitle();
        }

        private void OnGUI()
        {
            if (SceneController.state == SceneController.ElevatorState.Ready)
            {
                GUIUtil.DisplaySubtitlesInGivenGrammarWithTimeStamp(
                    subtitles,
                    Room1.Util.GetCamera(),
                    GUIUtil.DefaultFontSize,
                    GUIUtil.DefaultSubtitleRatioHeight,
                    secondsOfEachLine,
                    secondBetweenLine);
            }
        }
    }
}
