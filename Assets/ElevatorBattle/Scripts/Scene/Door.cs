﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Elevator
{
    public class Door : MonoBehaviour, NetSyncInterface
    {
        private GameObject child1;
        private GameObject child2;

        private bool open;
        private bool close;

        NetSyncController m_controller = null;

        //true: open false: close
        public static bool state;

        [SerializeField]
        public float speed;

        // Use this for initialization
        void Start()
        {
            child1 = GameObject.Find("left");
            child2 = GameObject.Find("right");

            open = false;
            close = false;
            state = false;
        }

        // Update is called once per frame
        void Update()
        {
            //关门过程结束
            if ((child1.transform.position.z <= 2.4 || child2.transform.position.z >= -2.4) && state)
            {
                state = false;
                close = false;
                GameObject.Find("d_w").GetComponent<BoxCollider>().enabled = false;
            }

            //开门过程结束
            if ((child1.transform.position.z >= 7 || child2.transform.position.z <= -7) && !state)
            {
                state = true;
                open = false;
                GameObject.Find("d_w").GetComponent<BoxCollider>().enabled = false;
            }

            //打开门
            if (open)
            {
                child1.transform.position += new Vector3(0, 0, speed * Time.deltaTime);
                child2.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
            }

            //关上门
            if (close)
            {
                child1.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
                child2.transform.position += new Vector3(0, 0, speed * Time.deltaTime);
            }

        }

        //网络同步
        public void RecvData(SyncData data)
        {
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            return data;
        }

        public void Init(NetSyncController controller)
        {
            m_controller = controller;
        }

        //发信息
        private void closeDoor()
        {
            closeDoor_Imp();
            m_controller.RPC(this, "closeDoor_Imp");
        }

        private void openDoor()
        {
            openDoor_Imp();
            m_controller.RPC(this, "openDoor_Imp");
        }

        private void Operate()
        {
            if (Elevator.SceneController.state == SceneController.ElevatorState.Initing)
            {
                openDoor();
                RayShooter.state = true;
            }
        }

        public void closeDoor_Imp()
        {
            if (state)
            {
                GameObject.Find("d_w").GetComponent<BoxCollider>().enabled = true;
                //关门
                close = true;
            }
        }

        public void openDoor_Imp()
        {
            if (!state)
            {
                GameObject.Find("d_w").GetComponent<BoxCollider>().enabled = true;
                //开门
                open = true;
            }
        }
    }
}
