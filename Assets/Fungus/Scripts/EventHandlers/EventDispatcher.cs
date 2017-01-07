using System.Collections.Generic;
using System;

namespace Fungus
{
    public class EventDispatcher 
    {
        public delegate void TypedDelegate<T>(T e) where T : class;

        #region Statics
        static EventDispatcher _instance;
        public static EventDispatcher Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EventDispatcher();
                }
                return _instance;
            }
        }

        public static void AddLog(Action<string> log)
        {
            Instance.addLog(log);
        }

        public static void RemoveLog(Action<string> log)
        {
            Instance.removeLog(log);
        }

        public static void AddListener<T>(TypedDelegate<T> listener) where T : class
        {
            Instance.addListener(listener);
        }

        public static void RemoveListener<T>(TypedDelegate<T> listener) where T : class
        {
            Instance.removeListener(listener);
        }

        public static void Raise<T>(T evt) where T : class
        {
            Instance.raise(evt);
        }

        public static void Raise<T>() where T : class, new()
        {
            Instance.raise<T>(new T());
        }

        public static void UnregisterAll()
        {
            Instance.unregisterAll();
        }
        #endregion

        #region Private Members
        readonly Dictionary<Type, List<Delegate>> _delegates;
        event Action<string> _onLog;
        #endregion

        #region Private Functions
        private EventDispatcher()
        {
            _delegates = new Dictionary<Type, List<Delegate>>();
        }
        /// <summary>
        /// Gets the delegate list copy.
        /// </summary>
        /// <remarks>
        /// As listener can modify the list while iterating it, it is better to iterate a copy of the delegates list instead of a reference.
        /// </remarks>
        /// <returns>A copy of the delegates list if found. Null if the dictionary does not contain a delegate list for this event.</returns>
        /// <param name="evt">Event instance.</param>
        /// <typeparam name="T">Type of the received event.</typeparam>
        List<Delegate> getDelegateListCopy<T>(T evt)
        {
            var type = typeof(T);
            return _delegates.ContainsKey(type) ? new List<Delegate>(_delegates[type]) : null;
        }

        void log(string message)
        {
            if(_onLog != null)
            {
                _onLog(message);
            }
        }
        #endregion

        #region Public Functions
        public void addLog(Action<string> log)
        {
            _onLog += log;
        }

        public void removeLog(Action<string> log)
        {
            _onLog -= log;
        }

        public void addListener<T>(TypedDelegate<T> listener) where T : class
        {
            var type = typeof(T);
            if(!_delegates.ContainsKey(type))
            {
                _delegates.Add(type, new List<Delegate>());
            }

            var list = _delegates[type];
            if(!list.Contains(listener))
            {
                list.Add(listener);
            }
        }

        public void removeListener<T>(TypedDelegate<T> listener) where T : class
        {
            var type = typeof(T);
            if(_delegates.ContainsKey(type))
            {
                _delegates[type].Remove(listener);
                return;
            }
        }

        public void raise<T>(T evt) where T : class
        {
            if(evt == null)
            {
                log("Raised a null event");
                return;
            }

            var list = getDelegateListCopy(evt);
            if(list == null || list.Count < 1)
            {
                log("Raised an event with no listeners");
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
                        log(gotcha.Message);
                    }
                }
            }
        }

        public void raise<T>() where T : class, new()
        {
            raise<T>(new T());
        }

        public void unregisterAll()
        {
            _delegates.Clear();
        }
        #endregion
    }
}