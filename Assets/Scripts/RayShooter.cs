using OperationTrident.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OperationTrident.Room1
{
    public class RayShooter : MonoBehaviour
    {
        // 判断能否够到物体的距离
        [SerializeField]
        private float distanceQuota = 1.0f;

        // 射速：一秒钟能射多少枪
        public float shootingSpeed = 10.0f;
        // 辅助射速系统，判断能不能开枪
        private bool canShoot = true;
        

        // 后坐力：枪在X轴上的随机抖动factor
        public float jitterFactorX = 1.0f;
        public float jitterFactorY = 0.2f;

        // 附加在这个东西上的摄像机
        private new Camera camera;
        // Use this for initialization
        void Start()
        {
            camera = GetComponent<Camera>();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        IEnumerator ShootRoutine()
        {
            canShoot = false;
            yield return new WaitForSeconds(1.0f / shootingSpeed);
            canShoot = true;
        }

        // Update is called once per frame
        void Update()
        {
            //响应鼠标按键
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)&&canShoot
                //&&  !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()
                )
            {
                StartCoroutine(ShootRoutine());
                Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0);//屏幕中心
                Ray ray = camera.ScreenPointToRay(point);//在摄像机所在位置创建射线
                RaycastHit hit;//射线交叉信息的包装
                               //Raycast给引用的变量填充信息
                if (Physics.Raycast(ray, out hit))   //out确保在函数内外是同一个变量
                {
                    //hit.point:射线击中的坐标
                    GameObject hitObject = hit.transform.gameObject;//获取射中的对象
                    OperationTrident.Room1.ReactiveTarget target = hitObject.GetComponent<ReactiveTarget>();
                    if (target != null)   //检查对象上是否有ReactiveTarget组件
                    {
                        target.ReactToHit();
                        //Messenger.Broadcast(GameEvent.ENEMY_HIT);
                    }
                    else
                    {
                        StartCoroutine(SphereIndicator(hit.point));//响应击中
                    }

                }
                // 镜头随机的抖动！！
                float rotationX = camera.transform.localEulerAngles.x + Random.Range(-jitterFactorX, jitterFactorX / 4);
                float rotationY = camera.transform.localEulerAngles.y + Random.Range(-jitterFactorY, jitterFactorY);
                float rotationZ = camera.transform.localEulerAngles.z;
                camera.transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
            }
            // 处理玩家的物品交互按键
            if (Input.GetKeyDown(KeyCode.F))
            {
                Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0);//屏幕中心
                Ray ray = camera.ScreenPointToRay(point);//在摄像机所在位置创建射线
                RaycastHit hit;//射线交叉信息的包装
                               //Raycast给引用的变量填充信息
                if (Physics.Raycast(ray, out hit))   //out确保在函数内外是同一个变量
                {
                    //hit.point:射线击中的坐标
                    GameObject hitObject = hit.transform.gameObject;//获取射中的对象
                    if (Vector3.Distance(this.transform.position,hitObject.transform.position)>distanceQuota)
                    {
                        return;
                    }
                    KeyScript target = 
                        hitObject.GetComponent<KeyScript>();
                    if (target != null)   //检查对象上是否有KeyScript组件
                    {
                        Messenger<int>.Broadcast(GameEvent.KEY_GOT,target.ThisId);
                        return;
                    }
                    DoorScript target1 =
                        hitObject.GetComponent<DoorScript>();
                    if (target1 != null)
                    {
                        Messenger<int>.Broadcast(GameEvent.DOOR_OPEN,target1.ThisId);
                        return;
                    }
                    InteractiveThing target2 =
                        hitObject.GetComponent<InteractiveThing>();
                    if (target2 != null)
                    {
                        Messenger.Broadcast(GameEvent.CROPSE_TRY);
                        return;
                    }
                }
            }

        }

        //onGUI在每帧被渲染之后执行
        private void OnGUI()
        {
            int size = 12;
            float posX = camera.pixelWidth / 2 - size / 4;
            float posY = camera.pixelHeight / 2 - size / 4;
            // 准心！！！
            GUI.Label(new Rect(posX, posY, size, size), "*");


        }

        //协程，随着时间推移逐步执行
        private IEnumerator SphereIndicator(Vector3 pos)
        {
            // 创建一个新的球体
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = pos;
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            sphere.GetComponent<Collider>().enabled = false;

            yield return new WaitForSeconds(1);   //yield：协程在何处暂停

            Destroy(sphere);   //移除GameObject并释放占用的内存
        }
    }
}
