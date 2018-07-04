using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using OperationTrident.EventSystem;

    [RequireComponent(typeof(Image))]
    public class ButtonScript : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        // Button背景
        private Image image;

        public const float maxAlpha = 50.0f/256.0f;
        public const float minAlpha = 12.0f/256.0f;
        public const float changeSpeed = 5.0f/256.0f;

        private bool increasing = false;
        // Use this for initialization
        void Start()
        {
            
            image = GetComponent<Image>();
            image.color = new Vector4(image.color.r, image.color.g, image.color.b, minAlpha);
        }

        // Update is called once per frame
        void Update()
        {
            // 鼠标在上面
            if (increasing)
            {
                Vector4 toChange = image.color;
                toChange.w = Mathf.Min(toChange.w + changeSpeed, maxAlpha);
                image.color = toChange;
            }
            else
            {
                Vector4 toChange = image.color;
                toChange.w = Mathf.Max(toChange.w - changeSpeed, minAlpha);
                image.color = toChange;
            }
        }

        // 移到上面的时候调用
        public void OnPointerEnter(PointerEventData eventData)
        {
            increasing = true;
        }

        // 移出去的时候调用
        public void OnPointerExit(PointerEventData eventData)
        {
            increasing = false;
        }



}
