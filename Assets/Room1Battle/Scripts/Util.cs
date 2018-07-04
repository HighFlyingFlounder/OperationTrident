using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OperationTrident.FPS.Common;

namespace OperationTrident.Room1
{
    public static class Util
    {
        public static Camera GetCamera()
        {
            if (GameMgr.instance)//联机状态
            {
                return SceneNetManager.instance.list[GameMgr.instance.id].GetComponent<GetCamera>().GetCurrentUsedCamera();
            }
            else if (GameObject.FindWithTag("Player").GetComponent<GetCamera>() != null) return GameObject.FindWithTag("Player").GetComponent<GetCamera>().GetCurrentUsedCamera();
            else if (Camera.main != null) return Camera.main;
            else return Camera.current;
        }


    }
}
