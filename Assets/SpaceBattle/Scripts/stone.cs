using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stone : MonoBehaviour {
    public float tumble = 1.0f;
    public static Transform cam;

    private void Awake()
    {
        //cam = GameObject.FindWithTag("MainCamera").transform;
    }
    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (cam)
        {
            if (transform.position.x < cam.position.x - 10f)
            {
                transform.position = cam.position + new Vector3(Random.Range(70f, 100f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
            }
        }
    }
}
