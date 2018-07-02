using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{

    public class boosAI : MonoBehaviour
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
        protected Transform target;

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


        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            AnimatorTransitionInfo transitioInfo = animator.GetAnimatorTransitionInfo(0);
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            switch (currentState)
            {
                //停住
                case fireState.Idle:
                    {
                        animator.SetBool("StopFire", false);

                        if (thinkTime < 4.0f)
                        {
                            thinkTime += Time.deltaTime;
                        }
                        else
                        {
                            int choice = (int)Random.Range(0, 3);
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
                                        currentState = fireState.OpenFire;
                                        animator.SetBool("handup", true);
                                        thinkTime = 0.0f;

                                    }
                                    break;
                                case 2:
                                    {
                                        currentState = fireState.MissileLaunch;
                                        animator.SetBool("missileLaunch", true);
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
                        //切换完毕了
                        if (stateInfo.IsName("shoot"))
                        {
                            Instantiate(Missiles[0], leftHand.position, transform.rotation);
                            //开火
                            if (stateInfo.normalizedTime >= 0.8f)
                            {
                                animator.SetBool("handup", false);
                                animator.SetBool("shoot", true);
                                currentState = fireState.KeepFire;
                            }
                        }
                    }
                    break;
                //抬手到播完再换手
                case fireState.KeepFire:
                    {
                        Instantiate(Missiles[0], leftHand.position, transform.rotation);
                        if (stateInfo.IsName("keepShooting"))
                        {
                            //直到开火完毕，抬起另一只手
                            if (stateInfo.normalizedTime >= 0.8f)
                            {
                                animator.SetBool("rightHandup", true);
                                animator.SetBool("shoot", false);
                                currentState = fireState.RightFire;
                            }
                        }
                    }
                    break;
                //另一只手抬起完成
                case fireState.RightFire:
                    {
                        Instantiate(Missiles[0], rightHand.position, transform.rotation);
                        //切换完毕了
                        if (stateInfo.IsName("shootback"))
                        {         
                            //开火
                            Debug.Log("fire");
                            if (stateInfo.normalizedTime >= 0.8f)
                            {
                                animator.SetBool("rightHandup", false);
                                animator.SetBool("shootAgain", true);
                                currentState = fireState.KeepFireAgain;
                            }
                        }
                    }
                    break;
                case fireState.KeepFireAgain:
                    {
                        //开火
                        Debug.Log("right fire");
                        Instantiate(Missiles[0], rightHand.position, transform.rotation);
                        if (stateInfo.IsName("keepShootingBack"))
                        {   
                            //直到开火完毕
                            if (stateInfo.normalizedTime >= 0.8f)
                            {
                                animator.SetBool("StopFire", true);
                                animator.SetBool("shootAgain", false);
                                currentState = fireState.StopFire;
                            }
                        }
                    }
                    break;
                case fireState.StopFire:
                    {
                        if (stateInfo.IsName("keepShootingBack"))
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
                        if (stateInfo.IsName("missileLaunch"))
                        {
                            if (stateInfo.normalizedTime >= 1.0f)
                            {
                                animator.SetBool("missileLaunch", false);
                                foreach (missilLauncher a in pos)
                                {
                                    a.SetTargetPostion(target);
                                    a.launch();
                                }
                                currentState = fireState.Idle;
                            }
                        }
                    }
                    break;
                default:
                    return;
            }

        }
        /*
        /// <summary>
        /// 使用携程隔一段时间发一枚导弹
        /// </summary>
        /// <returns></returns>
        IEnumerator shotTogether()
        {
            isShotDone = false;
            float r = radius;
            for (int j = 0; j < 3; ++j)
            {
                r += 3;
                for (int i = 0; i < pos.Length; ++i)
                {
                    Vector3 pos_ = target.position;
                    pos_.x += Random.Range(-r, r);
                    pos_.y += Random.Range(-r, r);

                    //Quaternion targetRotation = Quaternion.LookRotation(target.position - pos[i].position);


                    //pos[i].rotation = targetRotation;
                    Transform p = pos[i];   
                    p.LookAt(pos_);

                    Vector3 vec = new Vector3(p.eulerAngles.x, p.eulerAngles.y, p.eulerAngles.z);
                    Quaternion ro = p.rotation;

                    p.eulerAngles = new Vector3(-90,p.eulerAngles.y,0);

                    Transform t = pos[i].Find("Rocket launcher");

                    t.LookAt(pos_, t.up);
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x+90, t.localEulerAngles.y, t.localEulerAngles.z);



                    Instantiate(Missiles[Random.Range(0, Missiles.Length)], p.position,ro);


                    yield return new WaitForSeconds(Random.Range(1.0f,2.0f));
                }
            }
            isShotDone = true;
        }
        */

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

        void OnAnimatorMove()
        {
            switch (currentState)
            {
                case fireState.OpenFire:
                case fireState.KeepFire:
                    transform.Rotate(transform.up, 1.0f);
                    break;
                case fireState.RightFire:
                case fireState.KeepFireAgain:
                    transform.Rotate(transform.up, -1.0f);
                    break;
            }
        }
    }
}