using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

public class Fadein : MonoBehaviour
{
    public bool fadeIn = true;
    public float duration = 5.0f;

    // Use this for initialization
    void Start()
    {
        FadeInOutUtil.SetFadingState(duration, OperationTrident.Room1.Util.GetCamera(), Color.black, fadeIn ? FadeInOutUtil.FADING_STATE.FADING_IN : FadeInOutUtil.FADING_STATE.FADING_OUT);
    }

    // Update is called once per frame
    void Update()
    {
        FadeInOutUtil.UpdateState();
    }

    private void OnGUI()
    {
        FadeInOutUtil.RenderGUI();
    }
}
