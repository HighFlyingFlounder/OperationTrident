using OperationTrident.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OperationTrident.Room1
{
    public class RayShooter : MonoBehaviour
    {

        // 附加在这个东西上的摄像机
        private Camera camera;
        // Use this for initialization
        void Start()
        {
            camera = GetComponent<Camera>();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            //响应鼠标按键
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)
                //&&  !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()
                )
            {
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
                    OperationTrident.Room1.KeyScript target = 
                        hitObject.GetComponent<OperationTrident.Room1.KeyScript>();
                    if (target != null)   //检查对象上是否有KeyScript组件
                    {
                        Messenger.Broadcast(GameEvent.KEY1_GOT);
                        //Messenger.Broadcast(GameEvent.ENEMY_HIT);
                    }
                    else
                    {
                        
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
