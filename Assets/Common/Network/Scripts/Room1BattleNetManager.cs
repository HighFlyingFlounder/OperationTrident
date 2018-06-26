using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1 {
    public class Room1BattleNetManager : SceneNetManager {
        public override void HandlePlayer(string id, GameObject playerObj)
        {
            if (id == GameMgr.instance.id)//本地玩家
            {
                playerObj.AddComponent<RayShooter>();
                playerObj.AddComponent<InteractiveRay>();
            }
        }
    }
}