using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLineLoadScene : MonoBehaviour {
    public string nextScene = "SpaceBattle";
	// Use this for initialization
	void Start () {
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
