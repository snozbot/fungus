using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Base class for all of our physics event handlers
    /// </summary>
    [AddComponentMenu("")]
    public abstract class TagFilteredEventHandler : EventHandler
    {
        [Tooltip("Only fire the event if one of the tags match. Empty means any will fire.")]
        [SerializeField]
        protected string[] tagFilter;

        protected void ProcessTagFilter(string tagOnOther)
        {
            if (tagFilter.Length == 0)
            {
                ExecuteBlock();
            }
            else
            {
                if (System.Array.IndexOf(tagFilter, tagOnOther) != -1)
                {
                    ExecuteBlock();
                }
            }
        }
    }
}
