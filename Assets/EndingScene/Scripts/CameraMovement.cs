using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //逃生舱
    public GameObject m_EscapingCabin;

    //鲲
    public GameObject m_Kun;

    enum CameraState
    {
        ROAMING,//一开始缓慢移动,和靠近，用Timeline
        THIRD_PERSON,//第三人称看着逃生舱（可以控制）
        LOOKING_AT_KUN,//脱离第三人称绑定，看着鲲爆炸，不能控制
    }

    //BGM 4小节的时间
    private const float m_BgmBarTime = 60.0f / 140.0f * 4.0f ;

    //当前Camera的状态
    private CameraState m_CamState;

    //场景流逝总时间
    private float m_Time = 0.0f;

    //Timeline控制一个Camera_Directed，第三人称控制一个CameraThirdPerson
    //在Timeline里的activation track控制两个camera的enabled
    public Camera m_CamDirected;
    public Camera m_CamThirdPerson;

    //第三人称环视的Camera信息
    public float m_MouseLookSensitivity;
    private Vector3 m_ThirdPersonCamOffset;
    private Vector3 m_DestCamPos;//真正的Cam.transform要线性插值跟随这个target pos
    //private Quaternion m_DestCamRotation;//真正的Cam.transform要线性插值跟随这个target pos

    // Use this for initialization
    void Start ()
    {
        m_CamState = CameraState.ROAMING;
       // const float camRotateRadius = 50.0f;
        //m_ThirdPersonCamOffset = new Vector3(0, 0, -camRotateRadius);
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_Time += Time.deltaTime;
        switch(m_CamState)
        {
            case CameraState.ROAMING:
                Update_Roaming();
                break;

            case CameraState.THIRD_PERSON:
                Update_ThirdPerson();
                break;

            case CameraState.LOOKING_AT_KUN:
                Update_LookingAtKun();
                break;

        }

	}

    /************************************************
     *                           PRIVATE
     * **********************************************/
     private void Update_Roaming()
    {
        if(m_Time>m_BgmBarTime * 8)
        {
            //切换至第三人称状态
            m_CamState = CameraState.THIRD_PERSON;
            //m_DestCamRotation = Quaternion.identity;
            const float camRotateRadius = 50.0f;
            m_ThirdPersonCamOffset = new Vector3(0,0, -camRotateRadius);
            m_CamThirdPerson.transform.position = m_CamDirected.transform.position;
        }
    }

    private void Update_ThirdPerson()
    {

        if (m_Time > m_BgmBarTime * (8+16+16))
        {
            //切至下一状态，不再绑定在玩家的第三人称，禁用控制
            m_CamState = CameraState.LOOKING_AT_KUN;
        }
        else
        {
            //鼠标控制target transform的旋转角度
            //实际Camera transform以一定比例Lerp向target transform
            float mouseX = m_MouseLookSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
            float mouseY = -m_MouseLookSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

            //旋转视向量
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(mouseY , mouseX, 0));
            m_ThirdPersonCamOffset = deltaRotation * m_ThirdPersonCamOffset;

            //计算新的需要插值到的camera pos
            m_DestCamPos = m_EscapingCabin.transform.position+ m_ThirdPersonCamOffset;

            //实际Camera位置向Target camera处插值
            const float lerpScale =5.0f;
            m_CamThirdPerson.transform.position = Vector3.Lerp(m_CamThirdPerson.transform.position, m_DestCamPos, lerpScale * Time.deltaTime);
            m_CamThirdPerson.transform.LookAt(m_EscapingCabin.transform.position);
        }
    }

    private void Update_LookingAtKun()
    {
        Debug.Log("asdfsdf");

    }
}
