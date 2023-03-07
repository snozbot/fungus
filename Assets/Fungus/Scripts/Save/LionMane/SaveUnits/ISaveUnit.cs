using Object = System.Object;
using System.Collections.Generic;

namespace Fungus.LionManeSaveSys
{
    /// <summary>
    /// Interface for classes that represent units of save data, be they for a specific system, gameplay mechanic, etc.
    /// </summary>
    public interface ISaveUnit
    {
        string TypeName { get; }
        IList<ISaveUnit> Subunits { get; }
    }


    public interface ICommandSaveUnit : ISaveUnit
    {
        int Index { get; }

        /// <summary>
        /// Specifically at the time that this save unit was made
        /// </summary>
        bool WasExecuting { get; }
    }

    public interface ICommandSaveUnit<TContents> : ICommandSaveUnit
    { }
}