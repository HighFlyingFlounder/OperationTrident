using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OperationTrident.Common.AI
{
    public class AITimer : MonoBehaviour
    {
        static AITimer _instance;

        // Timer objects
        List<Timer> timers;
        // Timer removal queue
        List<int> removalPending;

        private int idCounter = 0;

        public static AITimer Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Timer entity class
        /// </summary>
        class Timer
        {
            public int id;
            public bool isActive;

            public float rate;
            public int ticks;
            public int ticksElapsed;
            public float last;
            public Action callBack;

            public Timer(int _id, float _rate, int _ticks, Action _callback)
            {
                id = _id;
                rate = _rate < 0 ? 0 : _rate;
                ticks = _ticks < 0 ? 0 : _ticks;
                callBack = _callback;
                last = 0;
                ticksElapsed = 0;
                isActive = true;
            }

            public void Tick()
            {
                last += Time.deltaTime;

                if (isActive && last >= rate)
                {
                    last = 0;
                    ticksElapsed++;
                    callBack.Invoke();

                    if (ticks > 0 && ticks == ticksElapsed)
                    {
                        isActive = false;
                        AITimer.Instance.RemoveTimer(id);
                    }
                }
            }
        }

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            _instance = this;
            timers = new List<Timer>();
            removalPending = new List<int>();
        }

        /// <summary>
        /// Creates new timer
        /// </summary>
        /// <param name="rate">Tick rate</param>
        /// <param name="callBack">Callback method</param>
        /// <returns>Time GUID</returns>
        public int AddTimer(float rate, Action callBack)
        {
            return AddTimer(rate, 0, callBack);
        }

        /// <summary>
        /// Creates new timer
        /// </summary>
        /// <param name="rate">Tick rate</param>
        /// <param name="ticks">Number of ticks before timer removal</param>
        /// <param name="callBack">Callback method</param>
        /// <returns>Timer GUID</returns>
        public int AddTimer(float rate, int ticks, Action callBack)
        {
            Timer newTimer = new Timer(++idCounter, rate, ticks, callBack);
            timers.Add(newTimer);
            return newTimer.id;
        }

        /// <summary>
        /// Removes timer
        /// </summary>
        /// <param name="timerID">Timer GUID</param>
        public void RemoveTimer(int timerID)
        {
            removalPending.Add(timerID);
        }

        /// <summary>
        /// Timer removal queue handler
        /// </summary>
        void Remove()
        {
            if (removalPending.Count > 0)
            {
                foreach (int id in removalPending)
                    for (int i = 0; i < timers.Count; i++)
                        if (timers[i].id == id)
                        {
                            timers.RemoveAt(i);
                            break;
                        }

                removalPending.Clear();
            }
        }

        /// <summary>
        /// Updates timers
        /// </summary>
        void Tick()
        {
            for (int i = 0; i < timers.Count; i++)
                timers[i].Tick();
        }

        // Update is called once per frame
        void Update()
        {
            Remove();
            Tick();
        }
    }
}