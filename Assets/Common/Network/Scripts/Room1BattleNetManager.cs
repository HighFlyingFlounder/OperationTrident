using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class Room1BattleNetManager : SceneNetManager
    {
        public override void HandlePlayer(string id, GameObject playerObj)
        {
            RayShooter ray_shooter = playerObj.AddComponent<RayShooter>();
            //playerObj.GetComponent<NetSyncController>().sync_scripts.Add(ray_shooter);
            playerObj.GetComponent<NetSyncController>().AddSyncScripts(ray_shooter);
            playerObj.AddComponent<InteractiveRay>();
            if (id != GameMgr.instance.id)
                playerObj.GetComponent<RayShooter>().isLocalPlayer = false;
        }
    }
}