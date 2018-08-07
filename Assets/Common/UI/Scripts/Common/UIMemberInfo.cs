using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class UIMemberInfo : MonoBehaviour
    {
        public struct MemberInfo
        {
            public string playerName;
            public bool isOwner;
            public bool isMe;

            public void Init()
            {
                playerName = "";
                isOwner = false;
                isMe = false;
            }

            public bool IsEmpty
            {
                get
                {
                    return playerName == "";
                }
            }
        }

        [SerializeField]
        GameObject backgroud;

        [SerializeField]
        Color emptyColor;
        [SerializeField]
        Color normalColor;
        [SerializeField]
        Color meColor;

        MemberInfo memberInfo;

        bool isDirty;

        private void Start()
        {
            ResetMemberInfo();
        }

        private void Update()
        {
            if (!isDirty)
                return;

            isDirty = false;
            UpdateMemberInfoUI();
        }

        public void SetMemberInfo(string playerName, bool isOwner, bool isMe)
        {
            memberInfo.playerName = playerName;
            memberInfo.isOwner = isOwner;
            memberInfo.isMe = isMe;
            isDirty = true;
        }

        public void ResetMemberInfo()
        {
            memberInfo.Init();
            isDirty = true;
        }

        void UpdateMemberInfoUI()
        {
            if (memberInfo.IsEmpty)
            {
                GetComponent<Text>().text = "";
                backgroud.GetComponent<Image>().fillCenter = false;
                backgroud.GetComponent<Image>().color = emptyColor;
                return;
            }

            GetComponent<Text>().text = " " + memberInfo.playerName + (memberInfo.isOwner ? " [房主]" : "");
            backgroud.GetComponent<Image>().fillCenter = true;

            if (memberInfo.isMe)
                backgroud.GetComponent<Image>().color = meColor;
            else
                backgroud.GetComponent<Image>().color = normalColor;

        }

    }
}
