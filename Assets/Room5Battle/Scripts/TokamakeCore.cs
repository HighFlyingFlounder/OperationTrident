using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokamakeCore : MonoBehaviour
{
    //停止运作之后的材质
    public Material m_ShutdownCoreMat;

    //漂浮&旋转的原点
    private Vector3 m_OriginPos;

    //用于漂浮的递增factor
    private static float m_Time;

    //漂浮的幅度
    public float m_FloatingAmplitude;

    //漂浮的频率
    public float m_FloatingFrequency;

    //旋转的速率
    public float m_RotatingVelocity;

    //是否已停止运作
    private bool m_IsStop=false;

	// Use this for initialization
	void Start () {
        m_OriginPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_IsStop)
        {
            //Quaternion rot =  Quaternion.Euler(0, Time.deltaTime * m_RotatingVelocity, 0);
            this.transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * m_RotatingVelocity, Space.World);

            m_Time += Time.deltaTime;
            this.transform.position = m_OriginPos + new Vector3(0, Mathf.Sin(m_Time * m_FloatingFrequency), 0) * m_FloatingAmplitude;
        }
     }

    //关闭反应柱后，关灯，改Emissive，停止旋转。应该由外界调用
    public void Shutdown()
    {
        m_IsStop = true;
        //换个关了灯的材质
        GetComponent<MeshRenderer>().material = m_ShutdownCoreMat;
        //关个核心自带的灯
        GetComponentInChildren<Light>().enabled = false;
    }

}
