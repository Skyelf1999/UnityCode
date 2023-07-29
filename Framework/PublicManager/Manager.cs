using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        通用Manager对象管理抽象类

        主要功能：管理类对象

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
        /// <summary>
        /// 本Manager工作所需要的System名称
        /// </summary>
        protected string[] relativeSystems;
        /// <summary>
        /// 使用本Manager的游戏对象
        /// </summary>
        protected GameObject userObject;


        public Manager(GameObject gameObject)
        {
            this.userObject = gameObject;
            initRelativeSystems();
            CheckSystem();
        }


        /// <summary>
        /// 初始化所需System名称数组
        /// </summary>
        protected abstract void initRelativeSystems();

        /// <summary>
        /// 检查是否缺少所需的System
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 清空管理对象
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Manager更新方法
        /// </summary>
        public virtual void ManagerUpdate()
        {

        }
    }
}