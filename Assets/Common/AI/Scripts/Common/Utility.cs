using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OperationTrident.Common.AI
{
    public static class Utility
    {
        public static Type GetType(string typeName)
        {
            return Type.GetType("OperationTrident.Common.AI." + typeName);
        }

        public static AIStateRegister GetAIStateRegister()
        {
            return Resources.Load<AIStateRegister>("AIStateRegister");
        }

        public static Transform[] GetPlayersPosition()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Transform[] result = new Transform[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                result[i] = players[i].transform.Find("ShootedTarget");
            }
            return result;
        }

        public static Transform GetPlayerByName(string playerName)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            
            foreach(var player in players)
            {
                if(player.name == playerName)
                    return player.transform.Find("ShootedTarget");
            }
            return null;
        }

        public static Transform DetectPlayers(AICamera camera)
        {
            camera.UpdateCamera();
            Transform[] players = Utility.GetPlayersPosition();
            foreach (var player in players)
            {
                if (camera.DetectTarget(player))
                {
                    return player;
                }
            }
            return null;
        }

        public static Vector3 GetDirectionOnXOZ(Vector3 origin, Vector3 target)
        {
            Vector3 result = target - origin;
            result.y = 0;
            return result;
        }

        public static Transform RangeDetectAll(Vector3 detectorPosition, float depressionAngle)
        {
            Transform[] players = Utility.GetPlayersPosition();
			Vector3 origin = detectorPosition;

            foreach (var player in players)
            {
                Vector3 direction = player.position - origin;
                Vector3 projectionOnXOZ = GetDirectionOnXOZ(origin, player.position);
                float angle = Vector3.Angle(direction, projectionOnXOZ);
                if (angle > depressionAngle)
                    continue;

                Ray ray = new Ray(origin, player.position - origin);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(LayerMask.GetMask("IgnoreBullets") | LayerMask.GetMask("Enemy"))))
                {
                    if (hit.collider.tag == "Player")
                    {
                        return player;
                    }
                }
            }
            return null;
        }

        public static bool RangeDetect(Transform player, Vector3 detectorPosition, float depressionAngle)
        {
			Vector3 origin = detectorPosition;
            Vector3 direction = player.position - origin;
            Vector3 projectionOnXOZ = GetDirectionOnXOZ(origin, player.position);
            float angle = Vector3.Angle(direction, projectionOnXOZ);
            if (angle > depressionAngle)
                return false;

            Ray ray = new Ray(origin, player.position - origin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(LayerMask.GetMask("IgnoreBullets") | LayerMask.GetMask("Enemy"))))
            {
                if (hit.collider.tag != "Player")
                {
                    return false;
                }
            }
            return true;
        }

#if UNITY_EDITOR
        public static bool CanDrawEditor()
        {
            return Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint;
        }
#endif
    }
}
