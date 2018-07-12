using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLineLoadScene : MonoBehaviour {
    public string nextScene;
	// Use this for initialization
	void Start () {
        if (GameMgr.instance)//联网状态
        {
            GameMgr.instance.nextScene = nextScene;
            SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
