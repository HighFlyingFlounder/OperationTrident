using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OperationTrident.FPS.Common;

namespace OperationTrident.Room1
{
    public class Util
    {

        public static Camera camera;

        public static Camera GetCamera()
        {
            if(camera == null)
            {
                camera = GameMgr.instance ? SceneNetManager.instance.list[GameMgr.instance.id].GetComponent<GetCamera>().GetMainCamera() :
            GameObject.FindWithTag("Player").GetComponent<GetCamera>().GetMainCamera();
            }
            if (GameMgr.instance)//联机状态
            {
                return SceneNetManager.instance.list[GameMgr.instance.id].GetComponent<GetCamera>().GetCurrentUsedCamera();
            }
            else if (GameObject.FindWithTag("Player") != null && GameObject.FindWithTag("Player").GetComponent<GetCamera>() != null) return GameObject.FindWithTag("Player").GetComponent<GetCamera>().GetCurrentUsedCamera();
            //else if (Camera.main != null) return Camera.main;
            //else return Camera.current;
            //return Camera.current;
            else return camera;
        }

        public static void SetParent(GameObject obj, Transform parent)
        {
            obj.transform.parent = parent;
            obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            obj.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public static void SetTransform(GameObject a,Transform b)
        {
            a.transform.position = b.position;
            a.transform.localEulerAngles = b.localEulerAngles;
            a.transform.localScale = b.localScale;
        }
    }
}
