using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OperationTrident.Room1
{
    public class DieHandler : MonoBehaviour
    {

        public void OnReturnGameHallClick()
        {
            //退出游戏
            SceneManager.LoadScene("GameHall", LoadSceneMode.Single);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}