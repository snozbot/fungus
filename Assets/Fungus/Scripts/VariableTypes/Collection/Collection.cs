using UnityEngine;

namespace Fungus
{
    //N.B. is a monobeh presently, but may not remain so
    [AddComponentMenu("")]
    public abstract class Collection : MonoBehaviour
    {
        public abstract System.Type ContainedType();

        public abstract bool IsCompatible(object o);

        public abstract void Add(object o);

        public abstract void Add(Collection rhsCol);

        public abstract void Insert(int index, object o);

        public abstract void AddUnique(object o);

        public abstract void AddUnique(Collection rhsCol);

        public abstract void Remove(object o);

        public abstract void RemoveAll(Collection rhsCol);

        public abstract void RemoveAt(int index);

        public abstract void RemoveAll(object o);

        public abstract void Clear();

        public abstract object Get(int index);

        public abstract void Get(int index, ref Variable variable);

        public abstract void Set(int index, object o);

        public abstract int Count();

        public abstract void Shuffle();

        public abstract void Reverse();

        public abstract void Sort();

        public abstract bool Contains(object o);

        public abstract bool ContainsAllOf(Collection rhsCol);

        public abstract bool ContainsAllOfOrdered(Collection rhsCol);

        public abstract bool ContainsAnyOf(Collection rhsCol);

        public abstract int IndexOf(object o);

        public abstract int LastIndexOf(object o);

        public abstract int Occurrences(object o);

        public abstract void Reserve(int count);

        public abstract void Resize(int count);

        public abstract int Capacity();

        public abstract void Intersection(Collection rhsCol);

        public abstract void Exclusive(Collection rhsCol);

        public abstract void CopyFrom(Collection rhsCol);
    }
}