using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.EndingScene
{
    public class EndingSceneSubtitles : MonoBehaviour
    {

        public SceneDirector m_SceneDirector;
        public Camera m_CamFree;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        void OnGUI()
        {
            switch (m_SceneDirector.GetCameraState())
            {
                case SceneDirector.CameraState.ROAMING:

                    break;

                case SceneDirector.CameraState.THIRD_PERSON:
                    GUIUtil.DisplayMissionTargetInMessSequently("任务完成，返回基地.", m_CamFree, Color.white, 0.1f);
                    string[] subtitles =
                    {
                        "",//等几秒先
                        "^g蓝星陆战队^w：指挥部，已取回托卡马克之心",
                        "^g蓝星陆战队^w：陆战队所有成员均已登上逃生舱，任务完成",
                        "^g地球指挥部^w：收到，尽快返回海神号。",
                    };

                    float[] subtitleTime = { 8.3f, 2.5f, 4.0f, 3.0f };
                    float[] intervals = { 0, 0.5f, 2.0f, 2.0f };
                    GUIUtil.DisplaySubtitlesInGivenGrammarWithTimeStamp(
                        subtitles, m_CamFree, GUIUtil.DefaultFontSize, GUIUtil.DefaultSubtitleRatioHeight, subtitleTime, intervals);

                    break;

                case SceneDirector.CameraState.LOOKING_AT_KUN:

                    break;

                case SceneDirector.CameraState.VIDEO:

                    break;
            }

        }

    }
}