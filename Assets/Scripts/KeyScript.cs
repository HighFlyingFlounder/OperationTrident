using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Room1
{
    public class KeyScript : MonoBehaviour
    {
        // 钥匙等关键物品的三个状态：准备（还没存在），出现，结束。
        public enum KeyState { Prepared, Existing, Finished};

        private KeyState keyState;
        // Use this for initialization
        void Start()
        {
            keyState = KeyState.Prepared;
            
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
