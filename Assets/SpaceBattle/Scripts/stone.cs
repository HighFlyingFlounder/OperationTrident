using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stone : MonoBehaviour {
    public float tumble = 1.0f;
    private Transform cam;

    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
        cam = GameObject.FindWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        if (transform.position.x < cam.position.x - 10f)
        {
            transform.position = cam.position + new Vector3(Random.Range(20f, 30f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
        }
    }
}
