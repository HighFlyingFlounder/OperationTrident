using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokamakeCoreFloating : MonoBehaviour {

    //漂浮&旋转的原点
    private Vector3 m_OriginPos;

    //用于漂浮的递增factor
    private static float m_Time;

    //漂浮的幅度
    public float m_FloatingAmplitude=0.1f;

    //漂浮的频率
    public float m_FloatingFrequency=2.0f;

    //旋转的速率
    public float m_RotatingVelocity = 30.0f;

	// Use this for initialization
	void Start () {
        m_OriginPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        //Quaternion rot =  Quaternion.Euler(0, Time.deltaTime * m_RotatingVelocity, 0);
        this.transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * m_RotatingVelocity,Space.World);

        m_Time += Time.deltaTime;
        this.transform.position = m_OriginPos + new Vector3(0,Mathf.Sin(m_Time* m_FloatingFrequency),0) * m_FloatingAmplitude;
    }
}
