namespace Fungus
{
    public interface IFungusCollection : System.Collections.IList
    {
        string name { get; }

        void Add(IFungusCollection rhsCol);

        void AddUnique(object o);

        void AddUnique(IFungusCollection rhsCol);

        int Capacity();

        System.Type ContainedType();

        bool ContainsAllOf(IFungusCollection rhsCol);

        bool ContainsAllOfOrdered(IFungusCollection rhsCol);

        bool ContainsAnyOf(IFungusCollection rhsCol);

        void CopyFrom(IFungusCollection rhsCol);

        void Exclusive(IFungusCollection rhsCol);

        object Get(int index);

        void Get(int index, ref Variable variable);

        void Intersection(IFungusCollection rhsCol);

        bool IsCompatible(object o);

        int LastIndexOf(object o);

        int Occurrences(object o);

        void RemoveAll(IFungusCollection rhsCol);

        void RemoveAll(object o);

        void Reserve(int count);

        void Resize(int count);

        void Reverse();

        void Set(int index, object o);

        void Shuffle();

        void Sort();
    }
}