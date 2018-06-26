using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;
using System;

namespace OperationTrident.Room1
{
    public class MissionSystem : MonoBehaviour
    {

        // 场景的相机主相机
        private new Camera camera;

        // 图标离目标有多高
        public float missionLabelOffset = 3.0f;

        // 任务系统！！！的显示字符串的位置
        private Vector3 UIPosition;

        // 是否显示（任务点)，背对着的时候不显示
        private bool toDisplayTheMissionPoint = true;

        // 显示任务目标更新
        //private bool toDisplayNewMission;

        // 在显示的任务目标索引
        private int missionContentsIndex;

        public string[] missionContents = {
            "",
            "进入第一个房间，找到打开第二个房间门的钥匙",
            "进入第二个房间，找到打开第三个房间门的钥匙",
            "尝试打开第三个房间门，取得ID卡",
            "门打不开！找找四周有什么可以利用的",
            "拿到了C4，速去取得ID卡",
            "逃离房间"
        };

        // 任务目标的内容
        private string missionContent;

        // 目标的世界坐标
        private Vector3 targetWorldPosition;

        // 字幕每个字显示的时间
        public float timePerSubTitleWord = 1.0f;

        // 任务目标每个字出现的速度
        public float appearInterval = 0.5f;

        // 任务目标每个乱码闪烁的速度
        public float blingInterval = 0.3f;

        // 任务目标是随机的生成正确的还是顺序
        public bool sequentClear = true;

        private float nowDistance;
        // Use this for initialization
        void Start()
        {
            camera = GetComponent<Camera>();
            //toDisplayNewMission = true;
            missionContent = String.Empty;
            missionContentsIndex = 0;
        }

        // Update is called once per frame
        void Update()
        {
            // 准备传入任务目标的世界坐标
            targetWorldPosition=new Vector3();
            // 准备传入的任务目标的下标
            switch (SceneController.state) {
                case SceneController.Room1State.FindingKey1:
                    missionContentsIndex = 1;
                    targetWorldPosition = SceneController.Key1WorldPosition;
                    break;
                case SceneController.Room1State.FindingKey2:
                    missionContentsIndex = 2;
                    targetWorldPosition = SceneController.Key2WorldPosition;
                    break;
                case SceneController.Room1State.TryingToOpenRoom:
                    missionContentsIndex = 3;
                    targetWorldPosition = SceneController.IDCardWorldPosition;
                    break;
                case SceneController.Room1State.FindingNeeded:
                    missionContentsIndex = 4;
                    targetWorldPosition = SceneController.CropseWorldPosition;
                    break;
                case SceneController.Room1State.FindingIDCard:
                    missionContentsIndex = 5;
                    targetWorldPosition = SceneController.IDCardWorldPosition;
                    break;
                case SceneController.Room1State.EscapingRoom:
                    missionContentsIndex = 6;
                    // TODO: 逃跑的时候任务目标是啥
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

            if (missionContent != string.Empty)
            {
                //GUIUtil.DisplayMissionTargetDefaultSequently(missionContent, camera,
                //    GUIUtil.brightGreenColor, interval: 0.4f, fontSize: 16, inLeft: true);
                GUIUtil.DisplayMissionTargetInMessSequently(missionContent, 
                    camera,
                    GUIUtil.brightGreenColor,
                    interval: appearInterval,
                    blingInterval:blingInterval,
                    fontSize: 16,
                    sequentClear:sequentClear);
            }


            GUIStyle style = GUIUtil.GetDefaultTextStyle(GUIUtil.FadeAColor(GUIUtil.greyColor,60.0f));
            Rect rect = GUIUtil.GetFixedRectDirectlyFromWorldPosition(targetWorldPosition, camera);
            // 指定颜色
            if (toDisplayTheMissionPoint)
            {
                GUI.Label(rect, (int)nowDistance + "m", style);
            }

            //string subtitle = "^w你好,^r一勺^w,我是^b鸡哥^w,我们要找到^y飞奔的啦啦啦";
            //GUIUtil.DisplaySubtitleInGivenGrammar(subtitle, camera, 20, 0.8f, subtitle.Length * timePerSubTitleWord);
            //string[] subtitles ={
            //    "^b地球指挥官:^w 根据情报显示，开启电源室入口的^y智能感应芯片^w在仓库里的几个可能位置",
            //    "^b地球指挥官:^w 你们要拿到它，小心里面的^r巡逻机器人"
            //};
            //GUIUtil.DisplaySubtitlesInGivenGrammar(
            //    subtitles, 
            //    camera, 
            //    fontSize: 16,
            //    subtitleRatioHeight: 0.9f, 
            //    secondOfEachWord: 0.5f, 
            //    secondBetweenLine: 3.0f);
        }
    }
}