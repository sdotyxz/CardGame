using System;
using System.Collections.Generic;

using UnityEngine;
using System.Collections;
using Core.Src.Interface;

namespace Core.Src.Manager
{
    //C# Unity event manager that uses strings in a hashtable over delegates and events in order to
    //allow use of events without knowing where and when they're declared/defined.
    //by Billy Fletcher of Rubix Studios

    public delegate void EventListener(params object[] args);
    
    class Event
    {
        private object _context;
        public object context
        {
            get { return _context; }
        }
        private string _name;
        public string name
        {
            get { return _name; }
        }
        private object[] _data;
        public object[] data
        {
            get { return _data; }
        }
        public Event(object context, string name, object[] data)
        {
            _context = context;
            _name = name;
            _data = data;
        }
    }

    class EventReference
    {
        private WeakReference _weakReference;
        public Delegate listener
        {
            get
            {
                if (_listener != null)
                {
                    return _listener;
                }
                else if (_weakReference != null)
                {
                    return _weakReference.Target as Delegate;
                }
                return null;
            }
        }
        private Delegate _listener;
        public bool isOnce { get { return _isOnce; } }
        private bool _isOnce;
        public EventReference(Delegate listener, bool useWeakReference, bool isOnce)
        {
            if (useWeakReference) {
                _weakReference = new WeakReference(listener);
            }else{
                _listener = listener;
            }
            _isOnce = isOnce;
        }
    }

    public class EventManager:ITickObject
    {
        public float weakReferenceGCDelay = 30f;
        private float _weakReferenceGCTime = 0.0f;
        public bool LimitQueueProcesing = false;
        public float QueueProcessTime = 0.0f;
        private Hashtable _listenerTable = new Hashtable();
        private Queue _eventQueue = new Queue();
        private object lockObject = new object();

        private static EventManager _instance = null;
        public static EventManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventManager();
                }

