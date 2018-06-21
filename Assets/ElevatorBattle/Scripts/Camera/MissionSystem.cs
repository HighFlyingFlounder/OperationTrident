using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;
using System;

namespace OperationTrident.Elevator
{
    public class MissionSystem : MonoBehaviour
    {

        // 场景的相机主相机
        private new Camera camera;

        // 图标离目标有多高
        public float missionLabelOffset = 0.5f;

        // 任务系统！！！的显示字符串的位置
        private Vector3 UIPosition;

        // 是否显示（任务点)，背对着的时候不显示
        private bool toDisplayTheMissionPoint = true;

        // 显示任务目标更新
        //private bool toDisplayNewMission;

        // 在显示的任务目标索引
        private int missionContentsIndex;

        private string[] missionContents = {
            "",
            "开启电梯门",
            "寻找启动电梯的按钮",
            "请抵御来袭的敌人，活下去！",
            "逃出电梯"
        };

        // 任务目标的内容
        private string missionContent;

        // 目标的世界坐标
        private Vector3 targetWorldPosition;

        // 字幕每个字显示的时间
        public float timePerSubTitleWord = 1f;

        // 是否显示距离
        private bool display;

        private float nowDistance;
        // Use this for initialization
        void Start()
        {
            camera = GetComponent<Camera>();
            //toDisplayNewMission = true;
            missionContent = String.Empty;
            missionContentsIndex = 0;
            display = false;
        }

        // Update is called once per frame
        void Update()
        {
            // 准备传入任务目标的世界坐标
            targetWorldPosition = new Vector3();
            // 准备传入的任务目标的下标
            switch (SceneController.state)
            {
                case SceneController.ElevatorState.Initing:
                    missionContentsIndex = 1;
                    display = true;
                    targetWorldPosition = new Vector3(16, 2, 5);
                    break;
                case SceneController.ElevatorState.FindingButton:
                    missionContentsIndex = 2;
                    display = true;
                    targetWorldPosition = SceneController.ButtonPosition;
                    missionLabelOffset = 0.1f;
                    break;
                case SceneController.ElevatorState.Start_Fighting:
                    display = false;
                    break;
                case SceneController.ElevatorState.Fighting:
                    missionContentsIndex = 3;
                    display = false;
                    break;
                case SceneController.ElevatorState.End:
                    missionContentsIndex = 4;
                    display = false;
                    break;
            }

            missionContent = missionContents[missionContentsIndex]; // 设置要显示的任务目标内容
            nowDistance = Vector3.Distance(targetWorldPosition,
                     GetComponentInParent<Transform>().position); // 两个世界坐标的
            Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0); // 屏幕中心
            Ray ray = camera.ScreenPointToRay(point); // 在摄像机所在位置创建射线
            Vector3 direction1 = ray.direction; // 摄像头的方向
            Vector3 direction2 = targetWorldPosition - GetComponentInParent<Transform>().position; // 到物体的方向
            // 如果物体大方向在人视线背后的话，就不显示了
            if (Vector3.Dot(direction1, direction2) <= 0) toDisplayTheMissionPoint = false;
            else toDisplayTheMissionPoint = true;
            targetWorldPosition = new Vector3(targetWorldPosition.x, targetWorldPosition.y + missionLabelOffset, targetWorldPosition.z);
            //UIPosition = camera.WorldToScreenPoint(targetWorldPosition);
        }

        private void DisplayNewMission()
        {
            //TODO:
            return;
        }

        //onGUI在每帧被渲染之后执行
        private void OnGUI()
        {
            // 显示任务目标

            if (missionContent != String.Empty)
            {
                GUIUtil.DisplayMissionTargetInMessSequently(missionContent, camera, GUIUtil.blueColor, interval: 0.2f, blingInterval: 0.05f, fontSize: 16, inLeft: true, sequentClear: true);
            }

            GUIStyle style = GUIUtil.GetDefaultTextStyle(GUIUtil.FadeAColor(GUIUtil.greyColor, 60.0f));
            Rect rect = GUIUtil.GetFixedRectDirectlyFromWorldPosition(targetWorldPosition, camera);
            // 指定颜色
            if (toDisplayTheMissionPoint && display)
            {
                GUI.Label(rect, (int)nowDistance + "m", style);
            }

            string subtitle = "^w你好，^r面包^w，我是^y甜甜圈";
            GUIUtil.DisplaySubtitleInGivenGrammar(subtitle, camera, 20, 0.8f, subtitle.Length * timePerSubTitleWord);
        }
    }
}