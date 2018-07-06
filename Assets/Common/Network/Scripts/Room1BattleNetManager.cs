using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class Room1BattleNetManager : SceneNetManager
    {
        public override void HandlePlayer(string id, GameObject playerObj)
        {
            playerObj.AddComponent<InteractiveRay>();
        }

        public void RecvResult(ProtocolBase protocol)
        {
            //解析协议
            int start = 0;
            ProtocolBytes proto = (ProtocolBytes)protocol;
            string protoName = proto.GetString(start, ref start);
            int isWin = proto.GetInt(start, ref start);
            //弹出胜负面板
            //玩家收到胜利条件后禁用掉list中的玩家
            GameObject flyer;
            Debug.Log("list.TryGetValue : " + list.TryGetValue(GameMgr.instance.id, out flyer));
            if (list.TryGetValue(GameMgr.instance.id, out flyer))
                flyer.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //取消监听
            NetMgr.srvConn.msgDist.DelListener("Result", RecvResult);
            ClearBattle();
       
            if (isWin == 0)//失败
            {
                Debug.Log("Room1 Fail");
                OperationTrident.EventSystem.Messenger.Broadcast(OperationTrident.Room1.DieHandler.PLAYER_DIE);
            }
            else//胜利
            {
                //某关卡胜利是直接进入下一个场景，故不会进入这里
                Debug.Log("关卡胜利！");
            }
        }
    }
}