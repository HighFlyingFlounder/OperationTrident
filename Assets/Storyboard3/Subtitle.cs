using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

public class Subtitle : MonoBehaviour {
    public string[] subtitles;
    //public Camera camera = null;
    public float[] secondsOfEachLine;
    public float[] secondBetweenLine;
    // Use this for initialization
    void Start () {
        
    }

    private void OnGUI()
    {
        GUIUtil.DisplaySubtitlesInGivenGrammarWithTimeStamp(
            subtitles,
            Camera.current,
            GUIUtil.DefaultFontSize,
            GUIUtil.DefaultSubtitleRatioHeight,
            secondsOfEachLine,
            secondBetweenLine);
    }
}
