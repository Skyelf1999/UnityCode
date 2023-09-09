using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using QFramework;

namespace ProjectUtil
{
    /*
        资源管理工具类
            负责用字典 [存储] 项目加载的资源，具体的 [加载] 由 [ResourceLoadManager] 实现
            存储结构：[资源名称]--[资源]        （资源名称都是Resources/下的完整路径，以避免同名文件）
            不存在的资源会使用 ResourceLoadSystem 自动加载
            可以跟存储对象的类型创建不同的存储对象

        注：
            只负责存储未使用的资源，正在使用的资源请在使用处管理
            传入的目标资源名称需要是 Resources/ 下的完整路径

        示例：
            管理加载的各种图片：
                ResourceManageSystem<Sprite> spriteManager = new ResourceManageSystem(gameObject);
            管理加载的音乐资源：
                clipManager = new ResourceManageSystem<AudioClip>(gameObject);
    */
    public interface IResourceManager<T> where T :UnityEngine.Object
    {
        /// <summary>
        /// 获取目标资源
        /// </summary>
        /// <param name="name">资源在Resources下的路径</param>
        /// <returns>资源</returns>
        public T Get(string name);
        /// <summary>
        /// 获取一个对应类型的资源（用参数直接接收结果）
        /// </summary>
        /// <param name="name">资源在Resources下的路径</param>
        /// <param name="ret">接收参数</param>
        public void Get(string name,out T ret);
        /// <summary>
        /// 获取一个对应类型的资源（用回调函数处理）
        /// </summary>
        /// <param name="name">资源在Resources下的路径</param>
        /// <param name="callBack">回调</param>
        public void Get(string name,Action<T> callBack);
    }


    public class ResourceManager<T> : Manager<T>,IResourceManager<T> where T :UnityEngine.Object 
    {

        // 存储加载过的资源
        protected Dictionary<string,T> resource;
        // 默认资源路径（实际路径为 "Resources/" + dir + name
        string defaultDir;  
        public string DefaultDir
        {
            get {return defaultDir;}
            set {defaultDir=value;}
        }


        public ResourceManager(GameObject root) : base(root)
        {
            resource = new Dictionary<string,T>();
        }
        protected override void initRelativeSystems()
        {
            relativeSystems = new string[]{"ResourceLoadSystem"};
        }


        public override void Clear()
        {
            resource.Clear();
        }


        public virtual T Get(string name)
        {
            T ret;
            if(resource.TryGetValue(name,out ret)) return ret;
            ret = ResourceLoadSystem.instance.SyncLoad<T>(name);
            resource.Add(name,ret);
            return ret;
        }


        // 取出对象（引用型加载结果/回调函数）
        public virtual void Get(string name,out T ret) 
        {
            if(resource.TryGetValue(name,out ret)) return;

            ResourceLoadSystem.instance.AsyncLoadCor<T>(name,ref ret,ob=>{
                resource.Add(name,ob);
            });
        }


        public virtual void Get(string name,Action<T> callBack) 
        {
            T ret;
            // 若当前资源库中 有目标对象，取出、执行回调，返回
            if(resource.TryGetValue(name,out ret))
            {
                callBack(ret);
                return;
            }

            // 当前库中没有目标资源，加载目标资源
            Debug.Log("当前资源库中不存在资源："+name+"，正在加载");
            callBack += ob=>{
                resource.TryAdd(name,ob);
            };
            ResourceLoadSystem.instance.AsyncLoadCor<T>(name,ref ret,callBack);
        }

    }
}