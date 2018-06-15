using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    public GameObject child1;
    public GameObject child2;

	// Use this for initialization
	void Start () {
        child1 = GameObject.Find("mesh1");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
