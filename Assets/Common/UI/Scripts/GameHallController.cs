using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class GameHallController : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            GameHallUIManager.Instance.EnterGameHall();
        }
    }
}