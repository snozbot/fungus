// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Global constants used in various parts of Fungus.
    /// </summary>
    public static class FungusConstants
    {
        #region Public members

        /// <summary>
        /// Duration of fade for executing icon displayed beside blocks & commands.
        /// </summary>
        public const float ExecutingIconFadeTime = 0.5f;

        /// <summary>
        /// The current version of the Flowchart. Used for updating components.
        /// </summary>
        public const int CurrentVersion = 1;

        /// <summary>
        /// The name of the initial block in a new flowchart.
        /// </summary>
        public const string DefaultBlockName = "New Block";

        #endregion
    }
}