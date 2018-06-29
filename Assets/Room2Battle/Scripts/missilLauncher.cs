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
        protected Quaternion rocketEulerAfter;

        //摆正钱发射架的rotation
        protected Vector3 launcherEulerBefore;

        //摆正后导弹的rotation
        protected Quaternion rocketEulerBefore;

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
                
                Transform tempLauncherTransform = transform;
                Transform t = transform.Find("Rocket launcher");
                t.SetParent(tempLauncherTransform);

                launcherEulerBefore = transform.eulerAngles;
                rocketEulerBefore = t.rotation;
                tempLauncherTransform.LookAt(targetTransform.position);
                rocketEulerAfter = tempLauncherTransform.rotation;
                launcherEulerAfter = new Vector3(270, tempLauncherTransform.eulerAngles.y, 0);
                launchQuaternion = tempLauncherTransform.rotation;
                tempLauncherTransform.eulerAngles = new Vector3(270, tempLauncherTransform.eulerAngles.y, 0);

               
                Debug.Log(rocketEulerBefore+"================"+rocketEulerAfter);
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
                transform.Find("Rocket launcher").rotation = Quaternion.Lerp(rocketEulerBefore, rocketEulerAfter, totalTime / 1.0f);

                totalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.eulerAngles = launcherEulerAfter;
            transform.Find("Rocket launcher").rotation = rocketEulerAfter;

            Instantiate(missilePrefabs, rocketInitPos.position, launchQuaternion);

            isLaunching = false;
        }


     
    }
}