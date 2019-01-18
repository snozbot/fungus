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

        public abstract void AddUnique(object o);

        public abstract void Remove(object o);

        public abstract void RemoveAt(int index);

        public abstract void RemoveAll(object o);

        public abstract void Clear();

        public abstract void Get(int index, ref object out_o);

        public abstract void Set(int index, object o);

        public abstract int Count();

        public abstract void Shuffle();

        public abstract void Reverse();

        public abstract bool Contains(object o);

        public abstract int IndexOf(object o);

        public abstract int LastIndexOf(object o);

        //sort
        //merge
        //exclusive
        //subtract
    }
}