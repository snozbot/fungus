// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Provides a common and complete MonoBehavior based reference point for FungusCollection.
    /// Fungus.GenericCollection inherits from this.
    /// </summary>
    [AddComponentMenu("")]
    [System.Serializable]
    public abstract class Collection : MonoBehaviour, IFungusCollection
    {
        public abstract int Capacity { get; set; }
        public abstract int Count { get; }
        public bool IsFixedSize { get { return false; } }
        public bool IsReadOnly { get { return false; } }
        public bool IsSynchronized { get { return false; } }
        public object SyncRoot { get { return null; } }
        public string Name { get { return name; } }

        public object this[int index] { get { return Get(index); } set { Set(index, value); } }

        public abstract int Add(object o);

        public abstract void Add(IFungusCollection rhsCol);

        public abstract void AddUnique(object o);

        public abstract void AddUnique(IFungusCollection rhsCol);

        public abstract void Clear();

        public abstract Type ContainedType();

        public abstract bool Contains(object o);

        public abstract bool ContainsAllOf(IFungusCollection rhsCol);

        public abstract bool ContainsAllOfOrdered(IFungusCollection rhsCol);

        public abstract bool ContainsAnyOf(IFungusCollection rhsCol);

        public abstract void CopyFrom(IFungusCollection rhsCol);

        public abstract void CopyFrom(System.Array array);

        public abstract void CopyFrom(System.Collections.IList list);

        public abstract void CopyTo(Array array, int index);

        public abstract void Exclusive(IFungusCollection rhsCol);

        public abstract object Get(int index);

        public abstract void Get(int index, ref Variable variable);

        public abstract IEnumerator GetEnumerator();

        public abstract int IndexOf(object o);

        public abstract void Insert(int index, object o);

        public abstract void Intersection(IFungusCollection rhsCol);

        public abstract bool IsCollectionCompatible(object o);

        public abstract bool IsElementCompatible(object o);

        public abstract int LastIndexOf(object o);

        public abstract int Occurrences(object o);

        public abstract void Remove(object o);

        public abstract void RemoveAll(IFungusCollection rhsCol);

        public abstract void RemoveAll(object o);

        public abstract void RemoveAt(int index);

        public abstract void Reserve(int count);

        public abstract void Resize(int count);

        public abstract void Reverse();

        public abstract void Set(int index, object o);

        public abstract void Shuffle();

        public abstract void Sort();

        public abstract void Unique();
    }
}