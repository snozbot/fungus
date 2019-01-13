using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public class GenericCollection<T> : Collection
    {
        [SerializeField]
        protected List<T> collection = new List<T>();

        protected virtual T Promote(object o)
        {
            if (o is T)
            {
                return (T)o;
            }
            else if (o is VariableBase<T>)
            {
                var oAs = o as VariableBase<T>;
                return (T)oAs.Value;
            }

            Debug.LogError("Collection cannot promote " + o.GetType().Name + " to " + typeof(T).Name);
            return default(T);
        }

        public override bool IsCompatible(object o)
        {
            return o is T || o is VariableBase<T>;
        }

        public override void Add(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                collection.Add(t);
            }
        }

        public override void AddUnique(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                if (!collection.Contains(t))
                {
                    collection.Add(t);
                }
            }
        }

        public override void Clear()
        {
            collection.Clear();
        }

        public override Type ContainedType()
        {
            return typeof(T);
        }

        public override void Get(int index, ref object out_o)
        {
            var t = Promote(out_o);
            if (t != null)
            {
                out_o = collection[index];
            }
        }

        public override void Remove(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                collection.Remove(t);
            }
        }

        public override void RemoveAll(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                collection.RemoveAll(x => x.Equals(t));
            }
        }

        public override void RemoveAt(int index)
        {
            collection.RemoveAt(index);
        }

        public override void Set(int index, object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                collection[index] = t;
            }
        }

        public override int Count()
        {
            return collection.Count;
        }
    }
}