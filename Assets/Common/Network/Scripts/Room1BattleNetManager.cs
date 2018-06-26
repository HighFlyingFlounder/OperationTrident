using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1 {
    public class Room1BattleNetManager : SceneNetManager {
        public override void HandlePlayer(string id, GameObject playerObj)
        {
            InteractiveRay temp_InteractiveRay = null;
            if (id == GameMgr.instance.id)//本地玩家
            {
                playerObj.AddComponent<RayShooter>();
                temp_InteractiveRay = playerObj.AddComponent<InteractiveRay>();
            }
            playerObj.GetComponent<NetSyncController>().sync_scripts.Add(temp_InteractiveRay);
        }
    }
}