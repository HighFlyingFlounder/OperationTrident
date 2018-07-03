using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.EndingScene
{
    public class UniverseLightSourceFlare : MonoBehaviour
    {

        public LensFlare m_LensFlares;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //在平行光的反向位置
            //Vector3 offsetFromCam = -10000.0f * m_DirectionalLight.transform.forward;
            //m_LensFlares.transform.position = Camera.current.transform.position + offsetFromCam;

            //调一下LensFlares的亮度（正对着最亮）
            Vector3 camLookat = Camera.main.transform.forward;
            float dotP = Vector3.Dot(camLookat.normalized, -m_LensFlares.transform.forward.normalized);
            if (dotP > 0.0f)
            {
                const float maxBrightness = 2.0f;
                m_LensFlares.brightness = (1.0f - (Mathf.Acos(dotP)) / (Mathf.PI / 2.0f)) * maxBrightness;
            }


        }
    }
}