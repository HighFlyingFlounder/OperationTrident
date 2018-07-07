using OperationTrident.Elevator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBattleNetManager : SceneNetManager {
    //玩家生成的时候为其进行一些操作
    public override void HandlePlayer(string id,GameObject player)
    {
        if(id == GameMgr.instance.id)//本地玩家
        {
            player.AddComponent<RenderDepth>();
        }
    }
}
