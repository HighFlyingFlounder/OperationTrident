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

        private static Camera GetCameraImpl()
        {
            if(camera == null)
            {
                try
                {
                    camera = GameMgr.instance ? SceneNetManager.instance.list[GameMgr.instance.id].GetComponent<GetCamera>().GetMainCamera() :
                GameObject.FindWithTag("Player").GetComponent<GetCamera>().GetMainCamera();
                }
                catch(Exception e)
                {
                    return Camera.current ? Camera.current : Camera.main;
                }
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

        public static Camera GetCamera()
        {
            try
            {
                return GetCameraImpl() ? GetCameraImpl() : Camera.current;
            }
            catch(Exception e)
            {
                return Camera.current ? Camera.current : Camera.main;
            }
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

        public static bool IsValidIP(string ip)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                string[] ips = ip.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
            else
                return false;
        }

        public static bool IsValidPort(string port)
        {
            try
            {
                GameMgr.instance.port = int.Parse(port);
                if (int.Parse(port) < 1 || int.Parse(port) > 65536)
                {
                    return false;
                }
                else return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
    }
}
