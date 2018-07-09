using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Common {
    public class TagFollow : MonoBehaviour {
        public float TagHeight;

        private float m_TagHeight;
        private float m_Scale;
        private GetCamera m_GetCamera;
        private string m_Name;

        // Use this for initialization
        void Start() {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log(players.Length);
            foreach(GameObject player in players) {
                m_GetCamera = player.GetComponent<GetCamera>();
                if(m_GetCamera != null) {
                    break;
                }
            }

            m_Name = this.gameObject.name;

            //Camera camera = m_GetCamera.GetCurrentUsedCamera();
            //m_Scale = TagHeight * Vector3.Magnitude(transform.position - camera.transform.position);
        }

        private void OnGUI() {
            //m_TagHeight = m_Scale / Vector3.Magnitude(transform.position - m_GetCamera.GetCurrentUsedCamera().transform.position);

            //Debug.Log(m_TagHeight);
            //得到tag在3D世界中的坐标
            Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            //根据tag头顶的3D坐标换算成它在2D屏幕中的坐标
            Vector2 position = m_GetCamera.GetCurrentUsedCamera().WorldToScreenPoint(worldPosition);
            //得到真实tag的2D坐标
            position = new Vector2(position.x, Screen.height - position.y);
            //计算tag的宽高
            Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(m_Name));
            //设置显示颜色
            GUI.color = Color.red;
            //绘制tag名称
            //GUI.Label(new Rect(position.x - (m_TagHeight * 2), position.y - m_TagHeight, m_TagHeight, m_TagHeight), m_Name);
            GUI.Label(new Rect(position.x - (nameSize.x / 2), position.y - nameSize.y, nameSize.x, nameSize.y), m_Name);
        }
    }
}
