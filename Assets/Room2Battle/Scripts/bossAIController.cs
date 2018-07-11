using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using OperationTrident.Util;
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

        [SerializeField]
        protected GameObject explosionPrefab;

        protected bool isDestroy = false;

        public override IEnumerator Destroy()
        {
            isDestroy = true;
            Instantiate(explosionPrefab, transform);
            yield return new WaitForSeconds(1.0f);
            Instantiate(explosionPrefab, transform);
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

        private void OnGUI()
        {
            if (isDestroy)
            {
                if (Camera.current != null)
                    GUIUtil.DisplaySubtitleInGivenGrammar(
                        "^rboss要爆了",
                        Camera.current,
                        30,
                        0.1f,
                        1.0f);
            }
        }

        public override bool DetectPlayer(Transform player)
        {
            throw new NotImplementedException();
        }
    }
}
