using Object = System.Object;

namespace Fungus.LionManeSaveSys
{
    /// <summary>
    /// Interface for classes that represent units of save data, be they for a specific system, gameplay mechanic, etc.
    /// </summary>
    public interface ISaveUnit
    {
        Object Contents { get; }
    }

    public interface ISaveUnit<TContents> : ISaveUnit
    {
        new TContents Contents { get; }
    }
}