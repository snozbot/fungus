using System;

namespace Fungus
{
    /// <summary>
    /// Interface for classes that represent units of save data, be they for a specific system, gameplay mechanic, etc.
    /// </summary>
    public interface ISaveUnit
    {
        /// <summary>
        /// For relatively type-independent retrieval of contents.
        /// </summary>
        Object GetContents();
    }

    public interface ISaveUnit<TContents> : ISaveUnit
    {
        TContents Contents { get; }
    }
}