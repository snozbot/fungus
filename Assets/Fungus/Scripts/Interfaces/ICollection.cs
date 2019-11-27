using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public interface ICollection : System.Collections.IList
    {
        System.Type ContainedType();

        bool IsCompatible(object o);
        
        void Add(ICollection rhsCol);
        
        void AddUnique(object o);

        void AddUnique(ICollection rhsCol);

        void RemoveAll(ICollection rhsCol);
        
        void RemoveAll(object o);
        
        object Get(int index);

        void Get(int index, ref Variable variable);

        void Set(int index, object o);

        void Shuffle();

        void Reverse();

        void Sort();
        
        bool ContainsAllOf(ICollection rhsCol);

        bool ContainsAllOfOrdered(ICollection rhsCol);

        bool ContainsAnyOf(ICollection rhsCol);
        
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