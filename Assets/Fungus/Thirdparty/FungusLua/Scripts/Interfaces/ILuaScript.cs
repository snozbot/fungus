using UnityEngine;

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