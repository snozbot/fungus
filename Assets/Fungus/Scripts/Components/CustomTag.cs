// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Create custom tags for use in Say text.
    /// </summary>
    [ExecuteInEditMode]
    public class CustomTag : MonoBehaviour
    {
        [Tooltip("String that defines the start of the tag.")]
        [SerializeField] protected string tagStartSymbol;

        [Tooltip("String that defines the end of the tag.")]
        [SerializeField] protected string tagEndSymbol;

        [Tooltip("String to replace the start tag with.")]
        [SerializeField] protected string replaceTagStartWith;

        [Tooltip("String to replace the end tag with.")]
        [SerializeField] protected string replaceTagEndWith;

        protected virtual void OnEnable()
        {
            if (!activeCustomTags.Contains(this))
            {
                activeCustomTags.Add(this);
            }
        }
        
        protected virtual void OnDisable()
        {
            activeCustomTags.Remove(this);
        }

        #region Public members

        public static List<CustomTag> activeCustomTags = new List<CustomTag>();

        /// <summary>
        /// String that defines the start of the tag.
        /// </summary>
        public virtual string TagStartSymbol { get { return tagStartSymbol; } }

        /// <summary>
        /// String that defines the end of the tag.
        /// </summary>
        public virtual string TagEndSymbol { get { return tagEndSymbol; } }

        /// <summary>
        /// String to replace the start tag with.
        /// </summary>
        public virtual string ReplaceTagStartWith { get { return replaceTagStartWith; } }

        /// <summary>
        /// String to replace the end tag with.
        /// </summary>
        public virtual string ReplaceTagEndWith { get { return replaceTagEndWith; } }

        #endregion
    }
}