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

#if UNITY_EDITOR
        public static bool CanDrawEditor()
        {
            return Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint;
        }
#endif
    }
}
