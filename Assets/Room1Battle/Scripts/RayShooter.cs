using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OperationTrident.EventSystem;
using OperationTrident.Util;

namespace OperationTrident.Room1
{
    public class RayShooter : MonoBehaviour,NetSyncInterface
    {
        [SerializeField]
        private GameObject bulletPrefab;

        NetSyncController m_NetSyncController;
        // 射速：一秒钟能射多少枪
        public float shootingSpeed = 10.0f;
        // 辅助射速系统，判断能不能开枪
        private bool canShoot = true;

        // 是否开启后坐力
        public bool jitterOn = true;
        // 后坐力：枪在X轴上的随机抖动factor
        public float jitterFactorX = 1.0f;
        public float jitterFactorY = 0.2f;
        public bool isLocalPlayer = true;


        // Use this for initialization
        void Start()
        {
            
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
            if (!isLocalPlayer)
                return;

            //响应鼠标按键
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)&&canShoot
                //&&  !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()
                )
            {
                StartCoroutine(ShootRoutine());
                Vector3 point = new Vector3(Util.GetCamera().pixelWidth / 2, Util.GetCamera().pixelHeight / 2, 0);//屏幕中心
                Ray ray = Util.GetCamera().ScreenPointToRay(point);//在摄像机所在位置创建射线
                Vector3 direction = ray.direction;
                Vector3 origin = ray.origin;
                //ShootWithRay(direction.x,direction.y,direction.z,origin.x,origin.y,origin.z);
                ShootWithRay(direction, origin);
                //BulletGenerator.GeneratorBullet(ray, bulletPrefab,50.0F,-2.0f);
                if (m_NetSyncController != null)
                {
                    //m_NetSyncController.RPC(this, "ShootWithRay", direction.x, direction.y, direction.z, origin.x, origin.y, origin.z);
                    m_NetSyncController.RPC(this, "ShootWithRay", direction, origin);
                }
                // 是否开启镜头抖动
                if (jitterOn)
                {
                    // 镜头随机的抖动！！
                    float rotationX = Util.GetCamera().transform.localEulerAngles.x + Random.Range(-jitterFactorX, jitterFactorX / 4);
                    float rotationY = Util.GetCamera().transform.localEulerAngles.y + Random.Range(-jitterFactorY, jitterFactorY);
                    float rotationZ = Util.GetCamera().transform.localEulerAngles.z;
                    Util.GetCamera().transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
                }
            }
        }

        //public void ShootWithRay(float d_x,float d_y,float d_z, float o_x,float o_y,float o_z)
        public void ShootWithRay(Vector3 direction, Vector3 origin)
        {
            //Vector3 origin = new Vector3(o_x, o_y, o_z);
            //Vector3 direction = new Vector3(d_x, d_y, d_z);

            Ray ray = new Ray(origin, direction);
            RaycastHit hit;//射线交叉信息的包装
                           //Raycast给引用的变量填充信息
            if (Physics.Raycast(ray, out hit))   //out确保在函数内外是同一个变量
            {
                //hit.point:射线击中的坐标
                GameObject hitObject = hit.transform.gameObject;//获取射中的对象
                FPS.Player.ReactiveTarget target = hitObject.GetComponent<FPS.Player.ReactiveTarget>();
                if (target != null)   //检查对象上是否有ReactiveTarget组件
                {
                     target.OnHit(gameObject.name,false,1);
                }
                else
                {
                    StartCoroutine(SphereIndicator(hit.point));//响应击中
                }
            }
        }
        //onGUI在每帧被渲染之后执行
        private void OnGUI()
        {
            float posX = Util.GetCamera().pixelWidth / 2;
            float posY = Util.GetCamera().pixelHeight / 2; ;

            // 红色的准心
            GUIStyle style = GUIUtil.GetDefaultTextStyle(GUIUtil.redColor);
            Rect rect = GUIUtil.GetFixedRectDueToFontSize(new Vector2(posX, posY), 12);

            // 准心！！！
            GUI.Label(rect, "*", style);

        }

        //协程，随着时间推移逐步执行
        private IEnumerator SphereIndicator(Vector3 pos)
        {
            // 创建一个新的球体,当做是打击点
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = pos;
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            // 这个球体不会碰撞嘻嘻
            sphere.GetComponent<Collider>().enabled = false;

            yield return new WaitForSeconds(1);   //yield：协程在何处暂停

            Destroy(sphere);   //移除GameObject并释放占用的内存
        }

        public void RecvData(SyncData data)
        {
            
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            data.Add(1);
            return data;
        }

        public void Init(NetSyncController controller)
        {
            m_NetSyncController = controller;
        }
    }
}
