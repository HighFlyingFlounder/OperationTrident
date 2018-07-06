using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLController : MonoBehaviour {
    public int num = 1;
    private int count;
    public GameObject[] model;

	// Use this for initialization
	void Start () {
        num -= 1;
        count = model.Length;
	}
	
	// Update is called once per frame
	void Update () {
        if (num != 0)
        {
            for(int i = 0; i < count - num; i++)//
            {
                model[count -1 - i].SetActive(false);//
            }
            enabled = false;
        }
	}
}
