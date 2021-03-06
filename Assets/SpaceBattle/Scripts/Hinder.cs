﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hinder : MonoBehaviour
{
    //public float radius = 10f;   //定义一个要添加爆炸力的半径
    //public float power = 4500f;   //定义一个爆炸力
    public GameObject explosion;
    //public GameObject stone;
    public float tumble = 1.0f;
    public int damage = -40;
    public static Transform cam;

    public void Boom()
    {
        //在陨石的位置实例化出爆炸

        //gameObject.SetActive(false);
        Instantiate(explosion, transform.position, transform.rotation);
        //Instantiate(stone, transform.position, transform.rotation);
        //Instantiate(stone, transform.position + new Vector3(1f, 1f, 1f), transform.rotation);
        //Instantiate(stone, transform.position + new Vector3(-1f, -1f, -1f), transform.rotation);
        //Instantiate(stone, transform.position + new Vector3(0.5f, -1f, 0.5f), transform.rotation);
        //Instantiate(stone, transform.position + new Vector3(-0.5f, 1f, -0.5f), transform.rotation);
        //Instantiate(stone, transform.position + new Vector3(0.5f, 0f, -0.5f), transform.rotation);

        ////Physics.OverlapSphere（）：球体投射，给定一个球心和半径，返回球体投射到的物体的碰撞器
        //Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        //foreach (Collider hits in colliders)  //遍历碰撞器数组
        //{
        //    //如果这个物体有刚体组件
        //    if (hits.tag == "Stone")
        //    {
        //        Debug.Log(hits.name);
        //        //给定爆炸力大小，爆炸点，爆炸半径
        //        //利用刚体组件添加爆炸力AddExplosionForce
        //        hits.GetComponent<Rigidbody>().AddExplosionForce(power, transform.position, radius);
        //    }
        //}
    }

    void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
        //cam = GameObject.FindWithTag("MainCamera").transform;
    }

    private void FixedUpdate()
    {
        if (cam)
        {
            if (transform.position.x < cam.position.x - 50f)
            {
                transform.position += new Vector3(1800f, 0f, 0f);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //只有本地玩家才会触发撞击事件并发送协议，其他玩家在本地玩家客户端是不会触发这些事件的。
        if (other.tag == "Player" && other.transform.GetComponent<NetSyncTransform>().ctrlType == NetSyncTransform.CtrlType.player)
        {
            SendHitRock();
            Boom();
        }
    }

    public void SendHitRock()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("HitRock");
        //
        proto.AddString(this.name);
        NetMgr.srvConn.Send(proto);
    }
}
