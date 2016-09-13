using MoonSharp.Interpreter;

namespace Fungus
{
    public interface ILuaStore
    {
        /// <summary>
        /// A Lua table that can be shared between multiple LuaEnvironments.
        /// </summary>
        Table PrimeTable { get; }
    }
}