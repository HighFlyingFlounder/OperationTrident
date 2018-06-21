using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.Room1
{
    // 暂时还用不到吧

    public class SubtitleSystem : MonoBehaviour
    {
        // 要显示的以文法显示的字幕
        public string subtitleInGrammar;
        // 每个字显示的时间
        public float timePerSubTitleWord;
        // 每个字的大小
        public int fontSize = 20;
        // 字幕在屏幕中的位置
        public float subtitleRatioHeight = 0.9f;

        // 是否准备好显示字幕
        public bool UptoDisplay = true;
        [SerializeField]
        private new Camera camera;
        // Use this for initialization
        void Start()
        {
            if (camera == null)
            {
                camera = GetComponent<Camera>();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void onGUI()
        {

            GUIUtil.DisplaySubtitleInGivenGrammar(subtitleInGrammar, camera, fontSize, subtitleRatioHeight, subtitleInGrammar.Length * timePerSubTitleWord);

        }
    }
}