using UnityEngine;
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