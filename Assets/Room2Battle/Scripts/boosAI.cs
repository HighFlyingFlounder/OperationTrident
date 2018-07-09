using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{

    public class boosAI : MonoBehaviour,NetSyncInterface
    {
        public enum fireState
        {
            OpenFire = 1, //开火
            Idle = 0, //思考人生
            KeepFire = 2, //机枪开火
            RightFire = 3, //抬起另一只手的机枪
            KeepFireAgain = 4, //另一只手继续射击
            StopFire = 5,
            MissileLaunch = 6
        };

        protected fireState currentState = fireState.Idle;

        protected float thinkTime = 0.0f;

        [SerializeField]
        protected GameObject[] Missiles;

        [SerializeField]
        protected missilLauncher[] pos;

        [SerializeField]
        protected float radius = 5.0f;

        protected Animator animator;

        //导弹齐射完毕与否
        protected bool isShotDone = true;

        //左手前的位置
        [SerializeField]
        protected Transform leftHand;

        //右手前的位置
        [SerializeField]
        protected Transform rightHand;

        //调整转向
        protected bool beginTurnAround = false;

        protected GameObject player;

        protected NetSyncController netSyncController;

        protected float animationCurrentTime = 0.0f;


        //==================================================
        //=====================     需要同步的状态量 =======
        //==================================================
        //目标位置
        [SerializeField]
        protected Transform target;

        //动画状态相关bool值
        protected bool shoot = false;
        protected bool handup = false;
        protected bool rightHandup = false;
        protected bool shootAgain = false;
        protected bool stopFire = false;
        protected bool missilLaunch = false;

        protected float fireFromLastTime = 0.0f;

        protected float intervalBetweenShot = 0.3f;

        /// <summary>
        /// 初始化函数
        /// 初始化animator，其他玩家的信息
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();

            if (GameMgr.instance)//联机状态
            {
                if (GameMgr.instance.isMasterClient)
                {
                    Debug.Log("start");
                }
            }
        }

        void Update()
        {
            if (GameMgr.instance)
            {
                //主机才思考
                if (GameMgr.instance.isMasterClient)
                {
                    Debug.Log("think");
                    mind();
                }
                else
                {
                    Debug.Log("client");
                    //根据同步的bool设置动画
                    animator.SetBool("shoot", shoot);
                    animator.SetBool("handup", handup);
                    animator.SetBool("rightHandup", rightHandup);
                    animator.SetBool("shootAgain", shootAgain);
                    animator.SetBool("StopFire", stopFire);
                    animator.SetBool("missileLaunch", missilLaunch);
                }
            }
        }



        /// <summary>
        /// 状态机
        /// </summary>
        void mind()
        {
            //只有攻击才会调用动画状态机
            {

                AnimatorTransitionInfo transitioInfo = animator.GetAnimatorTransitionInfo(0);
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                switch (currentState)
                {
                    //停住
                    case fireState.Idle:
                        {
                            Debug.Log("idle");
                            animator.SetBool("StopFire", false);

                            if (thinkTime < 2.0f)
                            {
                                thinkTime += Time.deltaTime;
                            }
                            else
                            {
                                int choice = (int)UnityEngine.Random.Range(0, 3);
                                if (GameMgr.instance)
                                {
                                    //随机搞
                                    //target = (players[UnityEngine.Random.Range(0,players.Count)] as GameObject).transform;
                                    target = (SceneNetManager.instance.list[GameMgr.instance.id]).transform;
                                }
                                //开始抬手
                                switch (choice)
                                {
                                    case 0:
                                        {
                                            thinkTime = 0.0f;
                                        }
                                        break;
                                    case 1:
                                        {
                                            //转移到下一个状态
                                            currentState = fireState.OpenFire;
                                            //同步
                                            handup = true;
                                            animator.SetBool("handup", true);
                                            Debug.Log("SyncVariables");
                                            netSyncController.SyncVariables();
                                            //充值思考时间
                                            thinkTime = 0.0f;
                                        }
                                        break;
                                    case 2:
                                        {
                                            //转移到下一个状态
                                            currentState = fireState.MissileLaunch;
                                            //同步
                                            animator.SetBool("missileLaunch", true);
                                            missilLaunch = true;
                                            Debug.Log("SyncVariables");
                                            netSyncController.SyncVariables();
                                            //充值思考时间
                                            thinkTime = 0.0f;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    //抬起手为止
                    case fireState.OpenFire:
                        {
                            Debug.Log("openfire");

                            //切换完毕了
                            if (stateInfo.IsName("shoot"))
                            {
                                if (fireFromLastTime > intervalBetweenShot)
                                {
                                    //开火
                                    leftHandFireImpl();
                                    netSyncController.RPC(this, "leftHandFireImpl");
                                    fireFromLastTime = 0.0f;
                                }
                                else
                                {
                                    fireFromLastTime += Time.deltaTime;
                                }
                                //动画状态转移，同步
                                if (stateInfo.normalizedTime >= 0.8f)
                                {
                                    animator.SetBool("handup", false);
                                    animator.SetBool("shoot", true);

                                    handup = false;
                                    shoot = true;

                                    Debug.Log("SyncVariables");
                                    netSyncController.SyncVariables();
                                    //转移状态
                                    currentState = fireState.KeepFire;
                                }
                            }
                        }
                        break;
                    //抬手到播完再换手
                    case fireState.KeepFire:
                        {
                            Debug.Log("fire");
                            if (stateInfo.IsName("keepShooting"))
                            {
                                //开火
                                if (fireFromLastTime > intervalBetweenShot)
                                {
                                    //开火
                                    leftHandFireImpl();
                                    netSyncController.RPC(this, "leftHandFireImpl");
                                    fireFromLastTime = 0.0f;
                                }
                                else
                                {
                                    fireFromLastTime += Time.deltaTime;
                                }
                                //直到开火完毕，抬起另一只手
                                //if (stateInfo.normalizedTime >= 0.8f)
                                if (animationCurrentTime >= 2.0f)
                                {
                                    animator.SetBool("rightHandup", true);
                                    animator.SetBool("shoot", false);

                                    rightHandup = true;
                                    shoot = false;
                                    //同步
                                    Debug.Log("SyncVariables");
                                    netSyncController.SyncVariables();
                                    //下一个状态
                                    currentState = fireState.RightFire;

                                    animationCurrentTime = 0.0f;
                                }
                                else
                                {
                                    animationCurrentTime += Time.deltaTime;
                                }
                            }
                        }
                        break;
                    //另一只手抬起完成
                    case fireState.RightFire:
                        {
                            Debug.Log("fire");
                            //切换完毕了
                            if (stateInfo.IsName("shootback"))
                            {
                                if (fireFromLastTime > intervalBetweenShot)
                                {
                                    //开火
                                    rightHandFireImpl();
                                    netSyncController.RPC(this, "rightHandFireImpl");
                                    fireFromLastTime = 0.0f;
                                }
                                else
                                {
                                    fireFromLastTime += Time.deltaTime;
                                }
                                //开火
                                Debug.Log("fire");
                                if (stateInfo.normalizedTime >= 0.8f)
                                {
                                    animator.SetBool("rightHandup", false);
                                    animator.SetBool("shootAgain", true);

                                    rightHandup = false;
                                    shootAgain = true;
                                    //同步
                                    Debug.Log("SyncVariables");
                                    netSyncController.SyncVariables();
                                    //下一个状态
                                    currentState = fireState.KeepFireAgain;
                                }
                            }
                        }
                        break;
                    case fireState.KeepFireAgain:
                        {
                            Debug.Log("fire");
                            if (stateInfo.IsName("keepShootingBack"))
                            {
                                if (fireFromLastTime > intervalBetweenShot)
                                {
                                    //开火
                                    rightHandFireImpl();
                                    netSyncController.RPC(this, "rightHandFireImpl");
                                    fireFromLastTime = 0.0f;
                                }
                                else
                                {
                                    fireFromLastTime += Time.deltaTime;
                                }
                                //直到开火完毕
                                //if (stateInfo.normalizedTime >= 0.8f)
                                if (animationCurrentTime >= 2.0f)
                                {
                                    animator.SetBool("StopFire", true);
                                    animator.SetBool("shootAgain", false);

                                    stopFire = true;
                                    shoot = false;
                                    //同步
                                    Debug.Log("SyncVariables");
                                    netSyncController.SyncVariables();

                                    currentState = fireState.StopFire;

                                    animationCurrentTime = 0.0f;
                                }
                                else
                                {
                                    animationCurrentTime += Time.deltaTime;
                                }
                            }
                        }
                        break;
                    case fireState.StopFire:
                        {
                            Debug.Log("stop");
                            if (stateInfo.IsName("idle"))
                            {
                                Debug.Log("end of fire");
                                if (!beginTurnAround)
                                {
                                    StartCoroutine(turnAround());
                                    currentState = fireState.Idle;
                                }
                            }
                        }
                        break;
                    case fireState.MissileLaunch:
                        {
                            Debug.Log("aunch");
                            if (stateInfo.IsName("missileLaunch"))
                            {
                                if (stateInfo.normalizedTime >= 0.8f)
                                {
                                    animator.SetBool("missileLaunch", false);

                                    missilLaunch = false;
                                    //同步
                                    netSyncController.SyncVariables();

                                    missileLaunchImpl(target.position);
                                    netSyncController.RPC(this, "missileLaunchImpl", target.position);

                                    currentState = fireState.StopFire;
                                }
                            }
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        public void missileLaunchImpl(Vector3 positon)
        {
            foreach (missilLauncher a in pos)
            {
                a.SetTargetPostion(positon);
                a.launch();
            }
        }

        /// <summary>
        /// 开枪
        /// </summary>
        public void leftHandFireImpl()
        {
            Instantiate(Missiles[0], leftHand.position, transform.rotation);
        }

        public void rightHandFireImpl()
        {
            Instantiate(Missiles[0], rightHand.position, transform.rotation);
        }

        //转向玩家
        IEnumerator turnAround()
        {
            beginTurnAround = true;

            Transform temp = transform;
            Quaternion originRotation = transform.rotation;

            temp.LookAt(target);

            Vector3 newAngle = new Vector3(0.0f, temp.eulerAngles.y, 0.0f);


            Quaternion newRotatio = Quaternion.Euler(newAngle);

            float totalTime = 0.0f;
            while (totalTime < 1.0f)
            {
                transform.rotation = Quaternion.Lerp(originRotation, newRotatio, totalTime / 1.0f);
                totalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.eulerAngles = newAngle;

            beginTurnAround = false;
        }

        /// <summary>
        /// 动画时改变位置
        /// </summary>
        void OnAnimatorMove()
        {
            //主机移动，别人同步
            if (GameMgr.instance)
            {
                if (GameMgr.instance.isMasterClient)
                {
                    randomTurnImpl();
                }
            }
            else
            {
                randomTurnImpl();
            }
        }

        /// <summary>
        /// 扫射时随机转动
        /// </summary>
        public void randomTurnImpl()
        {
            switch (currentState)
            {
                case fireState.OpenFire:
                case fireState.KeepFire:
                    transform.Rotate(transform.up, UnityEngine.Random.Range(-1.0f, 2.0f));
                    break;
                case fireState.RightFire:
                case fireState.KeepFireAgain:
                    transform.Rotate(transform.up, UnityEngine.Random.Range(-3.0f, 1.0f));
                    break;
            }
        }

        public void RecvData(SyncData data)
        {
            shoot = (bool)data.Get(typeof(bool));
            handup = (bool)data.Get(typeof(bool));
            rightHandup = (bool)data.Get(typeof(bool));
            shootAgain = (bool)data.Get(typeof(bool));
            stopFire = (bool)data.Get(typeof(bool));
            missilLaunch = (bool)data.Get(typeof(bool));
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            data.Add(shoot);
            data.Add(handup);
            data.Add(rightHandup);
            data.Add(shootAgain);
            data.Add(stopFire);
            data.Add(missilLaunch);
            return data;
        }

        public void Init(NetSyncController controller)
        {
            netSyncController = controller;
        }
    }
}