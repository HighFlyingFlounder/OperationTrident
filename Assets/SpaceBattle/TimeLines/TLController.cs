using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLController : MonoBehaviour {
    public int num = 0;
    public GameObject[] model;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (num != 0)
        {
            for(int i = 0; i < 4 - num; i++)
            {
                model[3 - i].SetActive(false);
            }
            enabled = false;
        }
	}
}
