// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Helper class to manage parsing and executing the conversation format.
    /// </summary>
    public interface IConversationManager
    {
        /// <summary>
        /// Caches the character objects in the scene for fast lookup during conversations.
        /// </summary>
        void PopulateCharacterCache();

        /// <summary>
        /// Parse and execute a conversation string.
        /// </summary>
        IEnumerator DoConversation(string conv);
    }
}