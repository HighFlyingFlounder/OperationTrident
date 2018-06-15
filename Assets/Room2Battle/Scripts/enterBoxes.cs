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

        [SerializeField]
        protected int message1;

        [SerializeField]
        protected string tags;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == tags)
            {
                (subsceneCotrol.GetComponent(subsceneName) as Subscene).notify(message1); 
            }
        }
       
    }
}
