using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLineLoadScene : MonoBehaviour {
    public string nextScene;
	// Use this for initialization
	void Start () {
        GameMgr.instance.nextScene = nextScene;
        SceneManager.LoadScene("Loading", LoadSceneMode.Single);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
