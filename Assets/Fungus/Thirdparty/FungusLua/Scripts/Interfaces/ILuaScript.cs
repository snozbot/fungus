// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Executes Lua script defined in a string property or in an external file.
    /// </summary>
    public interface ILuaScript 
    {
        /// <summary>
        /// Execute the Lua script.
        /// This is the function to call if you want to trigger execution from an external script.
        /// </summary>
        void OnExecute();
    }
}