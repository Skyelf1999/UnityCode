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
        资源加载System
            由于非MonoBehaviour无法启动协程，因此异步加载必须要由此对象实现
            只保存静态、动态加载方法，不负责保存家加载结果
            使用时，请传入资源在 [“Resources/” 下的完整路径]

    */
    public interface IResourceLoadSystem
    {
        // 静态加载
        public T SyncLoad<T>(string name) where T :UnityEngine.Object;
        // 动态加载（传入回调函数作为请求的回调函数）
        public void AsyncLoadCb<T>(string name,Action<AsyncOperation> cb) where T :UnityEngine.Object;

        // 动态加载（协程）
        // 必须传入接收加载结果的变量，可以选择性传入回调函数来处理加载结果
        public void AsyncLoadCor<T>(string name,ref T result,Action<T> cb=null) where T :UnityEngine.Object;

    }


    public class ResourceLoadSystem : MonoBehaviour , IResourceLoadSystem
    {
        public static ResourceLoadSystem instance;
        UnityEngine.Object result;


        private void Awake() {
            Debug.Log("ResourceLoadSystem: Awake");
            instance = this;
        }


        // 动态加载（回调函数）
        public void AsyncLoadCb<T>(string name,Action<AsyncOperation> cb) where T :UnityEngine.Object
        {
            Debug.Log("动态加载(回调) 目标："+name);
            ResourceRequest request = Resources.LoadAsync<T>(name);
            request.completed += cb;
        }


        // 动态加载（协程）
        // 必须传入接收加载结果的变量，可以选择性传入回调函数来处理加载结果
        public void AsyncLoadCor<T>(string name,ref T result,Action<T> cb=null) where T :UnityEngine.Object
        {
            Debug.Log("动态加载(协程) 目标："+name);
            StartCoroutine(Load<T>(name,result,cb));
        }
        IEnumerator Load<T>(string name,T result,Action<T> cb=null) where T :UnityEngine.Object
        {
            // 创建请求
            ResourceRequest request = Resources.LoadAsync<UnityEngine.Object>(name);
            yield return request;
            if(request.isDone && request.asset!=null)
            {
                result = request.asset as T;
                Debug.Log("加载成功");
            }
            else Debug.Log("加载失败");
            // 调用回调处理加载结果
            cb?.Invoke(result);
        }

        

        // 静态加载
        public T SyncLoad<T>(string name) where T :UnityEngine.Object
        {
            Debug.Log("静态加载 目标："+name);
            T ret = Resources.Load<T>(name);
            return ret;
        }
    }
}