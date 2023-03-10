using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        渐变工具
            执行Update时，会使渐变值在指定范围内渐变
    */
    public enum FadeSate
    {
        Stop,
        In,
        Out
    }

    public interface IFadeUtil
    {
        // 设置渐变范围
        public void SetRange(float min,float max);
        // 设置渐变状态（淡入/淡出）
        public void SetState(FadeSate state,bool init=false,Action cb=null);
        // 更新渐变值（根据当前渐变状态）
        public float Update();
    }
    

    public class FadeUtil
    {
        float curValue;                         // 当前值
        public float CurValue => curValue;
        float step;                             // 渐变步长
        public float Step
        {
            get {return step;}
            set {step=value;}
        }
        float min;                              // 最小值
        public float Min
        {
            get {return min;}
            set {min=value;}
        }
        float max;                              // 最大值
        public float Max
        {
            get {return max;}
            set {max=value;}
        }
        FadeSate state;                         // 当前工作状态
        public FadeSate State => state;
        Action callBack;                        // 结束回调

        public FadeUtil(float min,float max,float step=0.1f)
        {
            this.min = min;
            this.max = max;
            this.step = step;
            state = FadeSate.Stop;
            Debug.Log("FadeUtil构造完毕");
        }

        public void SetRange(float min,float max)
        {
            this.min = min;
            this.max = max;
        }

        // 设置渐变状态（可以选择是否重置当前值）
        public void SetState(FadeSate state,bool init=false,Action cb=null)
        {
            Console.WriteLine(state);
            this.state = state;
            if(cb!=null) callBack = cb;
            switch(state)
            {
                case FadeSate.Stop:
                    break;
                case FadeSate.In:
                    if(init) curValue = min;
                    break;
                case FadeSate.Out:
                    if(init) curValue = max;
                    break;
            }
        }

        // 更新当前渐变值
        public float Update()
        {
            if(state==FadeSate.Stop) return curValue;
            switch(state)
            {
                case FadeSate.In:
                    if(curValue<max)
                    {
                        Debug.Log(string.Format("渐入，{0} -> {1}",curValue,max));
                        curValue = Mathf.MoveTowards(curValue,max,Time.deltaTime);
                    }
                    else
                    {
                        Debug.Log("渐入结束");
                        curValue = max;
                        callBack?.Invoke();
                        SetState(FadeSate.Stop);
                    }
                    break;
                case FadeSate.Out:
                    if(curValue>min)
                    {
                        Debug.Log(string.Format("渐入，{0} -> {1}",curValue,min));
                        curValue = Mathf.MoveTowards(curValue,min,step);
                    }
                    else
                    {
                        Debug.Log("渐出结束");
                        curValue = min;
                        callBack?.Invoke();
                        SetState(FadeSate.Stop);
                    }
                    break;
            }
            return curValue;
        }


    }
}