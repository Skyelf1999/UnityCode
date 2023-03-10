using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        单例类模板
            部分游戏对象可能需要在整个过程中都保持单例状态
            将类名称通过泛型传递
        
        例如：
            定义一个用于执行System的Update的工具类
            public class SystemObjectCtrl : SingleMono<SystemObjectCtrl> , IController
    */
    public interface ISingleMono<T> where T:MonoBehaviour
    {
        public static T Instance {get;}
    }
    

    public abstract class SingleMono<T> : MonoBehaviour where T:MonoBehaviour
    {
        protected static T instance;
        public static T Instance
        {
            get 
            {
                if(instance==null)
                {
                    // 如果没初始化过，根据脚本名称创建游戏对象
                    var o = new GameObject(typeof(T).Name);
                    instance = o.AddComponent<T>();
                    GameObject.DontDestroyOnLoad(o);
                }
                return instance;
            }
        }

    }

}