using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProjectUtil
{
    /*
        计时器Manager
            存储未使用的和正在使用的计时器
            提供了两种Get方法
            需要手动更新计时器
    */
    public interface ITimerManager
    {
        public TimerUtil GetWithStart(float delayTime, Action<TimerUtil> onFinished, bool isLoop,Action<TimerUtil> callback);
        public void UpdateAllTimer();
    }

    public class TimerManager : NormalObjectManager<TimerUtil> , ITimerManager
    {
        public TimerManager() : base(null)
        {
            
        }

        public override TimerUtil Get(Action<TimerUtil> callback=null)
        {
            TimerUtil ret;
            if(availableQue.Count==0)
            {
                ret = new TimerUtil();
                Recycle(ret);
            }

            ret = availableQue.Dequeue();
            workingList.Add(ret);
            ret.onFinished += RecycleOnEnd;
            callback?.Invoke(ret);
            return ret;
        }
        public TimerUtil GetWithStart(float delayTime, Action<TimerUtil> onFinished, bool isLoop,Action<TimerUtil> callback=null)
        {
            TimerUtil ret;
            if(availableQue.Count==0)
            {
                ret = new TimerUtil();
                Recycle(ret);
            }

            ret = availableQue.Dequeue();
            workingList.Add(ret);
            onFinished += RecycleOnEnd;
            ret.Start(delayTime,onFinished,isLoop);
            callback?.Invoke(ret);
            return ret;
        }
        void RecycleOnEnd(TimerUtil timer)
        {
            Recycle(timer);
        }


        public void UpdateAllTimer()
        {
            int n = workingList.Count;
            if(n==0) return;
            for(int i=n-1;i>-1;i--) workingList[i].UpdateTimer();
        }
    }
}