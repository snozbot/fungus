// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The meat of the Fungus Collection. Internally uses a List of given type, simplest example
    /// being IntCollection. Provides some type specific additions to the base Collection for
    /// increasing speed and safety.
    ///
    /// Uses Promote methods to convert from objects or other collection or Fungus.Variable types
    /// being passed in, will attempt to do compatability for you, such as if you give an
    /// IntCollection an int or a Fungus.IntVariable, either works as the Promote is aware
    /// of Fungus.VariableBase<T>. Will also allow mixing some operations between
    /// GenericCollection<T>, T[], and List<T>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [AddComponentMenu("")]
    public class GenericCollection<T> : Collection
    {
        [SerializeField]
        protected System.Collections.Generic.List<T> collection = new System.Collections.Generic.List<T>();

        public override int Capacity
        {
            get
            {
                return collection.Capacity;
            }
            set
            {
                collection.Capacity = value;
            }
        }

        public override int Count { get { return collection.Count; } }

        public override int Add(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                return Add(t);
            }
            return -1;
        }

        public int Add(T t)
        {
            return (collection as System.Collections.IList).Add(t);
        }

        public override void Add(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                for (int i = 0; i < rhs.collection.Count; i++)
                {
                    Add(rhs.collection[i]);
                }
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

        public void AddUnique(T t)
        {
            if (!collection.Contains(t))
            {
                Add(t);
            }
        }

        public override void AddUnique(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                for (int i = 0; i < rhs.collection.Count; i++)
                {
                    AddUnique(rhs.collection[i]);
                }
            }
        }

        public override void Clear()
        {
            collection.Clear();
        }

        public override System.Type ContainedType()
        {
            return typeof(T);
        }

        public override bool Contains(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                return collection.Contains(t);
            }
            return false;
        }

        public override bool ContainsAllOf(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                for (int i = 0; i < rhs.collection.Count; i++)
                {
                    if (!collection.Contains(rhs.collection[i]))
                        return false;
                }

                return true;
            }

            return false;
        }

        public override bool ContainsAllOfOrdered(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                if (rhs.Count == Count)
                {
                    for (int i = 0; i < rhs.collection.Count; i++)
                    {
                        if (!rhs.collection[i].Equals(collection[i]))
                            return false;
                    }
                    return true;
                }
            }

            return false;
        }

        public override bool ContainsAnyOf(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                for (int i = 0; i < rhs.collection.Count; i++)
                {
                    if (collection.Contains(rhs.collection[i]))
                        return true;
                }
            }

            return false;
        }

        public override void CopyFrom(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                collection.Clear();
                collection.AddRange(rhs.collection);
            }
        }

        public override void CopyFrom(Array array)
        {
            foreach (var item in array)
            {
                Add(item);
            }
        }

        public override void CopyFrom(IList list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public override void CopyTo(System.Array array, int index)
        {
            (collection as System.Collections.IList).CopyTo(array, index);
        }

        public override void Exclusive(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                //if its in both, remove, if its only in 1 keep
                for (int i = 0; i < rhs.collection.Count; i++)
                {
                    var item = rhs.collection[i];
                    if (!collection.Remove(item))
                    {
                        collection.Add(item);
                    }
                }
            }
        }

        public override object Get(int index)
        {
            return collection[index];
        }

        public override void Get(int index, ref Variable variable)
        {
            if (variable is VariableBase<T>)
            {
                VariableBase<T> vt = variable as VariableBase<T>;
                vt.Value = collection[index];
            }
            else
            {
                Debug.LogError("Collection cannot get variable " + variable.Key + ". Is not matching type:" + typeof(T).Name);
            }
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public virtual T GetSafe(int index)
        {
            return collection[index];
        }

        public override int IndexOf(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                return collection.IndexOf(t);
            }
            return -1;
        }

        public override void Insert(int index, object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                collection.Insert(index, t);
            }
        }

        public override void Intersection(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                //only things that are in us and in rhs remain
                for (int i = collection.Count - 1; i >= 0; i--)
                {
                    if (!rhs.Contains(collection[i]))
                    {
                        collection.RemoveAt(i);
                    }
                }
            }
        }

        public override bool IsCollectionCompatible(object o)
        {
            if (o is GenericCollection<T> || o is System.Collections.Generic.IList<T>)
                return true;

            var ot = o.GetType();
            var ote = ot.GetElementType();
            var otgs = ot.GetGenericArguments();

            //element type only works for arrays, need to use getgenerictype with ilist<>T
            if (o is System.Array)
            {
                return ote is T || ote is Fungus.VariableBase<T>;
            }
            else if (o is System.Collections.IList && otgs.Length > 0)
            {
                return otgs[0] == typeof(T) || otgs[0].IsSubclassOf(typeof(Fungus.VariableBase<T>));
            }
            else
            {
                return false;
            }
        }

        public override bool IsElementCompatible(object o)
        {
            return o is T || o is VariableBase<T>;
        }

        public override int LastIndexOf(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                return collection.LastIndexOf(t);
            }
            return -1;
        }

        public override int Occurrences(object o)
        {
            int retval = 0;
            var t = Promote(o);
            if (t != null)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].Equals(t))
                        retval++;
                }
            }
            return retval;
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

        public override void RemoveAll(IFungusCollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                for (int i = rhsCol.Count - 1; i >= 0; i--)
                {
                    collection.RemoveAll(x => x.Equals(rhs.collection[i]));
                }
            }
        }

        public override void RemoveAt(int index)
        {
            collection.RemoveAt(index);
        }

        public override void Reserve(int count)
        {
            collection.Capacity = count;
        }

        public override void Resize(int count)
        {
            var toAdd = count - collection.Count;
            if (toAdd > 0)
                collection.AddRange(System.Linq.Enumerable.Repeat(default(T), toAdd));
        }

        public override void Reverse()
        {
            collection.Reverse();
        }

        public override void Set(int index, object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                collection[index] = t;
            }
        }

        public virtual void Set(int index, T value)
        {
            collection[index] = value;
        }

        public override void Shuffle()
        {
            for (int i = 0; i < collection.Count; i++)
            {
                var tmp = collection[i];
                var rnd = UnityEngine.Random.Range(0, collection.Count);
                collection[i] = collection[rnd];
                collection[rnd] = tmp;
            }
        }

        public override void Sort()
        {
            collection.Sort();
        }

        public override void Unique()
        {
            collection = collection.Distinct().ToList();
        }

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

        protected virtual GenericCollection<T> Promote(IFungusCollection col)
        {
            if (col is GenericCollection<T>)
            {
                return (GenericCollection<T>)col;
            }

            Debug.LogError("Collection cannot promote " + col.GetType().Name + " to " + this.GetType().Name);
            return null;
        }
    }
}