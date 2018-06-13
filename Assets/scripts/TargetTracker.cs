using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour {
    private GameObject Target;
    public Camera Cam;
    public float SpeedPar;
    private Rigidbody m_RigidBody;
    private Animator m_Animator;
    private GameObject Flyer;

    // Use this for initialization
    void Start () {
        Target = GameObject.Find("Target");
        m_RigidBody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        Flyer = transform.parent.gameObject;
    }
	
	void FixedUpdate () {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Cam.transform.rotation, 0.5f);

        float disX = Target.transform.position.x - this.transform.position.x;
        float disY = Target.transform.position.y - this.transform.position.y;
        float disZ = Target.transform.position.z - this.transform.position.z;

        Vector3 Move = new Vector3(disX, disY, disZ);
        float Speed = Move.sqrMagnitude * SpeedPar;
        Speed = Mathf.Clamp(Speed, 0.0f, 50.0f);
        Move = Move.normalized;
        m_RigidBody.velocity = Move * Speed;

        Vector3 ViewPortPos = Cam.WorldToViewportPoint(this.transform.position);

        m_Animator.SetFloat("Vertical", 3 * (2 * ViewPortPos.y - 1.0f + 0.3f));
        m_Animator.SetFloat("Horizontal", 3 * (2 * ViewPortPos.x - 1.0f));
        m_Animator.SetBool("isPushed", Flyer.GetComponent<UserController>().movementSettings.isPushed);
    }
}
