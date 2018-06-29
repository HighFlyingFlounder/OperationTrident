using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIController : MonoBehaviour, NetSyncInterface
{

    private float syncPerSecond = 10;
    private List<GameObject> AI_List;
    public GameObject[] AIPrefabs;
    public NetSyncController m_NetSyncController;


    public void createAI(int num, params object[] args)
    {

    }

    public void Update()
    {
        if (syncPerSecond == 0f)
        {
            return;
        }
        if (Time.time - syncPerSecond > 1.0f / syncPerSecond)
        {
            m_NetSyncController.SyncVariables();
            syncPerSecond = Time.time;
        }
    }

    public void DestroyAI(int id)
    {

    }

    public void RecvData(SyncData data)
    {
        
    }

    public SyncData SendData()
    {
        SyncData data = new SyncData();
        //data.Add();
        return data;
    }

    public void Init(NetSyncController controller)
    {
        m_NetSyncController = controller;
    }
}