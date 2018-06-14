using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Room1
{
    public class DoorScript : MonoBehaviour
    {
        // 门被打开的速度
        public float openSpeed = 0.01f;

        // 门是否被打开
        bool isOpen;
        // 门是否失去了用处
        bool canBeDestroy;

        void Awake()
        {
            //Messenger.AddListener(GameEvent.DOOR1_OPEN, OpenAndDestroy);
        }

        // Use this for initialization
        void Start()
        {
            isOpen = false;
            canBeDestroy = false;
        }

        // Update is called once per frame
        void Update()
        {

            // 如果门被打开而且没被销毁，那么就慢慢的往右移
            if (isOpen && !canBeDestroy)
            {

                this.transform.localPosition = new Vector3(
                    this.transform.localPosition.x - openSpeed,
                    this.transform.localPosition.y,
                    this.transform.localPosition.z);
            }
            if (canBeDestroy) Destroy(this);
        }


        // 外界调用，可以把这个门搞定
        public void OpenAndDestroy()
        {
            isOpen = true;
            StartCoroutine(WaitForDestroy());
        }

        IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(3);
            canBeDestroy = true;
        }
    }
}