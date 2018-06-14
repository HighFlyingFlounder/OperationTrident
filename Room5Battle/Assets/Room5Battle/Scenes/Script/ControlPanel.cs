using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour
{
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LateUpdate()
    {
        //摄像机中心发出的射线
        Ray viewRay = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0));
        RaycastHit hitInfo;
        if (Physics.Raycast(viewRay, out hitInfo, 0.5f))
        {
            if (hitInfo.)
        }
    }
}
