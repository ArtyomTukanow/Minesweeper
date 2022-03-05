using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using Libraries.RSG;
using UnityEngine;

namespace Utils
{
    public class TimerUtils : Singleton<TimerUtils>
    {
        private  readonly List<(int frames, Action callback)> framesDeferreds = new List<(int frames, Action callback)>();
        private readonly List<(float endTime, Action callback)> timeDeferreds = new List<(float endTime, Action callback)>();

        public static IPromise Wait(float time)
        {
            var promise = new Promise();
            Instance.AddByTime(Time.time + time, promise.Resolve);
            return promise;
        }
        
        public static IPromise NextFrame() => NextFrame(1);
        public static IPromise NextFrame(int count)
        {
            var promise = new Promise();
            Instance.AddByFrame(Time.frameCount + count, promise.Resolve);
            return promise;
        }


        private void AddByTime(float gameTime, Action callback)
        {
            var deferred = timeDeferreds.FirstOrDefault(t => t.endTime >= gameTime);
            var index = deferred == default ? timeDeferreds.Count : timeDeferreds.IndexOf(deferred);
            timeDeferreds.Insert(index, (gameTime, callback));
            
            if(index == 0)
                CheckByTimes();
        }

        private void AddByFrame(int endFrame, Action callback)
        {
            var deferred = framesDeferreds.FirstOrDefault(t => t.frames >= endFrame);
            var index = deferred == default ? framesDeferreds.Count : framesDeferreds.IndexOf(deferred);
            framesDeferreds.Insert(index, (endFrame, callback));
            
            if(index == 0)
                CheckByFrames();
        }
        
        private void Update()
        {
            CheckByFrames();
            CheckByTimes();
        }

        private void CheckByFrames()
        {
            var curFrame = Time.frameCount;
            
            while (framesDeferreds.Count > 0 && framesDeferreds[0].frames <= curFrame)
            {
                var frameDeferred = framesDeferreds[0];
                framesDeferreds.RemoveAt(0);
                frameDeferred.callback?.Invoke();
            }
        }

        private void CheckByTimes()
        {
            var gameTime = Time.time;
            
            while (timeDeferreds.Count > 0 && timeDeferreds[0].endTime <= gameTime)
            {
                var timeDeferred = timeDeferreds[0];
                timeDeferreds.RemoveAt(0);
                timeDeferred.callback?.Invoke();
            }
        }
    }
}