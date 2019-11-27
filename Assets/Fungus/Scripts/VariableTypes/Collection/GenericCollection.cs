using System;
using System.Collections;
using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public class GenericCollection<T> : Collection
    {
        [SerializeField]
        protected System.Collections.Generic.List<T> collection = new System.Collections.Generic.List<T>();

        public override int Count => collection.Count;

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

        protected virtual GenericCollection<T> Promote(ICollection col)
        {
            if (col is GenericCollection<T>)
            {
                return (GenericCollection<T>)col;
            }

            Debug.LogError("Collection cannot promote " + col.GetType().Name + " to " + this.GetType().Name);
            return null;
        }

        public override bool IsCompatible(object o)
        {
            return o is T || o is VariableBase<T>;
        }

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
            return (collection as IList).Add(t);
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

        public override void Clear()
        {
            collection.Clear();
        }

        public override System.Type ContainedType()
        {
            return typeof(T);
        }

        public override object Get(int index)
        {
            return collection[index];
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

        public override void Reverse()
        {
            collection.Reverse();
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

        public override int IndexOf(object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                return collection.IndexOf(t);
            }
            return -1;
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

        public override void Get(int index, ref Variable variable)
        {
            if(variable is VariableBase<T>)
            {
                VariableBase<T> vt = variable as VariableBase<T>;
                vt.Value = collection[index];
            }
            else
            {
                Debug.LogError("Collection cannot get variable " + variable.Key + ". Is not matching type:" + typeof(T).Name);
            }
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

        public override void Insert(int index, object o)
        {
            var t = Promote(o);
            if (t != null)
            {
                collection.Insert(index, t);
            }
        }

        public override void Sort()
        {
            collection.Sort();
        }

        public override void Reserve(int count)
        {
            collection.Capacity = count;
        }

        public override void Resize(int count)
        {
            var toAdd = count - collection.Count;
            if(toAdd > 0)
                collection.AddRange(System.Linq.Enumerable.Repeat(default(T), toAdd));
        }

        public override int Capacity()
        {
            return collection.Capacity;
        }

        public override void RemoveAll(ICollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                for (int i = 0; i < rhs.collection.Count; i++)
                {
                    collection.RemoveAll(x => x.Equals(rhs.collection[i]));
                }
            }
        }

        public override bool ContainsAllOf(ICollection rhsCol)
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

        public override bool ContainsAllOfOrdered(ICollection rhsCol)
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

        public override void Add(ICollection rhsCol)
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

        public override void AddUnique(ICollection rhsCol)
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

        public override bool ContainsAnyOf(ICollection rhsCol)
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

        public override void Intersection(ICollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                //only things that are in us and in rhs remain
                for (int i = collection.Count-1; i >=0; i--)
                {
                    if(!rhs.Contains(collection[i]))
                    {
                        collection.RemoveAt(i);
                    }
                }
            }
        }

        public override void Exclusive(ICollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                //if its in both, remove, if its only in 1 keep
                for (int i = 0; i <  rhs.collection.Count;  i++)
                {
                    var item = rhs.collection[i];
                    if (!collection.Remove(item))
                    {
                        collection.Add(item);
                    }
                }
            }
        }

        public override void CopyFrom(ICollection rhsCol)
        {
            var rhs = Promote(rhsCol);
            if (rhs != null)
            {
                collection.Clear();
                collection.AddRange(rhs.collection);
            }
        }

        public override void CopyTo(Array array, int index)
        {
            (collection as IList).CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}