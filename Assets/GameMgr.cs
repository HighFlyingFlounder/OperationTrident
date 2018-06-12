using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "Tank";

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
}
