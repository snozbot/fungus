using Object = System.Object;

namespace Fungus.LionManeSaveSys
{
    /// <summary>
    /// Interface for classes that represent units of save data, be they for a specific system, gameplay mechanic, etc.
    /// </summary>
    public interface ISaveUnit
    {
        Object Contents { get; }
        string TypeName { get; }
    }

    public interface ISaveUnit<TContents> : ISaveUnit
    {
        new TContents Contents { get; }
    }

    public interface ICommandSaveUnit : ISaveUnit
    {
        int Index { get; }

        /// <summary>
        /// Specifically at the time that this save unit was made
        /// </summary>
        bool WasExecuting { get; }
    }

    public interface ICommandSaveUnit<TContents> : ICommandSaveUnit, ISaveUnit<TContents>
    { }
}