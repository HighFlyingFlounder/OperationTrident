using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    /// <summary>
    /// 一个简单的状态机，只有发射和待发射两个状态
    /// 通过isLaunching标记当前状态
    /// </summary>

    public class missilLauncher : MonoBehaviour
    {
        //导弹出口
        [SerializeField]
        protected Transform rocketInitPos;

        //导弹预设
        [SerializeField]
        protected GameObject missilePrefabs;

        //目标位置
        public Transform targetTransform;

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

        public void SetTargetPostion(Transform pos)
        {
            //不能中途修改目标
            if(!isLaunching)
                targetTransform = pos;
        }


        public void launch()
        {
            if (!isLaunching)
            {
                //将t挂载到tempLauncherTransform的子节点上
                Transform tempLauncherTransform = transform;
                Transform t = transform.Find("Rocket launcher");

                //保存旧的
                launcherEulerBefore = transform.eulerAngles;
                //父节点lookat
                tempLauncherTransform.LookAt(targetTransform.position);
                
                //记录父节点新的
                launcherEulerAfter = new Vector3(270, tempLauncherTransform.eulerAngles.y, 0);
                launchQuaternion = tempLauncherTransform.rotation;
                tempLauncherTransform.eulerAngles = launcherEulerAfter;

                rocketRotationBefore = t.rotation;
                t.LookAt(targetTransform.position, t.up);
                t.localEulerAngles = new Vector3(t.localEulerAngles.x + 90, t.localEulerAngles.y, t.localEulerAngles.z);
                rocketRotationAfter = t.rotation;

                StartCoroutine(launchImpl());
            }
        }

        protected IEnumerator launchImpl()
        {
            isLaunching = true;

            float totalTime = 0.0f;

            while (totalTime < 1.0f)
            {
                transform.eulerAngles = Vector3.Lerp(launcherEulerBefore, launcherEulerAfter, totalTime / 1.0f);

                totalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.eulerAngles = launcherEulerAfter;

            Transform t = transform.Find("Rocket launcher");

            while (totalTime < 1.0f)
            {
                 t.rotation= Quaternion.Lerp(rocketRotationBefore, rocketRotationAfter, totalTime / 1.0f);

                totalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            t.rotation = rocketRotationAfter;


            Instantiate(missilePrefabs, rocketInitPos.position, launchQuaternion);

            isLaunching = false;
        }


     
    }
}