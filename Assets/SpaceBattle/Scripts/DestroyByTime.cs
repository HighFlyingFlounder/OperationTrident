using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour {
    public float liftTime = 0.5f;
    public float tumble = 1.5f;
    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble * Random.Range(0.0f, 1.0f);
        Destroy(gameObject, liftTime);
	}
}
