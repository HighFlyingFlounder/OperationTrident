using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace room2Battle
{
    //控制半边门
    public class elevatorDoor : MonoBehaviour
    {
        //左右移动的幅度
        [SerializeField]
        public Vector3 dPos;
        //门最初位置，由于门的左右移动在父空间，所以可以不变
        private Vector3 initPos;
        //门结束位置
        private Vector3 endPos;
        //是否正在交互
        //bool isInteractive = false;
        //门是否打开
        bool isOpen = false;
        //门是否还在打开
        bool stillOpen = false;
        /*
        public void notifyInteractive()
        {
            isInteractive = true;
        }
        */

        //@brief 计算位置
        void Start()
        {
            initPos = transform.localPosition;
            endPos = transform.localPosition - dPos;
        }

        //@brief 开门
        IEnumerator opendoorImpl()
        {
            //固定时间0.8秒开完
            float time = 0.0f;
            float total = 0.8f;
            while (time < total)
            {
                //插值
                transform.localPosition = Vector3.Lerp(initPos, endPos, time / total);
                time += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            //保证门能重合
            transform.localPosition = endPos;
            isOpen = !isOpen;
            stillOpen = false;
            //isInteractive = false;
        }
        //@brief 关门，原理和开门一样
        IEnumerator closedoorImpl()
        {
            float time = 0.0f;
            float total = 0.8f;
            while (time < total)
            {
                transform.localPosition = Vector3.Lerp(endPos, initPos, time / total);
                time += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.localPosition = initPos;
            isOpen = !isOpen;
            stillOpen = false;
            //isInteractive = false;
        }

        //@brief 暴露在外的接口，开门
        public void openTheDoor()
        {
            if (!stillOpen)
            {
                stillOpen = true;
                StartCoroutine(opendoorImpl());
            }
        }

        //@brief 外部接口，开门
        public void closeTheDoor()
        {
            if (!stillOpen)
            {
                stillOpen = true;
                StartCoroutine(closedoorImpl());
            }
        }
    }
}
