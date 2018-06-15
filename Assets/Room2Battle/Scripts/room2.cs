using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    public class room2 : MonoBehaviour {
        [SerializeField]
        protected SubsceneController subSceneController;

        void Start()
        {
            subSceneController.addSubscene("enterRoom2", "room2_enter");
            subSceneController.addSubscene("room2_powerroom", "room2_power");
            subSceneController.addSubscene("room2_battle", "room2_battle");
            subSceneController.setInitialSubScene("room2_powerroom");
            subSceneController.enabled = true;
        }
    }
}
