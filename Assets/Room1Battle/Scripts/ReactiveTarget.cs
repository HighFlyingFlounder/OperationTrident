using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OperationTrident.Room1
{
    public class ReactiveTarget : MonoBehaviour,NetSyncInterface
    {
        NetSyncController m_NetSyncController;
        //  血量
        public int health = 5;

        // 是否死亡
        private bool dead;

        public bool Dead
        {
            get
            {
                return dead;
            }
        }

        [SerializeField]
        private bool isPlayer;

        public bool IsPlayer
        {
            get
            {
                return isPlayer;
            }
        }

        private Camera mCamera;

        [SerializeField]
        private GameObject blackCanvas;

        // Use this for initialization
        void Start()
        {
            dead = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (dead)
            {
                DieCanvas();
            }
        }

        private bool coroutineStart = false;

        public void DieCanvas()
        {
            //blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().rectTransform. = blackCanvas.transform;
            blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color =
                new Vector4(0.0f,
                0.0f,
                0.0f,
                Mathf.Min(blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.a + 0.01f, 0.75f));
            blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().enabled = true;
            blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color = new Vector4(
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.r,
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.g,
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.b,
                 blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.a);
            if (blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.a >= 0.75&&!coroutineStart)
            {
                coroutineStart = true;
                StartCoroutine(DisplayButton());
            }
        }

        IEnumerator DisplayButton()
        {
            yield return new WaitForSeconds(0.2f);
            blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().enabled = true;
            blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().enabled = true;
            blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color = new Vector4(
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.r,
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.g,
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.b,
                0.0f);
            blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color = new Vector4(
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.r,
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.g,
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.b,
                0.0f
                );
            while (blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.a<=1.0f) {
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color = new Vector4(
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.r,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.g,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.b,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Image>().color.a + 0.01f);
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color = new Vector4(
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.r,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.g,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.b,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").Find("Text").GetComponent<Text>().color.a + 0.01f);
                    }
        }

        public void OnHit(string id,bool fromAI,int damage)
        {
            //被射击打中的动画效果

            //单机状态
            if(GameMgr.instance==null)
                HitImplement(damage);
            else if (GameMgr.instance.id == id)//本地玩家造成伤害
            {
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
            else if(fromAI && GameMgr.instance.isMasterClient)//MasterClient机器上的AI造成了伤害
            {
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
        }

        public void HitImplement(int damage)
        {
            health -= damage;
            Debug.Log(health);
            if (health <= 0)
            {
                if (!dead)
                {
                    dead = true;
                    AIController.instance.DestroyAI(gameObject.name);
                    StartCoroutine(Die());
                }
            }
        }

        

        private IEnumerator Die()
        {
            transform.Rotate(-75, 0, 0);
            if (!isPlayer)
            {
                yield return new WaitForSeconds(1.5f);
                Destroy(gameObject);
            }
            else
            {
                mCamera = GetComponent<FPS.Common.GetCamera>().GetMainCamera();
                GetComponent<FPS.Player.MovementController>().enabled = false;
                GetComponent<FPS.Player.MouseRotator>().enabled = false;
                if (GameObject.Find("DieCanvas") == null)
                {
                    Debug.Log("0x11");
                    Instantiate(blackCanvas);

                }

                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Button").GetComponent<Button>().onClick.AddListener(delegate () {
                    SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
                });
                blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color = new Vector4(
                    blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.r,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.g,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").GetComponent<Image>().color.b,
                    0.0f);
                blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color = new Vector4(
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.r,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.g,
                    blackCanvas.transform.Find("DieCanvas").Find("Image").Find("Text").GetComponent<Text>().color.b,
                    0.0f);

            }
        }



        public void RecvData(SyncData data)
        {
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            return data;
        }

        public void Init(NetSyncController controller)
        {
            m_NetSyncController = controller;
        }
    }
}