using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Animations;
using OperationTrident.Common.AI;
using System;

namespace room2Battle
{
    public class bossAIController : AIActionController
    {
        [SerializeField]
        protected GameObject bulletPrefabs;

        //导弹发射器
        [SerializeField]
        protected missilLauncher[] launcher;
        //动画
        protected Animator animator;

        //左手前的位置
        [SerializeField]
        protected Transform leftHand;
        //目标位置
        protected Vector3 targetPos;
        //打击位置
        protected Vector3 shootingTarget;

        [SerializeField]
        protected float rotateSpeed = 1.0f;
        //之前在转身lookat的携程
        protected Coroutine previousTurn = null;
        //之前开火的shoot的携程
        protected Coroutine previousAttack = null;
        //之前打导弹的携程
        protected Coroutine previousMissileLaunch = null;

        //当前动画状态
        protected AnimatorStateInfo currentAnimationState = new AnimatorStateInfo();

        public override IEnumerator Destroy()
        {
            yield return new WaitForSeconds(1.0f);
            Destroy(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        }

        public override void LookAt(Vector3 interestPoint)
        {
            if (!currentAnimationState.IsName("default"))
            {
                animator.SetBool("stopFire", true);
            }
            //第一次转
            if (previousTurn == null)
            {
                targetPos = interestPoint;
                previousTurn = StartCoroutine(LookAtImpl());
            }
            //开始新的携程，保证只有一个携程
            else {
                StopCoroutine(previousTurn);
                targetPos = interestPoint;
                previousTurn = StartCoroutine(LookAtImpl());
            }
        }

        protected IEnumerator LookAtImpl()
        {
            //算角度
            Transform temp = transform;
            Quaternion originRotation = transform.rotation;

            temp.LookAt(targetPos, transform.up);

            Vector3 newAngle = new Vector3(0.0f, temp.eulerAngles.y, 0.0f);

            Quaternion newRotation = Quaternion.Euler(newAngle);
            //直到回到默认状态
            while (!currentAnimationState.IsName("default"))
            {
                yield return new WaitForFixedUpdate();
            }

            animator.SetBool("stopFire", false);
            animator.SetBool("startShooting", true);
            //每帧更新
            float time = 0.0f;
            while (time <= 1.5f)
            {
                //防止为空
                if (targetPos != null)
                {
                    //一直一点点地旋转
                    transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, time / 1.5f);
                }
                yield return new WaitForFixedUpdate();
                time += Time.deltaTime;
            }
            transform.rotation = newRotation;
            animator.SetBool("startShooting", false);

        }

        public override void Shoot(Vector3 shootingPoint)
        {
            if (!currentAnimationState.IsName("default"))
            {
                animator.SetBool("stopFire", true);
            }
            //第一次转
            if (previousAttack == null)
            {
                shootingTarget = shootingPoint;
                previousAttack = StartCoroutine(ShootImpl());
            }
            else
            {
                StopCoroutine(previousAttack);
                shootingTarget = shootingPoint;
                previousTurn = StartCoroutine(ShootImpl());
            }
        }

        protected IEnumerator ShootImpl()
        {
            
            //等待回归默认状态
            while (!currentAnimationState.IsName("default"))
            {
                yield return new WaitForFixedUpdate();
            }

            animator.SetBool("stopFire", false);
            animator.SetBool("openFire", true);

            float time = 0.0f;

            while (time <= 3.0f)
            {
                transform.Rotate(transform.up, UnityEngine.Random.Range(-2.0f, 2.0f));

                GameObject bullet_ = Instantiate(bulletPrefabs, leftHand.position, transform.rotation);
                //设置子弹的方向
                bullet_.transform.LookAt(shootingTarget, bullet_.transform.up);

                time += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            animator.SetBool("openFire", false);
        }

        public void MissileLaunch(Vector3 shootingPoint)
        {
            if (!currentAnimationState.IsName("default"))
            {
                animator.SetBool("stopFire", true);
            }
            //第一次转
            if (previousMissileLaunch == null)
            {
                shootingTarget = shootingPoint;
                previousMissileLaunch = StartCoroutine(MissileLaunchImpl());
            }
            //开始新的携程，保证只有一个携程
            else
            {
                StopCoroutine(previousMissileLaunch);
                shootingTarget = shootingPoint;
                previousTurn = StartCoroutine(MissileLaunchImpl());
            }
        }

        protected IEnumerator MissileLaunchImpl()
        {
            //等待回归默认状态
            while (!currentAnimationState.IsName("default"))
            {
                yield return new WaitForFixedUpdate();
            }

            animator.SetBool("stopFire", false);
            animator.SetBool("missileLaunch", true);

            foreach (missilLauncher launch in launcher)
            {
                launch.SetTargetPostion(shootingTarget);
                launch.launch();
            }
            animator.SetBool("missileLaunch", false);
        }

        public override void FindTarget(bool isStart)
        {
            if (isStart)
            {
                animator.SetBool("seeking", true);
            }
            else
            {
                animator.SetBool("seeking", false);
            }
        }

        public override void Move(bool isStart)
        {
            if (isStart)
                animator.SetBool("isWalk", true);
            else
                animator.SetBool("isWalk", false);
        }
    }
}
