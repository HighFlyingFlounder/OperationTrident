using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour {
    public float liftTime = 1f;
    // Use this for initialization
    void Start () {
        Destroy(gameObject, liftTime);
	}
}
