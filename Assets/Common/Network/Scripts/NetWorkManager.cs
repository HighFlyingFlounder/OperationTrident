using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetWorkManager : MonoBehaviour
{
    //单例
    public static NetWorkManager instance;
    // Use this for initialization
    void Start()
    {
        //单例模式
        instance = this;
    }



}



