using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;
using OperationTrident.Util;
using System;

namespace OperationTrident.Elevator
{
    public class RayShooter : MonoBehaviour
    {
        // 判断能否够到物体的距离
        [SerializeField]
        private float distanceQuota = 3.0f;

        // 附加在这个游戏对象上的摄像头

        // 是否提示玩家按下某个键
        public bool toNotify = true;

        private bool toDisplayHint = false;
        private bool usingGrammar;  // 是否使用文法
        private string hintToDisplay;  // 要显示的字幕
        private int hintFontSize;  // 显示的字幕大小

        public static bool state = false;

        // Use this for initialization
        void Start()
        {
            hintToDisplay = string.Empty;
            state = false;
        }

        // Update is called once per frame
        void Update()
        {
            // 提示玩家按键
            if (toNotify)
            {
                Vector3 point = new Vector3(Room1.Util.GetCamera().pixelWidth / 2, Room1.Util.GetCamera().pixelHeight / 2, 0);//屏幕中心
                Ray ray = Room1.Util.GetCamera().ScreenPointToRay(point);//在摄像机所在位置创建射线
                RaycastHit hit;//射线交叉信息的包装
                               //Raycast给引用的变量填充信息
                if (Physics.Raycast(ray, out hit))   //out确保在函数内外是同一个变量
                {
                    //hit.point:射线击中的坐标
                    GameObject hitObject = hit.transform.gameObject;//获取射中的对象
                    if (Vector3.Distance(ray.origin, hitObject.transform.position) <= distanceQuota)
                    {
                        HintableObject target = hitObject.GetComponent<HintableObject>();
                        if (target != null && !state)
                        {
                            toDisplayHint = true;
                            hintToDisplay = target.WhatToHint;
                            usingGrammar = true;
                            hintFontSize = target.FontSize;
                            goto secondIf;
                        }
                    }
                    else toDisplayHint = false;
                }
            }
        secondIf:
            // 处理玩家的物品交互按键
            if (Input.GetKeyDown(KeyCode.F) && SceneController.state == SceneController.ElevatorState.Initing)
            {
                Vector3 point = new Vector3(Room1.Util.GetCamera().pixelWidth / 2, Room1.Util.GetCamera().pixelHeight / 2, 0);//屏幕中心
                Ray ray = Room1.Util.GetCamera().ScreenPointToRay(point);//在摄像机所在位置创建射线
                RaycastHit hit;//射线交叉信息的包装
                               //Raycast给引用的变量填充信息
                if (Physics.Raycast(ray, out hit))   //out确保在函数内外是同一个变量
                {
                    //hit.point:射线击中的坐标
                    GameObject hitObject = hit.transform.gameObject;//获取射中的对象
                    Debug.Log("物体" + hitObject.name);
                    Debug.Log("距离: " + Vector3.Distance(ray.origin, hitObject.transform.position));
                    if (Vector3.Distance(ray.origin, hitObject.transform.position) > distanceQuota)
                    {
                        return;
                    }
                    hitObject.SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        void OnGUI()
        {

            // 显示物体可以获得的提示
            if (toDisplayHint)
            {
                if (usingGrammar)
                    GUIUtil.DisplaySubtitleInGivenGrammar(hintToDisplay, Room1.Util.GetCamera(), hintFontSize, 0.5f);
                else
                    GUIUtil.DisplaySubtitleInDefaultPosition(hintToDisplay, Room1.Util.GetCamera(), hintFontSize, 0.5f);
            }
        }
    }
}