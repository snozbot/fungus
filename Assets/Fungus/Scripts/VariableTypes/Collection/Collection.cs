using System;
using System.Collections;
using UnityEngine;

namespace Fungus
{
    //N.B. is a monobeh presently, but may not remain so
    [AddComponentMenu("")]
    [System.Serializable]
    public abstract class Collection : MonoBehaviour, ICollection
    {
        public object this[int index] { get => Get(index); set => Set(index, value); }
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot => null;
        public abstract int Count { get; }
        public abstract int Add(object o);
        public abstract void Add(ICollection rhsCol);
        public abstract void AddUnique(object o);
        public abstract void AddUnique(ICollection rhsCol);
        public abstract int Capacity();
        public abstract void Clear();
        public abstract Type ContainedType();
        public abstract bool Contains(object o);
        public abstract bool ContainsAllOf(ICollection rhsCol);
        public abstract bool ContainsAllOfOrdered(ICollection rhsCol);
        public abstract bool ContainsAnyOf(ICollection rhsCol);
        public abstract void CopyFrom(ICollection rhsCol);
        public abstract void CopyTo(Array array, int index);
        public abstract void Exclusive(ICollection rhsCol);
        public abstract object Get(int index);
        public abstract void Get(int index, ref Variable variable);
        public abstract IEnumerator GetEnumerator();
        public abstract int IndexOf(object o);
        public abstract void Insert(int index, object o);
        public abstract void Intersection(ICollection rhsCol);
        public abstract bool IsCompatible(object o);
        public abstract int LastIndexOf(object o);
        public abstract int Occurrences(object o);
        public abstract void Remove(object o);
        public abstract void RemoveAll(ICollection rhsCol);
        public abstract void RemoveAll(object o);
        public abstract void RemoveAt(int index);
        public abstract void Reserve(int count);
        public abstract void Resize(int count);
        public abstract void Reverse();
        public abstract void Set(int index, object o);
        public abstract void Shuffle();
        public abstract void Sort();
    }
}