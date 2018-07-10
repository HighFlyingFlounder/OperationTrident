using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.Room1
{
    public class DisplayName : MonoBehaviour
    {
        public int fontSize;
        public Color color;
        public float labelOffsetHeight = 0.0f;
        private new string name;
        // Use this for initialization
        void Start()
        {
            name = gameObject.name;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {
            GUIUtil.DisplayWorldPointInScreen(
                gameObject.transform.position,
                Util.GetCamera() ? Util.GetCamera() : Camera.current,
                name,
                color,
                fontSize,
                labelOffsetHeight);
        }
    }
}