using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OperationTrident.EventSystem;
using UnityEngine.UI;



namespace OperationTrident.Room1
{
    public class DieHandler : MonoBehaviour,NetSyncInterface
    {
        private NetSyncController m_NetSyncController;
        public const bool ture = true;
        public readonly static string PLAYER_DIE = "PLAYER_DIE";

        public float transparentSpeed = 0.01f;

        public float maxTransparent = 0.75f;

        [SerializeField]
        private GameObject dieUIPrefab;

        private void Awake()
        {
            Messenger.AddListener(PLAYER_DIE, DieHandlerFunc);
        }

        private void OnDestroy()
        {
            Messenger.RemoveListener(PLAYER_DIE, DieHandlerFunc);
        }

        void DieHandlerFunc()
        {
            //m_NetSyncController.RPC(this, "DieHandlerImpl");
            DieHandlerImpl();
        }

        public void DieHandlerImpl()
        {
            // 暂时不知道怎么拿到场景中的玩家
            //GetComponent<FPS.Player.MovementController>().enabled = false;
            //GetComponent<FPS.Player.MouseRotator>().enabled = false;

            Instantiate(dieUIPrefab);


            // 界面背景是黑色，开始透明
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().enabled = true;
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color = new Vector4(
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.r,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.g,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.b,
                0.0f);
            // 现实的文字是红色，开始透明
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().enabled = true;
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color = new Vector4(
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.r,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.g,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.b,
                0.0f);
            // 返回主界面的按钮加上回调
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Button>().enabled = true;
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Button>().onClick.AddListener(delegate () {
                GameMgr.instance.nextScene = "GameHall";
                SceneManager.LoadScene("Loading", LoadSceneMode.Single);

                //SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
            });

            // 按钮的颜色开始透明
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().enabled = true;
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color = new Vector4(
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.r,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.g,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.b,
                0.0f);
            // 按钮文字的颜色开始透明
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().enabled = true;
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color = new Vector4(
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.r,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.g,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.b,
                0.0f
                );
        }

        public void OnReturnGameHallClick()
        {
            //退出游戏
            //SceneManager.LoadScene("GameHall", LoadSceneMode.Single);

            GameMgr.instance.nextScene = "GameHall";
            SceneManager.LoadScene("Loading", LoadSceneMode.Single);

        }

        // Use this for initialization
        void Start()
        {

        }

        private void DieCanvasUpdate()
        {
            // 界面背景是黑色，开始透明
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color =
                new Vector4(0.0f,
                0.0f,
                0.0f,
                Mathf.Min(dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.a + transparentSpeed, maxTransparent));
            // 现实的文字是红色，开始透明
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color = new Vector4(
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.r,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.g,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.b,
                 Mathf.Min(dieUIPrefab.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.a+ transparentSpeed, maxTransparent));
            // 返回主界面的按钮加上回调
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color = new Vector4(
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.r,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.g,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.b,
                Mathf.Min(dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.a+transparentSpeed,maxTransparent));
            // 按钮文字的颜色开始透明
            dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color = new Vector4(
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.r,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.g,
                dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.b,
                Mathf.Min(dieUIPrefab.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.a+transparentSpeed,maxTransparent)
                );
        }

        // Update is called once per frame
        void Update()
        {
            try
            {
                DieCanvasUpdate();
            }
            catch(System.Exception e)
            {

            }
        }

        public void RecvData(SyncData data)
        {
            throw new System.NotImplementedException();
        }

        public SyncData SendData()
        {
            throw new System.NotImplementedException();
        }

        public void Init(NetSyncController controller)
        {
            m_NetSyncController = controller;
        }
    }
}