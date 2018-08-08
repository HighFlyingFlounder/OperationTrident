using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class UIRoomInfo : MonoBehaviour
    {
        public class RoomInfo
        {
            public int roomID;
            public int roomStatus;
            public int playerNum;

            public RoomInfo(int roomID, int roomStatus, int playerNum)
            {
                this.roomID = roomID;
                this.roomStatus = roomStatus;
                this.playerNum = playerNum;
            }
        }

        [SerializeField]
        Text roomIDText;
        [SerializeField]
        Text roomStatusText;
        [SerializeField]
        Text playerNumText;

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            roomIDText.text += roomInfo.roomID.ToString();
            roomStatusText.text += roomInfo.roomStatus == 1 ? "准备中" : "正在战斗";
            playerNumText.text += roomInfo.playerNum.ToString();
        }
    }
}