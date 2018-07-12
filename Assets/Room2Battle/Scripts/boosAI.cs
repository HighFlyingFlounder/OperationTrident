using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OperationTrident.Util;
using OperationTrident.FPS.Common;

namespace room2Battle
{

    public class boosAI : MonoBehaviour, NetSyncInterface
    {
        //状态名
        public enum fireState
        {
            OpenFire = 1, //开火
            Idle = 0, //思考人生
            KeepFire = 2, //机枪开火
            RightFire = 3, //抬起另一只手的机枪
            KeepFireAgain = 4, //另一只手继续射击
            StopFire = 5,
            MissileLaunch = 6,
            SeekingPlayer = 7,
            wandering = 8
        };

        protected fireState currentState = fireState.Idle;
        //思考时间，模拟人类
        protected float thinkTime = 0.0f;
        //发射的子弹
        [SerializeField]
        protected GameObject[] Bullets;
        //导弹发射器
        [SerializeField]
        protected missilLauncher[] launchers_;

        protected Animator animator;

        //导弹齐射完毕与否
        protected bool isShotDone = true;

        //左手前的位置
        [SerializeField]
        protected Transform leftHand;

        //右手前的位置
        [SerializeField]
        protected Transform rightHand;

        //AI的头部
        [SerializeField]
        protected Transform head;

        //绕圈圈的路径
        [SerializeField]
        protected Transform[] wanderPath;

        //调整转向
        protected bool beginTurnAround = false;
        //同步器
        protected NetSyncController netSyncController;
        //动画播放相关
        protected float animationCurrentTime = 0.0f;
        //是否发现玩家
        protected bool isFoundPlayer = false;

        protected GetCamera getCamera;

        //==================================================
        //=====================     需要同步的状态量 =======
        //==================================================
        //目标位置
        [SerializeField]
        protected Transform target = null;

        //动画状态相关bool值
        protected bool shoot = false;
        protected bool handup = false;
        protected bool rightHandup = false;
        protected bool shootAgain = false;
        protected bool stopFire = false;
        protected bool missilLaunch = false;
        protected bool isWalking = false;

        protected float fireFromLastTime = 0.0f;

        protected float intervalBetweenShot = 0.15f;

        protected bool isStartWalking = false;

        protected bool isBeginWandering = false;

        protected Transform currentWanderTarget;

        /// <summary>
        /// 初始化函数
        /// 初始化animator，其他玩家的信息
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();

            if (GameMgr.instance)
            {
                if (SceneNetManager.instance.list.Count != 0)
                {
                    getCamera = SceneNetManager.instance.list[GameMgr.instance.id].GetComponent<GetCamera>();
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
                    mind();
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log("client");
#endif
                    //根据同步的bool设置动画
                    animator.SetBool("shoot", shoot);
                    animator.SetBool("handup", handup);
                    animator.SetBool("rightHandup", rightHandup);
                    animator.SetBool("shootAgain", shootAgain);
                    animator.SetBool("StopFire", stopFire);
                    animator.SetBool("missileLaunch", missilLaunch);
                    animator.SetBool("walk", isWalking);
                }
            }
        }



