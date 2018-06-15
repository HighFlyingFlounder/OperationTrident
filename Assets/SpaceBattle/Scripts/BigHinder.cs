using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigHinder : MonoBehaviour {
    public float tumble = 1.0f;
    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
    }
}
