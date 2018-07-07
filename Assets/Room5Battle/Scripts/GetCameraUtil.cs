using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room5
{
    public class GetCameraUtil
    {

        //使用FPS的GetCamera获取当前player在使用的Camera
        public static Camera GetCurrentCamera()
        {
            OperationTrident.FPS.Common.GetCamera fpsGetCamera=null;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                fpsGetCamera = player.GetComponent<OperationTrident.FPS.Common.GetCamera>();
                if (fpsGetCamera != null)
                {
                    break;
                }
            }
            return fpsGetCamera.GetCurrentUsedCamera();
        }
    }
}