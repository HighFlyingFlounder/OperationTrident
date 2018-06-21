using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour {

    //关门的timeline
    public UnityEngine.Playables.PlayableDirector m_Director;

    //进入多少个玩家就关门
    public int m_DoorClosePlayerCount = 1;

    //已进入的玩家
    private int m_EnteredPlayerCount =0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        
    }

}
