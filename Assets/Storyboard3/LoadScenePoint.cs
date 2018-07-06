using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenePoint : MonoBehaviour {
    public string nextScene = "SpaceBattle";

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }
}
