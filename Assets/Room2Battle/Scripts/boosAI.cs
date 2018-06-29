using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boosAI : MonoBehaviour {
    [SerializeField]
    protected GameObject[] Missiles;

    int i = 0;

    void Update()
    {
        if (i < 10)
        {
            Instantiate(Missiles[Random.Range(0, Missiles.Length)], transform);
        }
        i++;
    }
}
