using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        资源存储系统
            保存当前项目使用的通用资源管理类的系统
            通过不同类型的ResourceManager，存储本项目所需的所有资源
            例如：图片、音乐、预制体等

        持有3个基本资源管理对象：Sprite、音乐、Prefab
            
    */
    public interface IResourceSystem
    {
        public void InitResourceManager();
        public void ClearAllResourceManager();
    }

    public class ResourceSystem : MonoBehaviour , IResourceSystem
    {
        public static ResourceSystem instance;

        // 本项目所使用的资源类型与对应的Manager
        public ResourceManager<Sprite> spriteManager;
        public ResourceManager<AudioClip> musicManager;
        public ResourceManager<GameObject> prefabManager;


        private void Awake() {
            instance = this;
            InitResourceManager();
            Debug.Log("ResourceSystem: Awake");
        }


        public void InitResourceManager()
        {
            spriteManager = new ResourceManager<Sprite>(null);
            musicManager = new ResourceManager<AudioClip>(null);
            prefabManager = new ResourceManager<GameObject>(null);
        }

        public void ClearAllResourceManager()
        {
            spriteManager.Clear();
            musicManager.Clear();
            prefabManager.Clear();
        }
    }

}