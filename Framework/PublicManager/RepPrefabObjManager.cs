using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        预制体游戏对象管理 Repetitive Prefab Object Manager

        功能：专门负责管理游戏中需要重复创建且大量出现的游戏对象（例如子弹）
            多个Prefab资源管理                  使用ResourceSystem的prefabManager（扩展模式）
            多个Prefab的复制体资源管理
        
        注：
            请传入 [Resources/]下 的完整路径
    */

    public interface IRepPrefabObjManager
    {
        /// <summary>
        /// 添加预制体资源
        /// </summary>
        /// <param name="dir">预制体Resources下完整资源路径</param>
        /// <param name="name">预制体名称</param>
        public void AddPrefab(string dir, string name);
        /// <summary>
        /// 根据名称获取一个预制体复制（请传入完整路径）
        /// </summary>
        /// <param name="dir">完整路径</param>
        /// <param name="name">名称</param>
        /// <param name="parent">目标父对象</param>
        /// <returns>复制体</returns>
        public GameObject GetPrefabInstance(string dir, string name,Transform parent=null);
        /// <summary>
        /// 回收预制体(因为预制体本身就可以挂载脚本，因此不提供自动回收方法)
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="callback"></param>
        public void Recycle(GameObject ob,Action<GameObject> callback=null);
        /// <summary>
        /// 同时管理所有已激活的复制对象
        /// </summary>
        /// <param name="dir">完整路径</param>
        /// <param name="name">名称</param>
        /// <param name="setCb">管理方法</param>
        public void SetAllActiveObject(string dir, string name,Action<GameObject> setCb);
    }


    public class RepPrefabObjManager : Manager<GameObject>, IRepPrefabObjManager
    {
        // 保存Prefab名称、路径信息和Prefab资源
        ResourceManager<GameObject> prefabs;
        // Dictionary<string,List<string>> prefabDir;              // 不同名称Prefab的路径                    key是Prefab名称，value是该Prefab的不同路径
        // 保存创建的复制体（名称都是预制体的路径+名称）
        Dictionary<string,List<GameObject>> obActived;          // 各个Prefab与其已激活的各的复制体         key是Prefab路径+名称
        Dictionary<string,Queue<GameObject>> obUnactived;       // 各个Prefab与其未激活的各的复制体         key是Prefab路径+名称


        public RepPrefabObjManager(GameObject gameObject) : base(gameObject)
        {
            prefabs = ResourceSystem.instance.prefabManager;
            // prefabDir = new Dictionary<string, List<string>>();
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
            prefabs.Get(dir+name);
            // 初始化激活/未激活列表
            string prefabName = dir+name;
            if(obActived.ContainsKey(prefabName)) return;
            obActived.Add(prefabName,new List<GameObject>());
            obUnactived.Add(prefabName,new Queue<GameObject>());
        }


        public GameObject GetPrefabInstance(string dir, string name,Transform parent=null)
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

            // 从未激活队列取出
            ret = obUnactived[prefabName].Dequeue();
            ret.transform.eulerAngles = new Vector3(0,0,0);
            if(parent) ret.transform.parent = parent;   // 也可以将复制体挂到别的对象身上
            else ret.transform.parent = userObject.transform;
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
            // prefabDir.Clear();
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