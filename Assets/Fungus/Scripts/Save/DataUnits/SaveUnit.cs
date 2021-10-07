using Object = System.Object;

namespace Fungus
{
    /// <summary>
    /// Has a default implementation for units of save data made to work with 
    /// Fungus's save system.
    /// </summary>
    /// <typeparam name="TSave"></typeparam>
    public abstract class SaveUnit<TSave> : ISaveUnit<TSave>
    {
        protected TSave contents;
        public virtual TSave Contents => contents;

        public virtual Object GetContents()
        {
            return contents;
        }
    }
}