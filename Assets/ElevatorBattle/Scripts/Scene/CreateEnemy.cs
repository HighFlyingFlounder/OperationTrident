using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator
{
    public class CreateEnemy : MonoBehaviour, NetSyncInterface
    {
        [SerializeField]
        //要生成的物体
        public GameObject CreateObject;
        //要生成的数量
        public int number;
        //生成速度(间隔)
        public float speed;
        //开始生成时间
        public float time;
        //是否开始生成物体
        public bool isStart;

        //协程
        private IEnumerator coroutine;
        //维护一个物体列表
        private List<GameObject> list;
        //开始时间
        private float s_time;
        //现在时间
        private float c_time;
        //结束时间
        private float e_time;

        NetSyncController m_controller;

        // Use this for initialization
        void Start() {
            //一秒执行一次
            coroutine = WaitAndPrint(speed);
            list = new List<GameObject>();
        }

        // Update is called once per frame
        void Update() {
            //判断何时生成
            c_time += Time.deltaTime;
            if (c_time >= e_time)
            {
                if (isStart)
                {
                    //开始生成
                    StartCoroutine(coroutine);
                    isStart = false;
                }

                if (list.Count % number == 0)
                {
                    //停止生成器
                    StopCoroutine(coroutine);
                    //留种
                    coroutine = WaitAndPrint(speed);
                }
            }
        }

        //协程coroutine的存在，配合下面的yield return回调，这段代码将被1s执行一次
        private IEnumerator WaitAndPrint(float waitTime)
        {
            for (int i = 0; i < number;)
            {
                //生成物体
                GameObject o = Instantiate(CreateObject, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                o.name = list.Count.ToString();
                list.Add(o);
                i++;
                yield return new WaitForSeconds(waitTime);
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

        //RPC
        private void Operate(int id)
        {
            Operate_Imp();
            m_controller.RPC(this, "Operate_Imp");
        }

        public void Operate_Imp()
        {
            s_time = Time.time;
            c_time = s_time;
            e_time = s_time + time;
            isStart = true;
        }

        public void End(int id)
        {
            foreach (GameObject enemy in list)
            {
                
            }
        }

        private void Awake()
        {
            //开始生成敌人
            Messenger<int>.AddListener(GameEvent.Enemy_Start, Operate);
            //开始销毁所有敌人 
            Messenger<int>.AddListener(GameEvent.End, End);
        }

        private void Destroy()
        {
            //移除
            Messenger<int>.RemoveListener(GameEvent.Enemy_Start, Operate);
            Messenger<int>.AddListener(GameEvent.End, End);
        }
    }
}
