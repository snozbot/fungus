// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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
            if (DoesPassFilter(tagOnOther))
            {
                ExecuteBlock();
            }
        }

        protected bool DoesPassFilter(string tagOnOther)
        {
            return tagFilter.Length == 0 || System.Array.IndexOf(tagFilter, tagOnOther) != -1;
        }
    }
}