// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Fungus
{
    /// <summary>
    /// A simple efficient event dispatcher with logging support.
    /// </summary>
    public class EventDispatcher : MonoBehaviour
    {
        protected readonly Dictionary<Type, List<Delegate>> delegates = new Dictionary<Type, List<Delegate>>();

        protected virtual event Action<string> onLog;

        /// <summary>
        /// Gets the delegate list copy.
        /// </summary>
        /// <remarks>
        /// As listener can modify the list while iterating it, it is better to iterate a copy of the delegates list instead of a reference.
        /// </remarks>
        /// <returns>A copy of the delegates list if found. Null if the dictionary does not contain a delegate list for this event.</returns>
        /// <param name="evt">Event instance.</param>
        /// <typeparam name="T">Type of the received event.</typeparam>
        protected virtual List<Delegate> GetDelegateListCopy<T>(T evt)
        {
            var type = typeof(T);
            return delegates.ContainsKey(type) ? new List<Delegate>(delegates[type]) : null;
        }

        protected virtual void Log(string message)
        {
            if(onLog != null)
            {
                onLog(message);
            }
        }

        #region Public members

        /// <summary>
        /// A typed delegate which contains information about the event.
        /// </summary>
        public delegate void TypedDelegate<T>(T e) where T : class;

        /// <summary>
        /// Adds a log callback action.
        /// </summary>
        public virtual void AddLog(Action<string> log)
        {
            onLog += log;
        }

        /// <summary>
        /// Removes a log callback action.
        /// </summary>
        public virtual void RemoveLog(Action<string> log)
        {
            onLog -= log;
        }

        /// <summary>
        /// Adds a listener for a specified event type.
        /// </summary>
        public virtual void AddListener<T>(TypedDelegate<T> listener) where T : class
        {
            var type = typeof(T);
            if(!delegates.ContainsKey(type))
            {
                delegates.Add(type, new List<Delegate>());
            }

            var list = delegates[type];
            if(!list.Contains(listener))
            {
                list.Add(listener);
            }
        }

        /// <summary>
        /// Removes a listener for a specified event type.
        /// </summary>
        public virtual void RemoveListener<T>(TypedDelegate<T> listener) where T : class
        {
            var type = typeof(T);
            if(delegates.ContainsKey(type))
            {
                delegates[type].Remove(listener);
                return;
            }
        }

        /// <summary>
        /// Raise an event of a specified type.
        /// </summary>
        public virtual void Raise<T>(T evt) where T : class
        {
            if(evt == null)
            {
                Log("Raised a null event");
                return;
            }

            var list = GetDelegateListCopy(evt);
            if(list == null || list.Count < 1)
            {
                Log("Raised an event with no listeners");
                return;
            }

            for(int i = 0; i < list.Count; ++i)
            {
                var callback = list[i] as TypedDelegate<T>;

                if(callback != null)
                {
                    try
                    {
                        callback(evt);
                    }
                    catch(Exception gotcha)
                    {
                        Log(gotcha.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Raise an event of a specified type, creates an instance of the type automatically.
        /// </summary>
        public virtual void Raise<T>() where T : class, new()
        {
            Raise<T>(new T());
        }

        /// <summary>
        /// Unregisters all event listeners.
        /// </summary>
        public virtual void UnregisterAll()
        {
            delegates.Clear();
        }

        #endregion
    }
}