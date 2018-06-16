using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBattleManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        MultiBattle.instance.StartBattle(MultiBattle.fight_protocal);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
