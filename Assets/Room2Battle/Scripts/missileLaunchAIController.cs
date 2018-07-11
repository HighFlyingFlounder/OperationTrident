using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Common.AI;

namespace Room2Battle
{
    public class missileLaunchAIController : AIActionController
    { 
        //导弹出口
        [SerializeField]
        protected Transform rocketInitPos;

        //导弹预设
        [SerializeField]
        protected GameObject missilePrefabs;

        //目标位置
        public Vector3 targetTransformPosition;

        //发射角度角度
        protected Quaternion launchQuaternion;

        //摆正前发射架的rotation
        protected Vector3 launcherEulerAfter;

        //摆正前导弹的rotation
        protected Quaternion rocketRotationAfter;

        //摆正钱发射架的rotation
        protected Vector3 launcherEulerBefore;

        //摆正后导弹的rotation
        protected Quaternion rocketRotationBefore;

        //是否正在发射
        protected bool isLaunching = false;

        [SerializeField]
        protected GameObject explosionPrefabs;

        protected Coroutine lastShoot = null;

        public override void LookAt(Vector3 interestPoint)
        {
            Transform tempLauncherTransform = transform;
            tempLauncherTransform.LookAt(interestPoint);
            Vector3 newAngle = new Vector3(270, tempLauncherTransform.eulerAngles.y, 0);

            transform.eulerAngles = newAngle;
        }

        public override void Shoot(Vector3 shootingPoint)
        {
            if (lastShoot != null)
            {
                StopCoroutine(lastShoot);
            }

            targetTransformPosition = shootingPoint;

            //将t挂载到tempLauncherTransform的子节点上
            Transform tempLauncherTransform = transform;
            Transform t = transform.Find("Rocket launcher");

            //保存旧的
            launcherEulerBefore = transform.eulerAngles;
            //父节点lookat
            tempLauncherTransform.LookAt(targetTransformPosition);

            //记录父节点新的
            launcherEulerAfter = new Vector3(270, tempLauncherTransform.eulerAngles.y, 0);
            launchQuaternion = tempLauncherTransform.rotation;
            tempLauncherTransform.eulerAngles = launcherEulerAfter;

            rocketRotationBefore = t.rotation;
            t.LookAt(targetTransformPosition, t.up);
            t.localEulerAngles = new Vector3(t.localEulerAngles.x + 90, t.localEulerAngles.y, t.localEulerAngles.z);
            rocketRotationAfter = t.rotation;

            lastShoot = StartCoroutine(launchImpl());
        }

        public override IEnumerator Destroy()
        {
            Instantiate(explosionPrefabs, transform);
            yield return new WaitForSeconds(.2f);
            Destroy(gameObject);
        }

        protected IEnumerator launchImpl()
        {
            float totalTime = 0.0f;

            while (totalTime < 0.3)
            {
                transform.eulerAngles = Vector3.Lerp(launcherEulerBefore, launcherEulerAfter, totalTime / 0.3f);

                totalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.eulerAngles = launcherEulerAfter;

            Transform t = transform.Find("Rocket launcher");

            while (totalTime < .3f)
            {
                t.rotation = Quaternion.Lerp(rocketRotationBefore, rocketRotationAfter, totalTime / .3f);

                totalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            t.rotation = rocketRotationAfter;

            Instantiate(missilePrefabs, rocketInitPos.position, launchQuaternion);
        }

    }
}
