using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace ProjectUtil
{
    /*
        组件管理
            管理目标游戏对象挂载的多个组件

        注：
            [只负责保存组件]
            常用于 [需要挂载很多组件的对象] ，组件无论是否激活都归此类管理
            得益于引用类型的特点，可直接传入需要操作的对象
            组件激活后 [自动挂载到 userObject]  （这是由于创建组件需要通过 AddComponent 方法）

        示例：某个游戏对象可能有多个音频组件AudioSource，创建本Manager进行管理
            BehaviourManager<AudioSource> audioSourceManager = new ComponentManager<AudioSource>(gameObject);
    */
    public interface IBehaviourManager<T> where T : Behaviour
    {
        /// <summary>
        /// 激活一个可用组件
        /// </summary>
        /// <param name="component">接收变量</param>
        public void Active(out T component);
        /// <summary>
        /// 回收组件（取消激活，并加入未激活队列）
        /// </summary>
        /// <param name="component">组件</param>
        /// <param name="callback">回收回调</param>
        public void Recycle(T component,Action<T> callback=null);
        /// <summary>
        /// 根据判断方法对所有组件进行回收
        /// </summary>
        /// <param name="judge">判断方法</param>
        /// <param name="callback">回收回调</param>
        public void RecycleAllAuto(Func<T,bool> judge,Action<T> callback=null);
        /// <summary>
        /// 统一管理已激活的组件
        /// </summary>
        /// <param name="setCb">管理方法</param>
        public void SetAllActiveComponent(Action<T> setCb);
    }


    public class BehaviourManager<T> : Manager<T>, IBehaviourManager<T> where T : Behaviour
    {
        protected List<T> cpActivated;            // 已激活的组件
        protected Queue<T> cpUnactivated;         // 未激活的组件


        public BehaviourManager(GameObject root) : base(root)
        {
            cpActivated = new List<T>();
            cpUnactivated = new Queue<T>();
        }
        protected override void initRelativeSystems()
        {
            relativeSystems = new string[]{};
        }


        // 激活：从未激活队列中取出，激活，加入激活列表，返回组件对象
        public void Active(out T component)
        {
            // 取出
            if(cpUnactivated.Count>0)
                component = cpUnactivated.Dequeue();
            else
            {
                if(userObject==null)
                {
                    Debug.Log("对应的系统对象不存在，无法激活新的组件");
                    component = null;
                    return;
                }
                component = userObject.AddComponent<T>();
            }
            // 激活
            component.enabled = true;
            cpActivated.Add(component);
        }


        // 回收：从激活列表中删除，加入未激活队列，可用回调处理回收的目标
        public void Recycle(T component,Action<T> callback=null)
        {
            component.enabled = false;
            cpActivated.Remove(component);
            cpUnactivated.Enqueue(component);
            callback?.Invoke(component);
        }
        public void RecycleAt(int index,Action<T> callback=null)
        {
            if(index>=cpActivated.Count || index<0) return;
            T component = cpActivated[index];
            component.enabled = false;
            cpActivated.Remove(component);
            cpUnactivated.Enqueue(component);
            callback?.Invoke(component);
        }


        // 自动回收：根据回收条件，回收当前激活列表中可回收的对象，可用回调处理回收的目标
        public void RecycleAllAuto(Func<T,bool> judge,Action<T> callback=null)
        {
            for(int i=cpActivated.Count-1;i>=0;i--)
                if(judge(cpActivated[i]))
                {
                    callback?.Invoke(cpActivated[i]);
                    RecycleAt(i,callback);
                }
        }


        public void SetAllActiveComponent(Action<T> setCb)
        {
            foreach(T component in cpActivated) setCb(component);
        }


        public override void Clear()
        {
            foreach(T cp in cpActivated) GameObject.Destroy(cp);
            cpActivated.Clear();
            foreach(T cp in cpUnactivated) GameObject.Destroy(cp);
            cpUnactivated.Clear();
        }
    }
}