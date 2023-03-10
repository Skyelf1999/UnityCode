using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    public class TestManager<T> : Manager<T> where T : UnityEngine.Object
    {
        public List<GameObject> objectList;
        public Queue<GameObject> recycleList;
        string[] nameArr;


        public TestManager(GameObject sys) : base(sys)
        {
            objectList = new List<GameObject>();
            recycleList = new Queue<GameObject>();
            nameArr = new string[]{"dsh","htm","zdd","htm2","cwf","xg"};
            initList();
        }
        protected override void initRelativeSystems()
        {
            relativeSystems = new string[]{"TestSystem"};
        }

        void initList()
        {
            foreach(string s in nameArr)
            {
                GameObject ob = new GameObject(s);
                ob.transform.SetParent(userObject.transform);
                objectList.Add(ob);
            }
        }
        public void printList()
        {
            int n = objectList.Count;
            string[] curName = new string[n];
            Debug.Log("对象列表：");
            for(int i=0;i<n;i++)
                Debug.Log(objectList[i].name);
            Debug.Log("回收列表顶部：");
            if(recycleList.Count>0) Debug.Log(recycleList.Peek().name);
        }


        // 测试外部数据操作
        public void Remove(GameObject ob)
        {
            objectList.Remove(ob);
            recycleList.Enqueue(ob);
        }

        public override void Clear()
        {
            
        }
    }
}