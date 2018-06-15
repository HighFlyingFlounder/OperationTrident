using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace room2Battle
{
    //========================================================
    //=========         电梯逻辑控制类  =======================
    //========================================================
    public class Elevator : MonoBehaviour
    {
        //上升的高度大小
        [SerializeField]
        public Vector3 upFactor;

        //prefab中的左右门
        [SerializeField]
        protected GameObject leftdoor;

        [SerializeField]
        protected GameObject rightdoor;

        [SerializeField]
        protected BoxCollider front;

        [SerializeField]
        protected enterElevator inside;

        protected Camera mCamera;

        //电梯门是否开启
        bool isOpen = false;

        //电梯是否正在移动
        protected bool isMoving = false;

        //关闭电梯
        protected bool isShutDown = false;

        //为了让玩家只上一层楼，可删
        protected bool ban = false;

        //@brief 公共接口，启用协程上升
        public void goUp()
        {
            if (!isMoving && !isShutDown)
            {
                StartCoroutine(goUpImpl());
            }
        }

        //@brief 启用协程关门，上升，开门
        IEnumerator goUpImpl()
        {
            
            while (isOpen)
            {
                closeDoor();
                yield return new WaitForSeconds(1.0f);
            }
            float time = 0.0f;
            float total = 1.0f;

            isMoving = true;
            Vector3 pos = transform.position + upFactor;
            Vector3 init = transform.position;

            //通过插值实现上升
            while (time < total)
            {
                transform.position = Vector3.Lerp(init, pos, time / total);

                time += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            //保证最终位置正确
            transform.position = pos;
            //thinktime
            yield return new WaitForSeconds(1.5f);
            isMoving = false;
            //开门
            openDoor();
        }

        //@brief 关门
        public void goDown()
        {
            if (!isMoving && !isShutDown)
            {
                StartCoroutine(goDownImpl());
            }
        }

        //@brief 实现原理和goUpImpl一样
        IEnumerator goDownImpl()
        {
            while (isOpen)
            {
                closeDoor();
                yield return new WaitForSeconds(1.0f);
            }

            isMoving = true;

            float time = 0.0f;
            float total = 1.0f;

            Vector3 pos = transform.position - upFactor;
            Vector3 init = transform.position;
            while (time < total)
            {
                transform.position = Vector3.Lerp(init, pos, time / total);

                time += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.position = pos;

            yield return new WaitForSeconds(1.5f);
            isMoving = false;
            openDoor();
        }

        //@brief 调用门的脚本，其内部是一个协程
        public void openDoor()
        {
            if (!isMoving && !isOpen)
            {
                leftdoor.GetComponent<elevatorDoor>().openTheDoor();
                rightdoor.GetComponent<elevatorDoor>().openTheDoor();
                //isOpen = true;
            }
        }

        //@brief 调用门的脚本，其内部是一个协程
        public void closeDoor()
        {
            if (!isMoving && isOpen)
            {
                rightdoor.GetComponent<elevatorDoor>().closeTheDoor();
                leftdoor.GetComponent<elevatorDoor>().closeTheDoor();
                //isOpen = false;
            }
        }

        
        //关掉电梯
        public bool shutDown {
            get {
                return isShutDown;
            }
            set {
                isShutDown = value;
            }
        }

        void Start()
        {
            mCamera = Camera.main;
        }

        void Update()
        {
            isOpen = leftdoor.GetComponent<elevatorDoor>().getDoorStat;
        }

        public void disableAutoOpenDoor()
        {
            front.enabled = false;
        }

        void OnGUI()
        {
            float posX = mCamera.pixelWidth / 2 - 50;
            float posY = mCamera.pixelHeight / 2 - 50;
            if (inside.isEnter && !isMoving && !isShutDown && !ban)
            {
                GUI.Label(new Rect(posX, posY, 100, 100), "按F上楼，按E开门");
            }
            else  
            {
                GUI.Label(new Rect(posX, posY, 100, 100), "");    
            }
        }

        void LateUpdate()
        {
            
            if (inside.isEnter)
            {
                if (!isShutDown && !isMoving)
                {     
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (!ban)
                        {
                            ban = true;
                            goUp();
                        }   
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        openDoor();
                    }
                }
            }
        }
    }
}
