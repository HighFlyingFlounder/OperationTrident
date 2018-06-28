using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator
{
    public class ExplosionController : MonoBehaviour
    {
        [SerializeField]
        //爆炸的prefab
        public GameObject explosion;

        //生成爆炸的时间
        public float time = 0;

        //开始时间
        private float c_time = -1;

        bool enable = false;

        bool isStart = false;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (enable)
            {
                c_time = Time.time;
                time += c_time;

                enable = false;
                isStart = true;
            }

            if (c_time >= 0)
            {
                c_time += Time.deltaTime;
            }

            if (c_time >= time && isStart)
            {
                //生成物体
                Instantiate(explosion, this.transform.position, this.transform.rotation);
                isStart = false;
            }
        }

        void Operate()
        {
            enable = true;
        }

        private void Awake()
        {
            //开始
            Messenger.AddListener(GameEvent.Enemy_Start, Operate);
        }

        private void Destroy()
        {
            //移除
            Messenger.RemoveListener(GameEvent.Enemy_Start, Operate);
        }
    }
}
