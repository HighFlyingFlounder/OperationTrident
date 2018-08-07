using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class test : MonoBehaviour
    {
        public GameObject ui;
        // Use this for initialization
        void Start()
        {
            GameHallUIManager.Instance.Open(ui);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Changed()
        {
            // Debug.Log("value change");
        }
    }
}