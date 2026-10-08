using System.Collections.Generic;
using Enum;
using SingletonBase;
using UnityEngine.Events;

namespace Event
{
    public class EventCenter : SingletonBase<EventCenter>
    {
        /// <summary>
        /// 记录所有事件对象的字典
        /// </summary>
        private Dictionary<E_EventType, EventBase> _eventDictionary = new Dictionary<E_EventType, EventBase>();
    
        private EventCenter(){}
    
        /// <summary>
        /// 触发无参数事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public void EventTrigger(E_EventType eventType)
        {
            if (_eventDictionary.ContainsKey(eventType))
            {
                (_eventDictionary[eventType] as EventObject).actions?.Invoke();
            }
        }

        /// <summary>
        /// 触发有参数事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="param">传递的参数</param>
        /// <typeparam name="T">参数类型</typeparam>
        public void EventTrigger<T>(E_EventType eventType, T param)
        {
            if (_eventDictionary.ContainsKey(eventType))
            {
                (_eventDictionary[eventType] as EventObject<T>).actions?.Invoke(param);
            }
        }
    
        /// <summary>
        /// 为指定的无参数事件添加监听者
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="action">监听者函数</param>
        public void AddEventListener(E_EventType eventType, UnityAction action)
        {
            if (_eventDictionary.ContainsKey(eventType))
            {
                (_eventDictionary[eventType] as EventObject).actions += action;
            }
            else
            {
                _eventDictionary.Add(eventType, new EventObject(action));
            }
        }

        /// <summary>
        /// 为指定的有参数事件添加监听者
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="action">监听者函数</param>
        /// <typeparam name="T">要传递的参数</typeparam>
        public void AddEventListener<T>(E_EventType eventType, UnityAction<T> action)
        {
            if (_eventDictionary.ContainsKey(eventType))
            {
                (_eventDictionary[eventType] as EventObject<T>).actions += action;
            }
            else
            {
                _eventDictionary.Add(eventType, new EventObject<T>(action));
            }
        }

        /// <summary>
        /// 移除指定无参数事件的某个监听者
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="action">要移除的监听者函数</param>
        public void RemoveEventListener(E_EventType eventType, UnityAction action)
        {
            if (_eventDictionary.ContainsKey(eventType))
            {
                (_eventDictionary[eventType] as EventObject).actions -= action;
            }
        }

        /// <summary>
        /// 移除指定有参数事件的某个监听者
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="action">要移除的监听者函数</param>
        /// <typeparam name="T">事件的参数类型</typeparam>
        public void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> action)
        {
            if (_eventDictionary.ContainsKey(eventType))
            {
                (_eventDictionary[eventType] as EventObject<T>).actions -= action;
            }
        }
    
        /// <summary>
        /// 清空所有事件的监听者
        /// </summary>
        public void Clear()
        {
            _eventDictionary.Clear();
        }

        /// <summary>
        /// 清除某一个事件的所有监听者
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public void Clear(E_EventType eventType)
        {
            _eventDictionary.Remove(eventType);
        }
    }
}
