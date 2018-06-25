using OperationTrident.Elevator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBattleNetManager : SceneNetManager {
    public override void HandlePlayer(string id,GameObject player)
    {
        GameObject playerCamera = null;
        if(id == GameMgr.instance.id)//本地玩家
        {
            playerCamera = player.transform.Find("Camera").gameObject;
            if (playerCamera)
            {
                playerCamera.AddComponent<ElevatorMissionSystem>();
                playerCamera.AddComponent<RayShooter>();
            }
        }
    }
}
