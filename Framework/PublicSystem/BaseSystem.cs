using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFPlatformShooting;


namespace ProjectUtil
{
    /*
        项目基础系统
            通常为 [单例] ，因此各个System一般没有继承的情况
            有些功能不针对某特定对象，但是又需要MonoBehaviour的特性（例如启动协程、每帧更新）
            因此可以创建System游戏对象来执行这些程序
            在某些框架下（例如QFramework），不必创建对应的游戏对象即可运行
            
        BaseSystem为基本管理对象的脚本
        可将各种子System挂载到此模块的GameObject下，防止场景切换导致销毁
        
        需要Update的Manager可以将其更新方法加载到此脚本中的 Updates 上
    */
    
    public class BaseSystem : MonoBehaviour
    {
        public static BaseSystem instance;
        // public static BaseSystem Instance
        // {
        //     get
        //     {
        //         if(instance==null)
        //         {
        //             // 如果没初始化过，根据脚本名称创建游戏对象
        //             var o = new GameObject(typeof(BaseSystem).Name);
        //             instance = o.AddComponent<BaseSystem>();
        //             GameObject.DontDestroyOnLoad(o);
        //         }
        //         return instance;
        //     }
        // }
        
        public Action Updates;              // 需要执行的Update方法（常用于更新System的状态）
        public MusicManager musicManager;

        private void Awake() {
            Debug.Log("BaseSystem: Awake");
            if(!instance)
            {
                instance = this;
                GameObject.DontDestroyOnLoad(gameObject);
            }
            else{
                Destroy(gameObject);
                return;
            }
            gameObject.name = "BaseSystem";

            InitSystem<ResourceLoadSystem>();
            InitSystem<ResourceSystem>();
        }
        public void InitSystem<T>() where T : MonoBehaviour
        {
            string systemName = typeof(T).Name;
            if(GameObject.Find(systemName)) return;
            GameObject system = new GameObject(typeof(T).Name);
            system.transform.SetParent(transform);
            system.AddComponent<T>();
        }


        void Start() {
            Debug.Log("BaseSystem: Start");
            // musicManager = new MusicManager(gameObject,null,null);
            // musicManager.PlayBgm("英雄の証");
            // Updates += musicManager.Update;

        }



        void Update()
        {
            transform.position = Camera.main.transform.position;
            Updates?.Invoke();
        }

    }


    
}