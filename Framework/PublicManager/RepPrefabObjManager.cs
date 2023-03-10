using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        动态游戏对象管理 Repetitive Prefab Object Manager
            专门负责管理游戏中需要重复创建且大量出现的游戏对象（例如子弹）
            这些游戏对象通常根据有限的Prefab资源创建
            支持 [同时管理多个Prefab和其复制体]
        
        注：
            请传入完整路径，[支持] 对 [不同路径下的同名] Prefab的管理
            因为 prefabDir 中保存了同名Prefab的所有可能路径
    */

    public interface IRepPrefabObjManager
    {
        // 添加使用的预制体名称与其对应的路径
        public void AddPrefab(string dir, string name);
        // 根据名称获取一个预制体（请传入完整路径）
        public GameObject Get(string dir, string name,Transform parent=null);
        // 回收预制体(因为预制体本身就可以挂载脚本，因此不提供自动回收方法)
        public void Recycle(GameObject ob,Action<GameObject> callback=null);
        // 同时设置所有已激活的复制对象
        public void SetAllActiveObject(string dir, string name,Action<GameObject> setCb);

    }


    public class RepPrefabObjManager : Manager<GameObject>, IRepPrefabObjManager
    {
        // 复制体挂载的目标父对象
        GameObject parentTarget;
        public GameObject ParentTarget
        {
            get {return parentTarget;}
            set {parentTarget = value;}
        }
        ResourceManager<GameObject> prefabs;                    // 加载的各个路径与对应的Prefab资源
        Dictionary<string,List<string>> prefabDir;              // 不同名称Prefab的路径                 key是Prefab名称
        Dictionary<string,List<GameObject>> obActived;          // 各个Prefab已激活的各的复制体         key是Prefab路径+名称
        Dictionary<string,Queue<GameObject>> obUnactived;       // 各个Prefab未激活的各的复制体         key是Prefab路径+名称


        public RepPrefabObjManager(GameObject gameObject) : base(gameObject)
        {
            parentTarget = gameObject;
            prefabs = ResourceSystem.instance.prefabManager;
            prefabDir = new Dictionary<string, List<string>>();
            obActived = new Dictionary<string, List<GameObject>>();
            obUnactived = new Dictionary<string, Queue<GameObject>>();
        }
        protected override void initRelativeSystems()
        {
            relativeSystems = new string[]{"ResourceLoadSystem"};
        }


        // 添加可能需要的Prefab
        public void AddPrefab(string dir, string name)
        {
            if(!prefabDir.ContainsKey(name))     // 如果Prefab不存在，加载、保存路径信息
            {
                prefabDir.Add(name,new List<string>(new string[]{dir}));
                prefabs.Get(dir+name,ob=>{
                    Debug.Log("已添加Prefab："+dir+name);
                });
                
            }
            else                                // 目标名称存在，判断目标路径是否存在
            {
                if(prefabDir[name].Contains(dir)) return;
                prefabDir[name].Add(dir);
                prefabs.Get(dir+name,ob=>{
                    Debug.Log("已添加Prefab："+dir+name);
                });
            }
            // 初始化激活/未激活列表
            string prefabName = dir+name;
            obActived.Add(prefabName,new List<GameObject>());
            obUnactived.Add(prefabName,new Queue<GameObject>());
        }


        // 获取目标类型Prefab的可用复制体
        public GameObject Get(string dir, string name,Transform parent=null)
        {
            AddPrefab(dir,name);                        // 保证Prefab本体存在

            GameObject ret;
            string prefabName = dir+name;

            if(obUnactived[prefabName].Count==0)        // 该Prefab不存在未激活的复制体
            {
                ret = GameObject.Instantiate(prefabs.Get(prefabName));
                
                ret.name = prefabName;
                Recycle(ret);
            }

            ret = obUnactived[prefabName].Dequeue();
            ret.transform.eulerAngles = new Vector3(0,0,0);
            if(parent) ret.transform.parent = parent;   // 也可以将复制体挂到别的对象身上
            else ret.transform.parent = parentTarget.transform;
            ret.SetActive(true);
            obActived[prefabName].Add(ret);

            return ret;
        }


        // 回收目标复制体（自动判断Prefab本体并回收到对应队列）
        public void Recycle(GameObject ob, Action<GameObject> callback=null)
        {
            string prefabName = ob.name;
            if(obActived[prefabName].Contains(ob)) obActived[prefabName].Remove(ob);
            ob.SetActive(false);
            obUnactived[prefabName].Enqueue(ob);
            callback?.Invoke(ob);
        }


        
        public void SetAllActiveObject(string dir, string name,Action<GameObject> setCb)
        {
            string prefabName = dir+name; 
            foreach(GameObject ob in obActived[prefabName]) setCb(ob);
        }

        public override void Clear()
        {
            prefabs.Clear();
            prefabDir.Clear();
            foreach(string name in obActived.Keys)
            {
                foreach(GameObject ob in obActived[name])
                {
                    GameObject.Destroy(ob);
                }
            }
            obActived.Clear();
            foreach(string name in obUnactived.Keys)
            {
                foreach(GameObject ob in obActived[name])
                {
                    GameObject.Destroy(ob);
                }
            }
            obUnactived.Clear();
        }
    }
}