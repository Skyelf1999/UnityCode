using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        计时器工具类
            
    */
    public interface ITimerUtil
    {
        public void Start(float delayTime, Action<TimerUtil> onFinished, bool isLoop);
        public bool IsFinished {get;}
        public void UpdateTimer();
        public void Stop();
    }


    public class TimerUtil : ITimerUtil
    {
        float finishTime;               // 结束时间
        float delayTime;                // 计时时间
        bool isFinished;                // 是否计时结束
        public bool IsFinished
        {
            get {return isFinished;}
        }
        public Action<TimerUtil> onFinished;              // 结束回调
        bool isLoop;                    // 是否是循环


        // 开始
        public void Start(float delayTime, Action<TimerUtil> onFinished, bool isLoop)
        {
            this.delayTime = delayTime;
            finishTime = Time.time+delayTime;
            this.onFinished = onFinished;
            this.isLoop = isLoop;
            isFinished = false;
        }

        public void Stop()
        {
            isFinished = true;
        }

        public void UpdateTimer()
        {
            // Time.time会自动增长，其实这里的Update只需判断是否到时间
            if(isFinished) return;
            if(Time.time<finishTime) return;
            else    // 时间到，判断是否循环
            {
                onFinished?.Invoke(this);
                if(isLoop) finishTime += delayTime;
                else
                {
                    onFinished = null;
                    Stop();
                } 
            }
        }
    
    }
}