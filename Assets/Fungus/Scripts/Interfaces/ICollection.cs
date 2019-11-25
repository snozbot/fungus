using UnityEngine;

namespace Fungus
{
    public interface ICollection
    {
        System.Type ContainedType();

        bool IsCompatible(object o);

        void Add(object o);

        void Add(ICollection rhsCol);

        void Insert(int index, object o);

        void AddUnique(object o);

        void AddUnique(ICollection rhsCol);

        void Remove(object o);

        void RemoveAll(ICollection rhsCol);

        void RemoveAt(int index);

        void RemoveAll(object o);

        void Clear();

        object Get(int index);

        void Get(int index, ref Variable variable);

        void Set(int index, object o);

        int Count();

        void Shuffle();

        void Reverse();

        void Sort();

        bool Contains(object o);

        bool ContainsAllOf(ICollection rhsCol);

        bool ContainsAllOfOrdered(ICollection rhsCol);

        bool ContainsAnyOf(ICollection rhsCol);

        int IndexOf(object o);

        int LastIndexOf(object o);

        int Occurrences(object o);

        void Reserve(int count);

        void Resize(int count);

        int Capacity();

        void Intersection(ICollection rhsCol);

        void Exclusive(ICollection rhsCol);

        void CopyFrom(ICollection rhsCol);

        string name { get; }
    }
}