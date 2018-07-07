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

        }

        public override void LookAt(Vector3 interestPoint)
        {

        }

        IEnumerator LookAtImpl(Vector3 target)
        {

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
