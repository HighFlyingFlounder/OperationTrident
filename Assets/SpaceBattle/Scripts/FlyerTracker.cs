using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerTracker : MonoBehaviour {
    public Transform objectToFollow;
    public Vector3 offset = new Vector3(0f, 0.7f, 0f);
    public float speed = 2.0f;

    public void setFollowObject(GameObject obj)
    {
        objectToFollow = obj.transform;
    }
    private void Start()
    {
        //offset = new Vector3(-1f,0.7f,0f);
    }

    void FixedUpdate()
    {
        if (objectToFollow != null)
        {
            float interpolation = speed * Time.deltaTime;

            Vector3 position = this.transform.position;
            position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.position.y + offset.y, interpolation);
            position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.position.x + offset.x, interpolation);
            position.z = Mathf.Lerp(this.transform.position.z, objectToFollow.position.z + offset.z, interpolation);
            this.transform.position = position;
        }
    }
}
