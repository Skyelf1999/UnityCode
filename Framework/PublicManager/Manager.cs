using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        通用Manager基类
            Manager相当于与拥有特定功能的工具类，一般用于 [存储各种对象] 
            不直接控制游戏对象，即不直接作为游戏脚本挂载，因此不继承Monobehaviour
            但是部分功能必须依赖Monobehaviour的特性
            因此可能需要在对应的System中创建Manager对象来使用预定义的功能
            有些系统也在特定情况下也需要对应的System执行操作
            使用时，请先 [创建Manager对象]

        注：
            由于一般被System使用，而System有可能不在切换场景时摧毁
            因此Manager最好提供Clear方法
            
        例如：
            MusicSystem 创建 BehaviourManager 来管理AudioSource组件
                         创建 ResourceManager 来管理需要播放的音乐资源
            资源管理ResourceManager 需要 ResourceLoadSystem 来执行加载资源的具体操作
    */

    public abstract class Manager<T>
    {
        protected string[] relativeSystems;             // 本Manager工作所需要的System支持
        protected GameObject userObject;                // 使用本Manager的游戏对象

        public Manager(GameObject gameObject)
        {
            this.userObject = gameObject;
            initRelativeSystems();
            CheckSystem();
        }

        protected abstract void initRelativeSystems();

        protected bool CheckSystem()
        {
            if(relativeSystems==null) return true;
            // 记录缺少的System
            List<string> error = new List<string>();
            for(int i=0;i<relativeSystems.Length;i++)
                if(GameObject.Find(relativeSystems[i])==null) error.Add(relativeSystems[i]);

            if(error.Count>0)
            {
                string str= string.Join(",", (string[])error.ToArray());
                Debug.Log("缺少System："+str);
                return false;
            }
            return true;
        }

        public abstract void Clear();
    }
}