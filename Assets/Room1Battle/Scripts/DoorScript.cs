using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Room1
{
    public class DoorScript : MonoBehaviour
    {
        // 标识三扇门的ID
        private static int totalId = 0;

        private int thisId;

        // 门持续开多久
        private float openSecond;

        public int ThisId
        {
            get
            {
                return thisId;
            }
        }

        // 门被打开的速度
        public float openSpeed = 0.01f;

        // 门移动的方向
        public enum DoorOpenDirection { XPositive,XNegative, YPositive,YNegative,ZPositive,ZNegative};

        private DoorOpenDirection direction;

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
            thisId = totalId++;
        }

        // Update is called once per frame
        void Update()
        {

            // 如果门被打开而且没被销毁，那么就慢慢的往右移
            if (isOpen && !canBeDestroy)
            {
                switch (direction)
                {
                    case DoorOpenDirection.XNegative:
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x - openSpeed,
                            this.transform.localPosition.y,
                            this.transform.localPosition.z);
                        break;
                    case DoorOpenDirection.XPositive:
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x + openSpeed,
                            this.transform.localPosition.y,
                            this.transform.localPosition.z);
                        break;
                    case DoorOpenDirection.YNegative:
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x,
                            this.transform.localPosition.y - openSpeed,
                            this.transform.localPosition.z);
                        break;
                    case DoorOpenDirection.YPositive:
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x,
                            this.transform.localPosition.y + openSpeed,
                            this.transform.localPosition.z);
                        break;
                    case DoorOpenDirection.ZNegative:
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x,
                            this.transform.localPosition.y,
                            this.transform.localPosition.z - openSpeed);
                        break;
                    case DoorOpenDirection.ZPositive:
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x,
                            this.transform.localPosition.y,
                            this.transform.localPosition.z + openSpeed);
                        break;
                }

            }
            if (canBeDestroy) Destroy(this);


        }


        // 外界调用，可以把这个门搞定。传入参数：门要开多少秒，门想哪开
        public void OpenAndDestroy(float _openSecond,DoorOpenDirection _direction)
        {
            isOpen = true;
            direction = _direction;
            openSecond = _openSecond;
            StartCoroutine(WaitForDestroy());
        }

        IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(openSecond);
            canBeDestroy = true;
        }
    }
}