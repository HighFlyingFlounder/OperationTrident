using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Room1
{
    public class DoorScript : MonoBehaviour
    {
        // 标识三扇门的ID
        public static int totalId = 0;

        [SerializeField]
        private GameObject explosion;

        [SerializeField]
        private GameObject fragmentPrefab;

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
        public float openSpeed = 0.1f;

        // 门移动的方向
        public enum DoorOpenDirection { XPositive,XNegative, YPositive,YNegative,ZPositive,ZNegative};

        private DoorOpenDirection direction;

        // 门是否被打开
        bool isOpen;
        // 门是否失去了用处
        bool canBeDestroy;

        // 爆炸后碎片出现的位置
        private List<Vector3> fragmentsInitPoints = new List<Vector3>();

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
                        transform.position = new Vector3(transform.position.x - openSpeed*Time.deltaTime,
                            transform.position.y,
                            transform.position.z);
                        break;
                    case DoorOpenDirection.XPositive:
                        transform.position = new Vector3(transform.position.x + openSpeed * Time.deltaTime,
                            transform.position.y,
                            transform.position.z);
                        break;
                    case DoorOpenDirection.YNegative:
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y - openSpeed * Time.deltaTime,
                            transform.position.z);
                        break;
                    case DoorOpenDirection.YPositive:
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y + openSpeed * Time.deltaTime,
                            transform.position.z);
                        break;
                    case DoorOpenDirection.ZNegative:
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y,
                            transform.position.z - openSpeed * Time.deltaTime);
                        break;
                    case DoorOpenDirection.ZPositive:
                        transform.position = new Vector3(transform.position.x,
                            transform.position.y,
                            transform.position.z + openSpeed * Time.deltaTime);
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

        // 在炸开的门下面弄点碎片
        public void CreateFragmentsInFloor(int amount,bool isMainX)
        {
            Explosion();
            const float randomFactor = 0.5f;
            if (isMainX) {
                // 左上角
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x - transform.localScale.x / 4,
                        transform.position.y + transform.localScale.y - 4,
                        transform.position.z));
                // 左边
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x - transform.localScale.x / 4,
                        transform.position.y,
                        transform.position.z));
                // 左下角
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x - transform.localScale.x / 4,
                        transform.position.y - transform.localScale.y - 4,
                        transform.position.z));
                // 上边
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x,
                        transform.position.y + transform.localScale.y - 4,
                        transform.position.z));
                // 中间
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x,
                        transform.position.y,
                        transform.position.z));
                // 下边
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x,
                        transform.position.y - transform.localScale.y - 4,
                        transform.position.z));
                // 右上角
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x + transform.localScale.x / 4,
                        transform.position.y + transform.localScale.y - 4,
                        transform.position.z));
                // 右边
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x + transform.localScale.x / 4,
                        transform.position.y,
                        transform.position.z));
                // 右下角
                fragmentsInitPoints.Add(new Vector3(
                        transform.position.x + transform.localScale.x / 4,
                        transform.position.y - transform.localScale.y - 4,
                        transform.position.z));
            }
            else
            {
                // TODO: 还没实现
            }
            foreach(var point in fragmentsInitPoints)
            {
                for(int i = 0; i < amount / fragmentsInitPoints.Count; i++)
                {
                    GameObject cube = Instantiate(fragmentPrefab) as GameObject;
                    cube.transform.localScale= new Vector3(
                        UnityEngine.Random.Range(0.2f, 0.5f),
                        UnityEngine.Random.Range(0.2f, 0.5f),
                        UnityEngine.Random.Range(0.2f, 0.5f));
                    cube.transform.position = point;
                    cube.AddComponent<Rigidbody>();
                }
            }
        }

        public void Explosion()
        {
            GameObject explosionObj=Instantiate(explosion) as GameObject;
            Util.SetTransform(explosionObj, gameObject.transform);
            

        }
    }
}