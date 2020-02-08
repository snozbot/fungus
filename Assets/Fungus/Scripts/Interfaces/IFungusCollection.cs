// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Extension of IList for Fungus collections and support for associated commands.
    ///
    /// Built upon objects being passed in and returned as the base starting point.
    /// The inherited classes may wish to provided typed access to underlying container,
    /// this is what the Fungus.GenericCollection does.
    /// </summary>
    public interface IFungusCollection : System.Collections.IList
    {
        int Capacity { get; set; }
        string Name { get; }

        void Add(IFungusCollection rhsCol);

        void AddUnique(object o);

        void AddUnique(IFungusCollection rhsCol);

        System.Type ContainedType();

        bool ContainsAllOf(IFungusCollection rhsCol);

        bool ContainsAllOfOrdered(IFungusCollection rhsCol);

        bool ContainsAnyOf(IFungusCollection rhsCol);

        void CopyFrom(IFungusCollection rhsCol);

        void CopyFrom(System.Array array);

        void CopyFrom(System.Collections.IList list);

        void Exclusive(IFungusCollection rhsCol);

        object Get(int index);

        void Get(int index, ref Variable variable);

        void Intersection(IFungusCollection rhsCol);

        bool IsCollectionCompatible(object o);

        bool IsElementCompatible(object o);

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

        void Unique();
    }
}