using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour {
    public float Hp = 100.0f;
    private GameObject Flyer;
    void Start()
    {
        Flyer = transform.parent.gameObject;
    }

    void ChangeHp(float x)
    {
        Hp += x;
        Hp = Mathf.Clamp(Hp, 0f, 100f);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Hinder")
        {
            ChangeHp(-10.0f);
            Vector3 Force = this.transform.position - other.transform.position;
            //Flyer.GetComponent<UserController>().m_RigidBody.velocity = Force * 2f;//往反方向推
            Flyer.GetComponent<UserController>().m_RigidBody.velocity = new Vector3(0f, 0f, 0f);
            Flyer.GetComponent<UserController>().m_RigidBody.AddForce(Force * 100f);//往反方向推
            Flyer.GetComponent<UserController>().movementSettings.isPushed = true;
        }
        else if (other.collider.tag == "BigHinder")
        {
            ChangeHp(-100.0f);
        }
        if (Hp == 0f)
        {
            Debug.Log("GameOver!!!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "End")
        {
            Flyer.GetComponent<UserController>().enabled = false;
            Flyer.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        }
    }
}