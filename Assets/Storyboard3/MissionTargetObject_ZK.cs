using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

public class MissionTargetObject_ZK : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        GUIUtil.DisplayMissionPoint(this.transform.position, Camera.current, Color.white);
    }
}
