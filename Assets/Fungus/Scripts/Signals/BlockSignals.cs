// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Block event signalling system.
    /// You can use this to be notified about various events in the Block execution process.
    /// </summary>
    public static class BlockSignals
    {
        #region Public members

        /// <summary>
        /// BlockStart signal. Sent when the Block starts execution.
        /// </summary>
        public static event BlockStartHandler OnBlockStart;
        public delegate void BlockStartHandler(Block block);
        public static void DoBlockStart(Block block) { if (OnBlockStart != null) OnBlockStart(block); }

        /// <summary>
        /// BlockEnd signal. Sent when the Block ends execution.
        /// </summary>
        public static event BlockEndHandler OnBlockEnd;
        public delegate void BlockEndHandler(Block block);
        public static void DoBlockEnd(Block block) { if (OnBlockEnd != null) OnBlockEnd(block); }

        /// <summary>
        /// CommandExecute signal. Sent just before a Command in a Block executes.
        /// </summary>
        public static event CommandExecuteHandler OnCommandExecute;
        public delegate void CommandExecuteHandler(Block block, Command command, int commandIndex, int maxCommandIndex);
        public static void DoCommandExecute(Block block, Command command, int commandIndex, int maxCommandIndex) { if (OnCommandExecute != null) OnCommandExecute(block, command, commandIndex, maxCommandIndex); }

        #endregion
    }
}
