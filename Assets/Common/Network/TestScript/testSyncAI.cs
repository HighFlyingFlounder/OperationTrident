using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSyncAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AIController.instance.createAI(2, 0, "SwopPoints");
        }
	}
}
