using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.AudioSystem
{

    /*
    背景音乐播放引擎：@class: BGMController震撼登场！

    可供外部调用的接口：
    @method: AudioGlobalSetting  修改音频全局设置，一般不用调用。要调用的话，请看下面的函数注释
    （修改的是全局的设置，不止影响BGM，可能会影响到其他音效！！）
    @method: ChangeAudioClip  更换当前BGM，有两种重载：随机更换和指定索引更换
    @method: ToPlay    可供外界调用，使BGM播放（处在不是playing状态）。操作时转变control状态机，在Update循环里面检测状态。
    @method: ToStop    可供外界调用，使BGM停止播放（处在Playing播放状态或Pausing暂停状态）
    @method: ToPause   可供外界调用，使BGM暂停播放（处在Playing播放状态）
    @method: ToUnPause 可供外界调用，使BGM继续播放（处在Pausing暂停状态）
    上面四个方法都有带有一个延迟参数@parameter: delayTime:float,ulong?（单位:秒）的重载函数。
    @method: AudioProcess   可以传入@interface: IAudioProcessor接口的继承类对象来实现音频的处理。
    */

    public class BGMController : MonoBehaviour
    {

        // 当前音频的播放状态
        public enum BGMPlayingState { Initing, Playing, Stoping, Pausing };

        private BGMPlayingState playingState;

        // 主要设计来方便统一变更的状态
        public enum BGMPlayingControlState { None, ToPlay, ToStop, ToPause, ToUnPause };

        private BGMPlayingControlState playingControlState;

        // 需要播放的所有bgm，可以都拖过来
        //public AudioClip[] audioClips;
        [SerializeField]
        private List<AudioClip> audioClips;

        // 每个音频剪辑的时长，单位是秒！！
        private float[] audioClipsTime;

        // 当前的播放源
        private AudioSource audioSource;

        // 从哪个音频剪辑开始
        [SerializeField]
        private int startAudioClipIndex = -1;

        // 是否淡入淡出
        [SerializeField]
        private bool fadedOn = false;

        // 淡入淡出处理类（已弃用）
        //[SerializeField] private FadeInFadeOutProcessor fadedProcessor = new FadeInFadeOutProcessor();

        [SerializeField]
        private FadedParameter fadedParameter;

        // 修改音频全局设置，一般不予以调用!!
        public void AudioGlobalSetting(int dspBufferSize, /*数字音频处理的缓冲区大小，低16位标明缓冲区尺寸，高16位表明分片（fragment的最大序号）*/
                                                          /*int numRealVoices,*/ /*当前游戏中进行的最大的同时进行的音频的数量，不予以修改*/
        int numVirtualVoices, /*可以支持的最大的同时进行的音频数量，超过这个数量的会简单的停止*/
        int sampleRate, /*音频输出设备的采样率*/
        AudioSpeakerMode speakerMode)
        {
            // 声音的全局设置
            AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
            // TODO:中间设置内容
            audioConfiguration.dspBufferSize = dspBufferSize;
            //audioConfiguration.numRealVoices = numRealVoices;  这可不能随便更改啊！
            audioConfiguration.numVirtualVoices = numVirtualVoices;
            audioConfiguration.sampleRate = sampleRate;
            audioConfiguration.speakerMode = speakerMode;
            /* 
            //     Channel count is unaffected.
            Raw = 0,
            //     Channel count is set to 1. The speakers are monaural.
            Mono = 1,
            //     Channel count is set to 2. The speakers are stereo. This is the editor default.
            Stereo = 2,
            //     Channel count is set to 4. 4 speaker setup. This includes front left, front right,
            //     rear left, rear right.
            Quad = 3,
            //     Channel count is set to 5. 5 speaker setup. This includes front left, front right,
            //     center, rear left, rear right.
            Surround = 4,
            //     Channel count is set to 6. 5.1 speaker setup. This includes front left, front
            //     right, center, rear left, rear right and a subwoofer.
            Mode5point1 = 5,
            //     Channel count is set to 8. 7.1 speaker setup. This includes front left, front
            //     right, center, rear left, rear right, side left, side right and a subwoofer.
            Mode7point1 = 6,
            //     Channel count is set to 2. Stereo output, but data is encoded in a way that is
            //     picked up by a Prologic/Prologic2 decoder and split into a 5.1 speaker setup.
            Prologic = 7*/
            AudioSettings.Reset(audioConfiguration);
        }

        // Use this for initialization
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            // 一开始的音频剪辑
            if (startAudioClipIndex == -1 ||
                startAudioClipIndex >= audioClips.Count ||
                startAudioClipIndex < 0) ChangeAudioClip();
            else ChangeAudioClip(startAudioClipIndex);
            playingState = BGMPlayingState.Initing;
            playingControlState = BGMPlayingControlState.ToPlay;

            audioClipsTime = new float[audioClips.Count];

            for (int i = 0; i < audioClips.Count; i++)
            {
                audioClipsTime[i] = audioClips[i].length;
            }


        }

        // Update is called once per frame
        void Update()
        {
            switch (playingControlState)
            {
                case BGMPlayingControlState.None:
                    // 该换碟了！
                    if (audioSource.isPlaying == false && playingState == BGMPlayingState.Playing)
                    {
                        // 如果开启淡入效果，并且此时刚好换放BGM
                        if (playingState == BGMPlayingState.Initing && fadedOn)
                        {

                            StartFade();
                        }
                        ChangeAudioClip();
                        playingControlState = BGMPlayingControlState.ToPlay;
                    }
                    break;
                case BGMPlayingControlState.ToPause:
                    audioSource.Pause();
                    OnControlStateChangedDone();
                    playingState = BGMPlayingState.Pausing;
                    break;
                case BGMPlayingControlState.ToPlay:
                    // 如果开启淡入效果，并且此时刚好开始放BGM
                    if (playingState == BGMPlayingState.Initing && fadedOn)
                    {
                        StartFade();
                    }
                    audioSource.Play();
                    OnControlStateChangedDone();

                    playingState = BGMPlayingState.Playing;
                    break;
                case BGMPlayingControlState.ToStop:
                    audioSource.Stop();
                    OnControlStateChangedDone();
                    playingState = BGMPlayingState.Stoping;
                    break;
                case BGMPlayingControlState.ToUnPause:
                    audioSource.UnPause();
                    OnControlStateChangedDone();
                    playingState = BGMPlayingState.Playing;
                    break;

                default:
                    break;
            }
        }

        // 淡入淡出的执行函数
        private void StartFade()
        {
            // 多少秒加0.01
            float fadeInSecPer001 = audioSource.volume / fadedParameter.fadeInSpeed;
            // volume*100即是我们循环的次数
            int totalNum = (int)audioSource.volume * 100;
            float sourceVolume = audioSource.volume;
            audioSource.volume = 0;
            StartCoroutine(FadeRoutine(totalNum, fadeInSecPer001, sourceVolume));
        }
        // 淡入淡出协程
        private IEnumerator FadeRoutine(int totalNum, float fadeInSecPer001, float sourceVolume)
        {
            int num = 0;
            while (num < totalNum)
            {
                audioSource.volume += 0.01f;
                yield return new WaitForSeconds(fadeInSecPer001);
            }
            audioSource.volume = sourceVolume;
        }


        // 更换当前的音频剪辑（随机）
        public void ChangeAudioClip()
        {
            int randomNum = UnityEngine.Random.Range(0, audioClips.Count);
            audioSource.clip = audioClips[randomNum];
        }

        // 更换当前的音频剪辑（指定）
        public void ChangeAudioClip(int index)
        {
            if (index >= audioClips.Count)
            {
                Debug.Log("指定换的音频超过了当前音频列表的长度！");
                return;
            }
            audioSource.clip = audioClips[index];
        }

        // 变更控制状态：准备播放
        public void ToPlay()
        {
            if (playingState != BGMPlayingState.Playing) playingControlState = BGMPlayingControlState.ToPlay;
        }

        public void ToPlay(ulong delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToPlay);
            Delay(delayTime, delayDelegate);
        }

        public void ToPlay(float delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToPlay);
            Delay(delayTime, delayDelegate);
        }

        // 变更控制状态：准备暂停
        public void ToPause()
        {
            if (playingState == BGMPlayingState.Playing) playingControlState = BGMPlayingControlState.ToPause;
        }

        public void ToPause(ulong delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToPause);
            Delay(delayTime, delayDelegate);
        }

        public void ToPause(float delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToPause);
            Delay(delayTime, delayDelegate);
        }

        // 变更控制状态：准备停止
        public void ToStop()
        {
            if (playingState == BGMPlayingState.Playing) playingControlState = BGMPlayingControlState.ToStop;
        }

        public void ToStop(ulong delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToStop);
            Delay(delayTime, delayDelegate);
        }

        public void ToStop(float delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToStop);
            Delay(delayTime, delayDelegate);
        }

        // 变更控制状态：准备暂停
        public void ToUnPause()
        {
            if (playingState == BGMPlayingState.Pausing) playingControlState = BGMPlayingControlState.ToUnPause;
        }

        public void ToUnPause(ulong delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToUnPause);
            Delay(delayTime, delayDelegate);
        }

        public void ToUnPause(float delayTime)
        {
            DelayDelegate delayDelegate = new DelayDelegate(ToUnPause);
            Delay(delayTime, delayDelegate);
        }


        // 控制状态变更完毕的一个回调函数
        private void OnControlStateChangedDone()
        {
            playingControlState = BGMPlayingControlState.None;
        }

        private delegate void DelayDelegate();

        // 一个协程，帮助做到延迟
        private IEnumerator Delay(float delayTime, DelayDelegate delayDelegate)
        {
            yield return new WaitForSeconds(delayTime);
            delayDelegate();
        }

        private IEnumerator Delay(ulong delayTime, DelayDelegate delayDelegate)
        {
            yield return new WaitForSeconds(delayTime);
            delayDelegate();
        }

        // 处理音频：传入处理类和要处理的音频序号
        public void AudioProcess(IAudioProcessor audioProcessor, int processIndex)
        {
            if (audioProcessor.AudioProcess(audioClips[processIndex]))
            {
                Debug.Log("处理成功！");
            }
            else Debug.Log("处理失败");
        }

        // 处理音频：传入处理类和要处理的音频
        public void AudioProcess(IAudioProcessor audioProcessor, AudioClip audioClip)
        {
            Debug.Log("WindyIce");
            audioProcessor.AudioProcess(audioClip);
        }

        // 存在问题：是否接受ulong的delay参数？！@！￥！
    }


    /*
    一个等于是struct的公有类@Class: FadedParameter闪亮登场！
    他是一个序列化的类，里面所有的参数都可以序列化，在Unity编辑器里被列出来
    这里注意！他一定要继承@Class: System.Object 才能被序列化
    */

    [System.Serializable]
    public class FadedParameter : System.Object
    {

        // 淡入淡出的类型：线性，凹的和凸的
        public enum FadedType { Linear, Concave, Raised };
        // 淡入速度
        public int fadeInSpeed;
        // 淡出速度
        public int fadeOutSpeed;
        // 淡入类型
        public FadedType fadeInType;
        // 淡出类型
        public FadedType fadeOutType;

        // 一个很一般的构造函数，接受全部参数
        public FadedParameter(int fadeInUntilTime, int fadeOutFromTime, FadedType fadeInType, FadedType fadeOutType)
        {
            this.fadeInSpeed = fadeInUntilTime;
            this.fadeOutSpeed = fadeOutFromTime;
            this.fadeInType = fadeInType;
            this.fadeOutType = fadeOutType;
        }


    }

}