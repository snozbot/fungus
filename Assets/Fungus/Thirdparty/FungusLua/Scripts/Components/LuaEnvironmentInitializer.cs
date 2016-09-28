// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Helper class used to extend the initialization behavior of LuaEnvironment.
    /// </summary>
    public abstract class LuaEnvironmentInitializer : MonoBehaviour
    {
        #region Public members

        /// <summary>
        /// Called when the LuaEnvironment is initializing.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Applies transformations to the input script prior to execution.
        /// </summary>
        public abstract string PreprocessScript(string input);

        #endregion
    }
}