        /// <summary>
        /// 状态机
        /// </summary>
        void mind()
        {
            {
                //获取动画状态
                AnimatorTransitionInfo transitioInfo = animator.GetAnimatorTransitionInfo(0);
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                switch (currentState)
                {
                    //停住
                    case fireState.Idle:
                        {
                            //一定要转到idle动画为止
                            animator.SetBool("StopFire", false);
                            //转完再说
                            if (beginTurnAround)
                                break;
                            //想一下

                            if (thinkTime < 0.5f)
                            {
                                thinkTime += Time.deltaTime;
                            }
                            else
                            {
                                thinkTime = 0.0f;
                                currentState = fireState.SeekingPlayer;
                            }
                        }
                        break;
                    //看看谁是下一个倒霉蛋
                    case fireState.SeekingPlayer:
                        {
                            isFoundPlayer = false;
                            foreach (var a in SceneNetManager.instance.list)
                            {                        
                                if (a.Value != null)
                                {
                                    //用头比较清真
                                    Vector3 dir = (a.Value.transform.Find("Head").position - new Vector3(0.0f,0.5f,0.0f)) - head.position;
                                    Ray ray = new Ray(head.position, dir);

                                    RaycastHit hit;
                                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                                    {
#if UNITY_EDITOR
                                        Debug.DrawLine(ray.origin, hit.point, Color.red, 2.0f);
                                        Debug.Log(hit.collider.tag);
#endif
                                        if (hit.collider.tag == "Player")
                                        {
                                            isFoundPlayer = true;
                                            target = a.Value.transform;
#if UNITY_EDITOR
                                            Debug.Log(target);
#endif
                                            break;
                                        }
                                    }
                                }
                            }
                            //找到了
                            if (isFoundPlayer)
                            {
                                int choice = (int)UnityEngine.Random.Range(0, 2);
                                switch (choice)
                                {
                                    case 0:
                                        {
                                            //转移到下一个状态
                                            currentState = fireState.OpenFire;
                                            handup = true;
                                            animator.SetBool("handup", true);
                                            //netSyncController.SyncVariables();
                                        }
                                        break;
                                    case 1:
                                        {
                                            //转移到下一个状态
                                            currentState = fireState.MissileLaunch;
                                            //同步
                                            animator.SetBool("missileLaunch", true);
                                            missilLaunch = true;
                                            //netSyncController.SyncVariables();
                                        }
                                        break;
                                }
                            }
                            //维持seek状态2秒
                            if (thinkTime >= 2.0f)
                            {
                                thinkTime = 0.0f;
                                currentState = fireState.wandering;
                            }
                            else
                            {
                                thinkTime += Time.deltaTime;
                            }
                        }
                        break;
                    case fireState.wandering:
                        {
                            //TODO:之前状态要加一下isStartWalking的初始化
                            //先转过去再走
                            if (!isStartWalking)
                            {
                                //选个目标
                                currentWanderTarget = wanderPath[UnityEngine.Random.Range(0, wanderPath.Length)];
                                StartCoroutine(turnAround(currentWanderTarget));
                                isStartWalking = true;
                            }
                            else
                            {
                                //转到位了
                                if (!beginTurnAround)
                                {
                                    if (!isBeginWandering)
                                    {
                                        animator.SetBool("walk", true);
                                        isWalking = true;
                                        //netSyncController.SyncVariables();
                                        isBeginWandering = true;
                                    }
                                    else
                                    {
                                        if (Vector3.Distance(transform.position, currentWanderTarget.position) < 10.0f)
                                        {
                                            isStartWalking = false;
                                            isBeginWandering = false;

                                            isWalking = false;
                                            animator.SetBool("walk", false);
                                            //netSyncController.SyncVariables();
                                            currentState = fireState.SeekingPlayer;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    //抬起手为止
                    case fireState.OpenFire:
                        {
                            //切换完毕了
                            if (stateInfo.IsName("shoot"))
                            {
                                //动画状态转移，同步
                                if (stateInfo.normalizedTime >= 0.5f)
                                {
                                    animator.SetBool("handup", false);
                                    animator.SetBool("shoot", true);

                                    handup = false;
                                    shoot = true;

                                    //netSyncController.SyncVariables();
                                    //转移状态
                                    currentState = fireState.KeepFire;
                                }
                            }
                        }
                        break;
                    //抬手到播完再换手
                    case fireState.KeepFire:
                        {
                            if (stateInfo.IsName("keepShooting"))
                            {
                                //开火
                                if (fireFromLastTime > intervalBetweenShot)
                                {
                                    if (target != null)
                                    {
                                        //开火
                                        leftHandFireImpl(target.transform.Find("Head").position - new Vector3(0.0f, 0.5f, 0.0f));
                                        netSyncController.RPC(this, "leftHandFireImpl", target.transform.Find("Head").position - new Vector3(0.0f, 0.5f, 0.0f));
                                        fireFromLastTime = 0.0f;
                                    }
                                }
                                else
                                {
                                    fireFromLastTime += Time.deltaTime;
                                }
                                //直到开火完毕，抬起另一只手
                                if (animationCurrentTime >= 2.0f)
                                {
                                    animator.SetBool("rightHandup", true);
                                    animator.SetBool("shoot", false);

                                    rightHandup = true;
                                    shoot = false;
                                    //同步
                                    //netSyncController.SyncVariables();
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
                            //切换完毕了
                            if (stateInfo.IsName("shootback"))
                            {
                                //开火
                                if (stateInfo.normalizedTime >= 0.5f)
                                {
                                    animator.SetBool("rightHandup", false);
                                    animator.SetBool("shootAgain", true);

                                    rightHandup = false;
                                    shootAgain = true;
                                    //同步
                                    //netSyncController.SyncVariables();
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
                                    if (target != null)
                                    {
                                        //开火
                                        rightHandFireImpl(target.transform.Find("Head").position - new Vector3(0.0f, 0.5f, 0.0f));
                                        netSyncController.RPC(this, "rightHandFireImpl", target.transform.Find("Head").position - new Vector3(0.0f, 0.5f, 0.0f));
                                        fireFromLastTime = 0.0f;
                                    }
                                }
                                else
                                {
                                    fireFromLastTime += Time.deltaTime;
                                }
                                //直到开火完毕
                                if (animationCurrentTime >= 2.0f)
                                {
                                    animator.SetBool("StopFire", true);
                                    animator.SetBool("shootAgain", false);

                                    stopFire = true;
                                    shoot = false;
                                    //netSyncController.SyncVariables();

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
                                    StartCoroutine(turnAround(target));
                                    currentState = fireState.Idle;
                                }
                            }
                        }
                        break;
                    case fireState.MissileLaunch:
                        {
                            if (stateInfo.IsName("missileLaunch"))
                            {
                                if (stateInfo.normalizedTime >= 0.8f)
                                {

                                    animator.SetBool("missileLaunch", false);

                                    missilLaunch = false;
                                    //同步
                                    //netSyncController.SyncVariables();
                                    if (target != null)
                                    {
                                        missileLaunchImpl(target.position);
                                        netSyncController.RPC(this, "missileLaunchImpl", target.position);
                                    }
                                    currentState = fireState.StopFire;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            bool[] states = {
                shoot,
                handup,
                rightHandup,
                shootAgain,
                stopFire,
                missilLaunch,
                isWalking
            };
            netSyncController.RPC(this, "updateAState_Room2", states);
        }

        /// <summary>
        /// @brief 打导弹
        /// </summary>
        /// <param name="positon"></param>
        public void missileLaunchImpl(Vector3 positon)
        {
            foreach (missilLauncher a in launchers_)
            {
                a.SetTargetPostion(positon);
                a.launch();
            }
        }

        /// <summary>
        /// 开枪
        /// </summary>
        public void leftHandFireImpl(Vector3 target_)
        {
            GameObject obj = Instantiate(Bullets[0], leftHand.position, transform.rotation);
            obj.transform.position = leftHand.position;
            obj.transform.up = (target_ - obj.transform.position);
        }

        public void rightHandFireImpl(Vector3 target_)
        {
            GameObject obj = Instantiate(Bullets[0], rightHand.position, transform.rotation);
            obj.transform.position = rightHand.position;
            obj.transform.up = (target_ - obj.transform.position);
        }

        //转向玩家
        IEnumerator turnAround(Transform target_)
        {
            beginTurnAround = true;

            Transform temp = transform;
            Quaternion originRotation = transform.rotation;

            if (target_ != null)
            {
                temp.LookAt(target_);

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
            }
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
                    fireTurnImpl();
                }
            }
            else
            {
                fireTurnImpl();
            }
        }

        /// <summary>
        /// 扫射时随机转动
        /// </summary>
        public void fireTurnImpl()
        {
            switch (currentState)
            {
                case fireState.OpenFire:
                case fireState.KeepFire:
                case fireState.RightFire:
                case fireState.KeepFireAgain:
                    Transform temp = transform;
                    if(target != null)
                        temp.LookAt(target);
                    Quaternion newRotation = Quaternion.Euler(0.0f, temp.eulerAngles.y, 0.0f);
                    //转向目标
                    transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime);
                    break;
                case fireState.wandering:
                    {
                        if (isStartWalking && !beginTurnAround)
                        {
                            transform.position = Vector3.Lerp(transform.position, currentWanderTarget.position, Time.deltaTime * 0.2f);
                        }
                    }
                    break;
            }
        }

        public void RecvData(SyncData data)
        { 
        }

        public SyncData SendData()
        {
            return null;
        }

        public void updateAState_Room2(bool[] states)
        {
            shoot = states[0];
            handup = states[1];
            rightHandup = states[2];
            shootAgain = states[3];
            stopFire = states[4];
            missilLaunch = states[5];
            isWalking = states[6];
        }

        public void Init(NetSyncController controller)
        {
            netSyncController = controller;
        }

        void OnGUI()
        {
            //if (getCamera != null && getCamera.GetCurrentUsedCamera() != null)
            //{
            //    Camera cam = getCamera.GetCurrentUsedCamera();
            //    Rect rect = new Rect(cam.pixelWidth * 0.4f, 0, 100, 100);
            //    GUIStyle style = GUIUtil.GetDefaultTextStyle(Color.red, 10);

            //    if (missilLaunch)
            //    {
            //        GUIUtil.DisplayContentInGivenPosition("WARNING:MISSILE!",
            //                rect,
            //                style
            //            );
            //        return;SS
            //    }
            //    else if (shoot || shootAgain || handup || rightHandup)
            //    {
            //        GUIUtil.DisplayContentInGivenPosition("WARNING:MACHINEGUN!",
            //                rect,
            //                style
            //            );
            //        return;
            //    }
            //    else if (isWalking)
            //    {
            //        GUIUtil.DisplayContentInGivenPosition("SAFE",
            //                rect,
            //                style
            //            );
            //    }
            //    else
            //    {
            //        GUIUtil.DisplayContentInGivenPosition("",
            //                rect,
            //                style
            //            );
            //    }
            //}
        }
    }
}