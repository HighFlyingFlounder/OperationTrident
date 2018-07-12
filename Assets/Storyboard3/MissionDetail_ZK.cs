using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

public class MissionDetail_ZK : MonoBehaviour {
    public string MissionTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnGUI()
    {
        GUIUtil.DisplayMissionTargetInMessSequently(MissionTarget, Camera.current, Color.white, 0.1f);
    }
}
