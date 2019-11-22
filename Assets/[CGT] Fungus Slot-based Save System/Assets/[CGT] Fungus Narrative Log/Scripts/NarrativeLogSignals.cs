using UnityEngine.Events;

namespace CGTUnity.Fungus.NarrativeLogSystem
{
    /// <summary>
    /// Signals related to the new NarrativeLog's actions.
    /// </summary>
    public static class Signals
    {
        /// <summary>
        /// Invoked when a new entry is registered into the Narrative Log.
        /// </summary>
        public static UnityAction<Entry> NarrativeAdded =   delegate {};

        /// <summary>
        /// Invoked when a NarrativeLog's contents are cleared.
        /// </summary>
        public static UnityAction LogCleared =              delegate {};
    }
}