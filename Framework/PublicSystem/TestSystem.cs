using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFPlatformShooting;

namespace ProjectUtil
{
    /*
        测试管理
            创建测试Manager对象并测试相关方法
    */

    public class TestSystem : MonoBehaviour
    {
        public static TestSystem instance;

        GameObject testObject;
        Component testComponent;

        void Awake() {
            Debug.Log("TestManager: Awake");
            instance = this;
        }

        void Start() {
            Debug.Log("TestManager: Start");
            // GameObject ob = new GameObject("dsh");
            // print(ob.name);
            // ChangeObjectName(ob,"htm");
            // print(ob.name);

            // TestRecycle();

            // CreateTestObject();
        }

        void Update() {
            
        }

        
        // 更改传入对象的名称
        void ChangeObjectName(GameObject ob,string name) 
        {
            var other = ob;
            other.name = "htm";
        }

        // 测试回收与引用类型的特性
        void TestRecycle()
        {
            TestManager<GameObject> test = new TestManager<GameObject>(gameObject);
            test.printList();
            for(int i=test.objectList.Count-1;i>-1;i--)
            {
                GameObject ob = test.objectList[i];
                if(ob.name=="zdd") ob.name="张大地";
                else if(ob.name=="cwf") test.Remove(ob);
            }
                
            test.printList();
        }


        // 测试：脚本控制为别的游戏对象添加子对象，并在之后销毁
        public void CreateTestObject()
        {
            GameObject parent = GameObject.Find("Player");
            testObject = new GameObject("testObject");
            testObject.transform.parent = parent.transform;
            testObject.AddComponent<Rigidbody2D>();
            for(int i=0;i<3;i++) new GameObject("sameNameObject").transform.parent = parent.transform;
        }
        public void DestroyTestObject()
        {
            Debug.Log("摧毁测试对象");
            // if(testObject) Destroy(testObject);
            testObject.transform.SetParent(null);
            Destroy(GameObject.Find("sameNameObject"));
        }

        
    }


    
}