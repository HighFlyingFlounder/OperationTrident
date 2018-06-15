using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    //判断是否在电梯门前方
    public class enterFront : MonoBehaviour
    {

        [SerializeField]
        public Elevator door;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //计算两个向量的点积，从而判断是否面对电梯们
                Vector3 dir = other.transform.forward;
                if (Vector3.Dot(transform.forward, dir) < 0.0f)
                    door.openDoor();
            }
        }

        //离开时慢慢关门
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(slowClose());
            }
        }

        //协程，不瞬间关门
        IEnumerator slowClose()
        {
            yield return new WaitForSeconds(0.5f);
            door.closeDoor();
        }
    }
}
