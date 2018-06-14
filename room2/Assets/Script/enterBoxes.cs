using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    //通用碰撞体，检测是否进入
    public class enterBoxes : MonoBehaviour
    {
        [SerializeField]
        protected SubsceneController subsceneCotrol;

        [SerializeField]
        protected string subsceneName;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                (subsceneCotrol.GetComponent(subsceneName) as Subscene).notify(1); 
            }
        }
       
    }
}
