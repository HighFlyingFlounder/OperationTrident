using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator
{
    public class Button : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Operate()
        {
            Messenger<int>.Broadcast(GameEvent.Push_Button, 0);
        }
    }
}
