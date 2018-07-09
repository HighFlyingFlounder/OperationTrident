using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class God_TimeLine : MonoBehaviour {
    //public string nextScene = "SpaceBattle";
    private GameObject timeLine;
    public GameObject manager;

    // Use this for initialization
    void Start () {
        timeLine = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            timeLine.SetActive(false);
            manager.GetComponent<SpaceBattleManager>().enabled = true;
            this.enabled = false;
        }
    }
}
