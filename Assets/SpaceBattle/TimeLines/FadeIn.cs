using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

public class FadeIn : MonoBehaviour {
    FadeInOutUtil fade;
    public float duration = 0.5f;

    // Use this for initialization
    void Start () {
        fade.FadeOut(duration, Color.black);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
