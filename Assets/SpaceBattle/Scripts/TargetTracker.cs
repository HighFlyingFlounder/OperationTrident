using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour {
    private GameObject Target;
    public Camera Cam;
    public float SpeedPar;
    private Rigidbody m_RigidBody;
    
    private GameObject Flyer;

    private void Awake()
    {
        Flyer = transform.parent.gameObject;
        Target = Flyer.transform.Find("Camera/Target").gameObject;
    }
    // Use this for initialization
    void Start () {
        m_RigidBody = GetComponent<Rigidbody>();
        
    }
	
	void FixedUpdate () {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Cam.transform.rotation, 0.5f);

        float disX = Target.transform.position.x - this.transform.position.x;
        float disY = Target.transform.position.y - this.transform.position.y;
        float disZ = Target.transform.position.z - this.transform.position.z;

        Vector3 Move = new Vector3(disX, disY, disZ);
        float Speed = Move.sqrMagnitude * SpeedPar;
        Speed = Mathf.Clamp(Speed, 0.0f, 125.0f);
        Move = Move.normalized;
        m_RigidBody.velocity = Move * Speed;

    }
}
