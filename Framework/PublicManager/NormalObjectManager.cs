using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProjectUtil
{
    /*
        自定义类的对象管理
            专门负责管理需要重复创建且大量出现的自定义类对象（例如计时器）
            不同于RepPrefabObjManager。此类主要针对一些与Unity无关的自定义类
            同一个NormalObjectManager对象 [只能管理同一个类的对象]

        注：被管理对象的类必须支持无参构造
    */
    public interface INormalObjectManager<T>
    {
        public T Get(Action<T> callback);
        public void Recycle(T ob);
        public void Clear();
    }


    public class NormalObjectManager<T> : Manager<T>, INormalObjectManager<T>
    {
        protected List<T> workingList;            // 正在使用的类对象
        protected Queue<T> availableQue;          // 未被使用的类对象
        Func<T> create;                  // 目标类的构造函数

        public NormalObjectManager(Func<T> ctor) : base(null)
        {
            create = ctor;
        }
        protected override void initRelativeSystems()
        {

        }
        

        public virtual T Get(Action<T> callback)
        {
            T ret;
            if(availableQue.Count==0)
            {
                ret = create();
                Recycle(ret);
            }

            ret = availableQue.Dequeue();
            workingList.Add(ret);
            callback?.Invoke(ret);
            return ret;
        }

        public void Recycle(T ob)
        {
            if(workingList.Contains(ob)) workingList.Remove(ob);
            availableQue.Enqueue(ob);
        }

        public override void Clear()
        {
            workingList.Clear();
            availableQue.Clear();
        }
    }
}