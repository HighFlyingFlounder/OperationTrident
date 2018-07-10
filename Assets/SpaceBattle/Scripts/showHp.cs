using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showHp : MonoBehaviour {

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(10, Screen.height - 20, 100, 20), "Health: " + GetComponent<FlyerController>().Hp);
    }
}
