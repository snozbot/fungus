// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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