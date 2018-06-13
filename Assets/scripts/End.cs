using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {
    private int Cnt = 0;//到达终点的人数

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Cnt++;
            Debug.Log(Cnt);
        }
    }
}
