using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.StartScene
{
    [RequireComponent(typeof(Camera))]
    public class CameraRotate : MonoBehaviour
    {

        private new Camera camera;

        public float rotateSpeed = 0.02f;
        // Use this for initialization
        void Start()
        {
            camera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            camera.transform.Rotate(new Vector3(0.0f, rotateSpeed, 0.0f));
        }
    }
}