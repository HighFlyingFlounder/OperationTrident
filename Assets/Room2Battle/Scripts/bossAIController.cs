using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OperationTrident.Common.AI;
using System;

namespace room2Battle
{
    public class bossAIController : AIActionController
    {
        [SerializeField]
        protected GameObject[] bulletPrefabs;

        //导弹发射器
        [SerializeField]
        protected missilLauncher[] launcher;
        //动画
        protected Animator animator;

        //左手前的位置
        [SerializeField]
        protected Transform leftHand;

        //右手前的位置
        [SerializeField]
        protected Transform rightHand;

        protected Vector3 targetPos;

        [SerializeField]
        protected float rotateSpeed = 1.0f;

        public override IEnumerator Destroy()
        {
            yield return new WaitForSeconds(1.0f);
            Destroy(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();

            StartCoroutine(LookAtImpl());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void LookAt(Vector3 interestPoint)
        {
            targetPos = interestPoint;
        }

        /// <summary>
        /// 虽为携程，却写着update二字，持续算lookat角度并进行更新
        /// </summary>
        /// <returns></returns>
        IEnumerator LookAtImpl()
        {
            //每帧更新
            while (true)
            {
                //防止为空
                if (targetPos != null)
                {
                    //利用lookat，算出绕Y轴旋转的角度，不考虑X,Z轴旋转
                    Transform temp = transform;
                    Quaternion originRotation = transform.rotation;

                    temp.LookAt(targetPos, transform.up);

                    Vector3 newAngle = new Vector3(0.0f, temp.eulerAngles.y, 0.0f);

                    Quaternion newRotation = Quaternion.Euler(newAngle);
                    //一直一点点地旋转
                    transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
                }
                    yield return new WaitForFixedUpdate();
            }
        }

        public override void Move(bool isStart)
        {
            if (isStart)
                animator.SetFloat("speed", 0.3f);
            else
                animator.SetFloat("speed", 0.0f);
        }
    }
}
