using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;
using OperationTrident.Util;
using System;

namespace OperationTrident.Room1
{
    public class InteractiveRay : MonoBehaviour
    {
        // 判断能否够到物体的距离
        [SerializeField]
        private float distanceQuota = 3.0f;

        // 附加在这个游戏对象上的摄像头
        private new Camera camera;

        // 是否提示玩家按下某个键
        public bool toNotify = true;

        private bool toDisplayHint = false;  
        private bool usingGrammar;  // 是否使用文法
        private string hintToDisplay;  // 要显示的字幕
        private int hintFontSize;  // 显示的字幕大小

        // Use this for initialization
        void Start()
        {
            camera = Camera.current;
            hintToDisplay = string.Empty;
        }

        // Update is called once per frame
        void Update()
        {
            // 提示玩家按键
            if (toNotify)
            {
                Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0);//屏幕中心
                Ray ray = camera.ScreenPointToRay(point);//在摄像机所在位置创建射线
                RaycastHit hit;//射线交叉信息的包装
                               //Raycast给引用的变量填充信息
                if (Physics.Raycast(ray, out hit))   //out确保在函数内外是同一个变量
                {
                    //hit.point:射线击中的坐标
                    GameObject hitObject = hit.transform.gameObject;//获取射中的对象
                    if (Vector3.Distance(this.transform.position, hitObject.transform.position) <= distanceQuota)
                    {
                        HintableObject target = hitObject.GetComponent<HintableObject>();
                        if (target != null)
                        {
                            toDisplayHint = true;
                            hintToDisplay = target.WhatToHint;
                            usingGrammar = target.UsingGrammar;
                            hintFontSize = target.FontSize;
                            goto secondIf;
                        }
                    }
                    else toDisplayHint = false;
                }
            }
            secondIf:
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
                    if (Vector3.Distance(this.transform.position, hitObject.transform.position) > distanceQuota)
                    {
                        Debug.Log(Vector3.Distance(this.transform.position, hitObject.transform.position));
                        return;
                    }
                    KeyScript target =
                        hitObject.GetComponent<KeyScript>();
                    if (target != null)   //检查对象上是否有KeyScript组件
                    {
                        Messenger<int>.Broadcast(GameEvent.KEY_GOT, target.ThisId);
                        return;
                    }
                    DoorScript target1 =
                        hitObject.GetComponent<DoorScript>();
                    if (target1 != null)
                    {
                        Messenger<int>.Broadcast(GameEvent.DOOR_OPEN, target1.ThisId);
                        return;
                    }
                    if (hitObject.CompareTag("Corpse"))
                    {
                        Messenger.Broadcast(GameEvent.CROPSE_TRY);
                        return; 
                    }
                    //InteractiveThing target2 =
                    //    hitObject.GetComponent<InteractiveThing>();
                    //if (target2 != null)
                    //{
                    //    Messenger.Broadcast(GameEvent.CROPSE_TRY);
                    //    return;
                    //}
                }
            }
        }

        void OnGUI()
        {

            // 显示物体可以获得的提示
            if (toDisplayHint)
            {
                if (usingGrammar)
                    GUIUtil.DisplaySubtitleInGivenGrammar(hintToDisplay, camera, hintFontSize, 0.5f);
                else
                    GUIUtil.DisplaySubtitleInDefaultPosition(hintToDisplay, camera, hintFontSize, 0.5f);
            }
        }
    }
}