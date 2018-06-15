using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OperationTrident.AudioSystem
{
    /*
     音频处理的接口@interface: IAudioProcessor 
     用于处理实际的音频剪辑，AudioClip
     */
    public interface IAudioProcessor
    {
        // 处理函数接口
        bool AudioProcess(AudioClip sourceAudio);
    }

    /*
    淡入淡出的音频处理类@Class: FadeInFadeOutProcessor
    他实现了音频处理接口@interface: IAudioProcessor
    事实上，并没有用到。。直接调音量就可以了，具体实现可以看@Class: BGMController
    但是提供了一种真正要用到音频数据处理的时候的实现参考。
    */

    public class FadeInFadeOutProcessor : IAudioProcessor
    {
        // 淡入持续到第几秒
        private float fadeInUntilTime = 4;

        // 淡出从结束的第几秒前开始
        private float fadeOutFromTime = 4;

        // 淡入、淡出的类型
        // 1: 线性
        // 2: 凸 先快后慢
        // 3: 凹 先慢后快
        private int fadeInType = 1;
        private int fadeOutType = 1;

        // 默认构造函数
        public FadeInFadeOutProcessor()
        {

        }

        // 接受参数的构造函数
        public FadeInFadeOutProcessor(float _fadeInUntilTime,
            float _fadeOutFromTime,
            int _fadeInType,
            int _fadeOutType)
        {
            fadeInUntilTime = _fadeInUntilTime;
            fadeOutFromTime = _fadeOutFromTime;
            fadeInType = _fadeOutType;
            fadeOutType = _fadeOutType;
        }

        // 渐进渐出处理函数
        public bool AudioProcess(AudioClip sourceAudio)
        {
            float[] data = new float[sourceAudio.samples];
            if (sourceAudio.GetData(data, 0))
            {
                int samplesPerSecond = sourceAudio.frequency;
                float[] fadedFactors = GetAllFadedFactor(sourceAudio.samples,
                    samplesPerSecond, sourceAudio.length);
                Debug.Log("因子长度：" + fadedFactors.Length + "音频长度:" + sourceAudio.length);

                //if (data.Length != fadedFactors.Length)
                //{
                //    Debug.Log("AudioProcess: 处理长度不对");
                //    return false;
                //}

                //Debug.Log("开始处理");
                //for(int i = 0; i < data.Length; i++)
                //{
                //    data[i] = data[i] * fadedFactors[i];
                //}
            }
            else
            {
                Debug.Log("获取音频数据失败");
            }
            //sourceAudio.SetData(data, 0);
            return true;
        }

        // 接受一个现在的采样数，返回应该削减多少 
        // 参数：nowSamples:当前的采样数。 samplesPerSecond:音频每一秒的采样数。audioLength：音频的时长
        private float GetFadedFactor(float nowSamples,
            int samplesPerSecond,
            float audioLength)
        {
            // 现在的时间
            float nowTime = nowSamples / samplesPerSecond;
            // 如果在fadeIn之后fadeOut之前就不管
            if (nowTime >= fadeInUntilTime && nowTime <= (audioLength - fadeOutFromTime)) return 1;
            // 处于渐近的时间
            if (nowTime < fadeInUntilTime)
            {
                switch (fadeInType)
                {
                    // 线性
                    case 1:
                        return nowTime / fadeInUntilTime;

                }
            }
            return 1;
        }

        // 一次性返回一个数组，获得整个faded的factor。
        private float[] GetAllFadedFactor(int samples,
            int samplesPerSecond, float audioLength)
        {
            float[] toRet = new float[samples];

            if (fadeInUntilTime > (audioLength - fadeOutFromTime))
            {
                Debug.Log("AudioProcessor: 淡入淡出时间交叉！");
                throw new Exception("AudioProcessor: 淡入淡出时间交叉！");
            }
            // 处理淡入的
            switch (fadeInType)
            {
                // 线性
                case 1:
                    //Debug.Log((int)fadeInUntilTime * samplesPerSecond);
                    for (int i = 0; i < (int)fadeInUntilTime * samplesPerSecond; i++)
                    {
                        toRet[i] = i / (int)(fadeInUntilTime * samplesPerSecond);
                        Debug.Log(toRet[i]);
                    }
                    break;
                // 先快后慢
                case 2:
                    break;
                // 先慢后快
                case 3:
                    break;
            }
            // 正常的就是1
            for (int i = (int)fadeInUntilTime * samplesPerSecond;
                i < samples - (int)fadeOutFromTime * samplesPerSecond; i++)
            {
                toRet[i] = 1;
            }

            // 处理淡出的
            switch (fadeOutType)
            {
                // 线性
                case 1:
                    //Debug.Log(samples - (int)fadeOutFromTime * samplesPerSecond);
                    for (int i = samples - 1;
                        i >= samples - (int)fadeOutFromTime * samplesPerSecond; i--)
                    {
                        toRet[i] = (samples - i) / (samples - (int)fadeOutFromTime * samplesPerSecond);
                        Debug.Log(toRet[i]);
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            return toRet;
        }
    }
}