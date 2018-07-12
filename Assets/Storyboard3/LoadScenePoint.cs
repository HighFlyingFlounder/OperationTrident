using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OperationTrident.Util;

public class LoadScenePoint : MonoBehaviour {
    public string nextScene = "SpaceBattle";
    private Camera m_FadeOutGuiCamera;
    void Start()
    {
        m_FadeOutGuiCamera = null;
    }
    void Update()
    {
        FadeInOutUtil.UpdateState();
    }
    private void OnGUI()
    {
        FadeInOutUtil.RenderGUI();
    }
    public void SetGuiCamera(Camera cam)
    {
        m_FadeOutGuiCamera = cam;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            //淡出
            float fadeOutDuration = 3.0f;
            FadeInOutUtil.SetFadingState(fadeOutDuration, m_FadeOutGuiCamera, Color.black, FadeInOutUtil.FADING_STATE.FADING_OUT);
            StartCoroutine(FadeOutCoroutine(fadeOutDuration));//等多一下确定淡出完成？
        }
    }

    private IEnumerator FadeOutCoroutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        GameMgr.instance.nextScene = nextScene;
        SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        //SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
