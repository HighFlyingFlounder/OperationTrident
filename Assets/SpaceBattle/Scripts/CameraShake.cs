using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    // 抖动目标的transform(若未添加引用，怎默认为当前物体的transform)
    public Transform camTransform;

    //持续抖动的时长
    public float shake = 1.0f;

    // 抖动幅度（振幅）
    //振幅越大抖动越厉害
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    private bool start = false;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    public void begin()
    {
        start = true;
    }

    void Update()
    {
        if (start && shake >= 0)
        {
            originalPos = camTransform.localPosition;
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, originalPos.y + (Random.insideUnitSphere * shakeAmount).y, originalPos.z + (Random.insideUnitSphere * shakeAmount).z);

            shake -= Time.deltaTime * decreaseFactor;

            //this.transform.localPosition = originalPos;
        }

        if(shake < 0)
        {
            start = false;
            shake = 1.0f;

            this.transform.localPosition = camTransform.localPosition;
        }
    }
}
