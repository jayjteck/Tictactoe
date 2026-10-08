using UnityEngine.Events;

namespace Event
{
    /// <summary>
    /// 无参数事件对象
    /// </summary>
    public class EventObject :EventBase
    {
        public UnityAction actions;
    
        public EventObject(UnityAction action)
        {
            this.actions += action;
        }
    }

    /// <summary>
    /// 有参数事件
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public class EventObject<T> : EventBase
    {
        public UnityAction<T> actions;
    
        public EventObject(UnityAction<T> action)
        {
            this.actions += action;
        }
    }
}