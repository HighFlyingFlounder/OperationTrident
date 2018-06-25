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

    //Timeline控制一个Camera，第三人称控制一个Camera
    public Camera m_CamDirected;
    public Camera m_CamThirdPerson;

    //第三人称环视的Camera信息
    public float m_CamRotateSpeed = 0.5f;
    private Vector3 m_ThirdPersonCamOffset;
    private Vector3 m_TargetCamPos;//用鼠标控制的，同时影响CameraPos和Lookat的方向。真是Cam.transform要线性插值跟随。

    // Use this for initialization
    void Start ()
    {
        m_CamState = CameraState.THIRD_PERSON;
        m_ThirdPersonCamOffset = new Vector3(20f,0,0);
        m_TargetCamPos = m_CamDirected.transform.position;

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

            /*case CameraState.APPROACHING_NEAR:
                Update_ApproachingNear();
                break;*/

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
        if(m_Time>m_BgmBarTime * 1)
        {
            //切换至第三人称状态
            m_CamState = CameraState.THIRD_PERSON;
            //m_ThirdPersonRadius = (m_Cam.transform.position - m_EscapingCabin.transform.position).magnitude;
            m_TargetCamPos = m_CamDirected.transform.position;//初始化第三人称视角camera的position
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
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Quaternion rot = Quaternion.Euler(new Vector3(mouseY * Time.deltaTime, mouseX * Time.deltaTime, 0));
            m_CamThirdPerson.transform.rotation *= rot;
            m_ThirdPersonCamOffset = Quaternion.Inverse(rot) * m_ThirdPersonCamOffset;
            m_TargetCamPos = m_EscapingCabin.transform.position+ m_ThirdPersonCamOffset;
            Debug.Log(m_TargetCamPos);

            //实际Camera位置向Target camera处插值
            const float lerpScale = 50.0f;
            m_CamThirdPerson.transform.position = m_TargetCamPos;
            Debug.Log(lerpScale * Time.deltaTime);
        }
    }

    private void Update_LookingAtKun()
    {
        Debug.Log("asdfsdf");

    }
}
