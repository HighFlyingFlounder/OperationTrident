using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.Elevator
{
    public class MissionSystem : MonoBehaviour
    {

        // 场景的相机主相机
        private new Camera camera;

        // 图标离目标有多高
        public float missionLabelOffset = 3.0f;

        // 任务系统！！！的显示字符串的位置
        private Vector3 UIPosition;

        bool display = true;

        private float nowDistance;

        private GUIStyle style;

        // Use this for initialization
        void Start()
        {
            camera = GetComponent<Camera>();
            style = GUIUtil.GetDefaultTextStyle(GUIUtil.blueColor);
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 worldPosition=new Vector3();
            switch (SceneController.state) {
                case SceneController.ElevatorState.FindingButton:
                    worldPosition = SceneController.ButtonPosition;
                    break;
                case SceneController.ElevatorState.Start_Fighting:
                    break;
                case SceneController.ElevatorState.Fighting:
                    break;
                case SceneController.ElevatorState.End:
                    break;
            }

            nowDistance = Vector3.Distance(worldPosition,
                     GetComponentInParent<Transform>().position);
            Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0); // 屏幕中心
            Ray ray = camera.ScreenPointToRay(point); // 在摄像机所在位置创建射线
            Vector3 direction1 = ray.direction; // 摄像头的方向
            Vector3 direction2 = worldPosition - GetComponentInParent<Transform>().position; // 到物体的方向
                                                                                             // 如果物体大方向在人视线背后的话，就不显示了
            if (Vector3.Dot(direction1, direction2) <= 0) display = false;
            else display = true;
            worldPosition = new Vector3(worldPosition.x, worldPosition.y + missionLabelOffset, worldPosition.z);
            UIPosition = camera.WorldToScreenPoint(worldPosition);
        }

        //onGUI在每帧被渲染之后执行
        private void OnGUI()
        {
            int size = 12;
            switch (SceneController.state)
            {
                case SceneController.ElevatorState.FindingButton:
                    if (!display) return;
                    float posX = UIPosition.x - size / 4;
                    float posY = UIPosition.y - size / 4;
                    GUI.Label(new Rect(posX, camera.pixelHeight - posY, size, size), (int)nowDistance + "m", style);
                    break;
                case SceneController.ElevatorState.Start_Fighting:
                    style.fontSize = 50;
                    GUI.Label(new Rect(camera.pixelWidth / 2 - size / 2, 0 + size / 2, size, size), "Start", style);
                    break;
                case SceneController.ElevatorState.Fighting:
                    break;
                case SceneController.ElevatorState.End:
                    break;
            }
        }
    }
}