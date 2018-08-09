using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class GameHallUIManager : UIManager
    {
        static GameHallUIManager instance;

        public static GameHallUIManager Instance
        {
            get
            {
                return instance;
            }
        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as GameHallUIManager;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        [Header("UI Prefab")]
        [SerializeField]
        GameObject firstInitUIPrefab;

        public void EnterGameHall()
        {
            if (UIStack.Count == 0)
            {
                Open(firstInitUIPrefab);
            }
            else
            {
                ShowLast();
            }
        }
    }
}