                return _instance;
            }
        }

        public void Init()
        {
            Engine.AddTickObject(this);
        }

        private List<EventReference> _GetEventListenerList(object context, string eventName, bool createIfNotExist = false)
        { 
            if (!_listenerTable.ContainsKey(context))
            {
                if (!createIfNotExist) {
                    return null;
                }
                _listenerTable.Add(context, new Dictionary<string, List<EventReference>>());
            }
            IDictionary<string, List<EventReference>> dictionary = _listenerTable[context] as IDictionary<string, List<EventReference>>;
            if (!dictionary.ContainsKey(eventName))
            {
                if (!createIfNotExist) {
                    return null;
                }
                dictionary[eventName] = new List<EventReference>();
            }
            return dictionary[eventName];
        }

        public static bool HasListener(object context, string eventName, Delegate listener)
        {
            return instance.HasEventListener(context, eventName, listener);
        }

        public bool HasEventListener(object context, string eventName, Delegate listener)
        {
            if (!_listenerTable.ContainsKey(context))
                return false;

            List<EventReference> list = _GetEventListenerList(context, eventName);
            if (list == null)
            {
                return false;
            }
            if (!list.Exists(l => l.listener == listener))
            {
                return false;
            }
            return true;
        }

        public static bool AddListener<T>(object context, string eventName, T listener, bool useWeakReference = false, bool isOnce = false)
        {
            return _instance._AddEventListener(context, eventName, listener as Delegate, useWeakReference, isOnce);
        }
        public static bool AddListener(object context, string eventName, EventListener listener, bool useWeakReference = false, bool isOnce = false)
        {
            return _instance._AddEventListener(context, eventName, listener, useWeakReference, isOnce);
        }

        //Add a listener to the event manager that will receive any events of the supplied event name.
        public bool AddEventListener<T>(object context, string eventName, T listener, bool useWeakReference = false, bool isOnce = false)
        {
            return _AddEventListener(context, eventName, listener as Delegate, useWeakReference, isOnce);
        }

        public bool AddEventListener(object context, string eventName, EventListener listener, bool useWeakReference = false, bool isOnce = false)
        {
            return _AddEventListener(context, eventName, listener, useWeakReference, isOnce);
        }

        private bool _AddEventListener(object context, string eventName, Delegate listener, bool useWeakReference = false, bool isOnce = false)
        {
            if (context == null || listener == null || string.IsNullOrEmpty(eventName))
            {
                Log.Error("Event Manager: AddListener failed due to no listener or event name specified.");
                return false;
            }

            List<EventReference> list = _GetEventListenerList(context, eventName, true);
            if (list.Exists(l => l.listener == listener))
            {
                Log.Error("Event Manager: Listener: " + listener.GetType().ToString() + " is already in list for event: " + eventName);
                return false; //listener already in list
            }

            list.Add(new EventReference(listener, useWeakReference, isOnce));
            return true;
        }

        public static bool RemoveListener<T>(object context, string eventName, T listener)
        {
            return instance._RemoveEventListener(context, eventName, listener as Delegate);
        }

        public static bool RemoveListener(object context, string eventName, EventListener listener)
        {
            return instance._RemoveEventListener(context, eventName, listener);
        }

        public bool RemoveEventListener(object context, string eventName, EventListener listener)
        {
            return _RemoveEventListener(context, eventName, listener);
        }

        //Remove a listener from the subscribed to event.
        public bool RemoveEventListener<T>(object context, string eventName, T listener)
        {
            return _RemoveEventListener(context, eventName, listener as Delegate);
        }

        private bool _RemoveEventListener(object context, string eventName, Delegate listener)
        {
            if (!_listenerTable.ContainsKey(context))
                return false;

            List<EventReference> list = _GetEventListenerList(context, eventName);
            if (list == null)
            {
                return false;
            }
            EventReference eventReference = list.Find(l => l.listener == listener);
            if (eventReference == null)
            {
                return false;
            }
            list.Remove(eventReference);
            if (list.Count <= 0)
            {
                IDictionary<string, List<EventReference>> dictionary = _listenerTable[context] as IDictionary<string, List<EventReference>>;
                dictionary.Remove(eventName);
                if (dictionary.Count <= 0)
                {
                    _listenerTable.Remove(context);
                }
            }
            return true;
        }

        public static bool Trigger(object context, string eventName, params object[] data)
        {
            return _instance._TriggerEvent(context, eventName, data);
        }

        //Trigger the event instantly, this should only be used in specific circumstances,
        //the QueueEvent function is usually fast enough for the vast majority of uses.
        public bool TriggerEvent(object context, string eventName, params object[] data)
        {
            return _TriggerEvent(context, eventName, data);
        }

        private bool _TriggerEvent(object context, string eventName, object[] data)
        {
            if (!_listenerTable.ContainsKey(context))
            {
                Log.Error("Event Manager: Event \"" + eventName + "\" triggered has no listeners!");
                return false; //No listeners for event so ignore it
            }

            List<EventReference> list = _GetEventListenerList(context, eventName);
            if (list == null) {
                return false;
            }

            list.ForEach(l =>
            {
                Delegate listener = l.listener;
                if (listener != null)
                {
                    try
                    {
                        if (listener is EventListener)
                        {
                            (listener as EventListener)(data);
                        }
                        else
                        {
                            listener.DynamicInvoke(data);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("EventManager:TriggerEvent - " + e.ToString());
                    }

                    if (l.isOnce)
                    {
                        _RemoveEventListener(context, eventName, listener);
                    }
                }
            });
            return true;
        }

        public static bool Dispatch(object context, string eventName, params object[] data)
        {
            return _instance._DispatchEvent(context, eventName, data);
        }

        //Inserts the event into the current queue.
        public bool DispatchEvent(object context, string eventName, params object[] data)
        {
            return _DispatchEvent(context, eventName, data);
        }

        private bool _DispatchEvent(object context, string eventName, object[] data)
        {
            if (!_listenerTable.ContainsKey(context))
            {
                Log.Error("EventManager: QueueEvent failed due to no listeners for event: " + eventName);
                return false;
            }

            lock (lockObject)
            {
                _eventQueue.Enqueue(new Event(context, eventName, data));
            }
            return true;
        }

        /// <summary>
        /// @Private
        /// Called By Engine
        /// Every update cycle the queue is processed, if the queue processing is limited,
        /// a maximum processing time per update can be set after which the events will have
        /// to be processed next update loop.
        /// </summary>
        public void Update()
        {
            float timer = 0.0f;
            while (_eventQueue.Count > 0)
            {
                if (LimitQueueProcesing)
                {
                    if (timer > QueueProcessTime)
                        return;
                }

                Event evt = null;
                lock(lockObject)
                {
                    evt = _eventQueue.Dequeue() as Event;
                }
                _TriggerEvent(evt.context, evt.name, evt.data);
                //if (!_TriggerEvent(evt.context, evt.name, evt.data));
                //    Logger.Debug("", "Not exist event listener when processing event: " + evt.name);

                if (LimitQueueProcesing)
                    timer += Time.deltaTime;
            }
            _weakReferenceGCTime += Time.deltaTime;
            if (_weakReferenceGCTime > weakReferenceGCDelay) {
                _weakReferenceGCTime = 0.0f;
                _GCWeakReference();
            }
        }

        private void _GCWeakReference()
        {
            IDictionary<string, List<EventReference>> dictionary;
            List<EventReference> list;
            List<EventReference> gcList = new List<EventReference>();
            List<string> gcDictionary = new List<string>();
            List<object> gcTable = new List<object>();
            foreach (DictionaryEntry entry in _listenerTable)
            {
                dictionary = entry.Value as IDictionary<string, List<EventReference>>;
                foreach (KeyValuePair<string, List<EventReference>> pair in dictionary)
                {
                    list = pair.Value;
                    list.ForEach(l=>{
                        if (l.listener == null) {
                            gcList.Add(l);
                        }
                    });
                    gcList.ForEach(l=>list.Remove(l));
                    gcList.Clear();
                    if (list.Count <= 0) {
                        gcDictionary.Add(pair.Key);
                    }
                }
                gcDictionary.ForEach(d => dictionary.Remove(d));
                gcDictionary.Clear();
                if (dictionary.Count <= 0) {
                    gcTable.Add(entry.Key);
                }
            }
            gcTable.ForEach(t => _listenerTable.Remove(t));
            gcTable.Clear();
        }

        public void OnApplicationQuit()
        {
            _listenerTable.Clear();
            _eventQueue.Clear();
            _instance = null;
        }
    }
}
