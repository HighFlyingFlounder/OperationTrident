using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class BGMRoom1 : MonoBehaviour
    {
        public AudioClip bgm1;
        public AudioClip bgm2;

        public AudioSource bgmSource;
        // Use this for initialization
        void Start()
        {
            if (bgmSource == null)
            {
                bgmSource = GetComponent<AudioSource>();
            }
            bgmSource.loop = true;
            bgmSource.clip = bgm1;
            bgmSource.Play();
        }

        private bool temp = true;

        // Update is called once per frame
        void Update()
        {
            if (SceneController.state == SceneController.Room1State.EscapingRoom)
            {
                if (temp)
                {
                    bgmSource.Stop();
                    bgmSource.clip = bgm2;
                    bgmSource.loop = true;
                    bgmSource.Play();
                    temp = false;
                }
            }
        }
    }
}
