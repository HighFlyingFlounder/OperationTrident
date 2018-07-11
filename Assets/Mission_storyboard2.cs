using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

public class Mission_storyboard2 : MonoBehaviour {
    public string[] mission;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        GUIUtil.DisplayMissionDetailDefault(mission, Camera.current, Color.white,18,0.005f,0.05f);
    }
}
