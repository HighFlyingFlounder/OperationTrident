using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    public static Manager instance;
    public bool GameOver = false;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameOver)
        {
            Application.Quit();
        }
	}
}
