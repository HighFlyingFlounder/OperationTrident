using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace room2Battle
{
    public class enterElevator : MonoBehaviour
    {
        protected bool enter = false;
        // Use this for initialization
        
        public bool isEnter
        {
            get {
                return enter;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                enter = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                enter = false;
            }
        }
    }

}
