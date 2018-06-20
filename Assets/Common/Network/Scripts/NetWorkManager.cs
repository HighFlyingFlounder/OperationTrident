using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetWorkManager : MonoBehaviour
{
    //单例
    public static NetWorkManager instance;
    //Start Game所需的协议信息
    public static ProtocolBytes fight_protocal;
    // Use this for initialization
    void Start()
    {
        //单例模式
        instance = this;
    }



}



