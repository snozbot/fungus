// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Create custom tags for use in Say text.
    /// </summary>
    [ExecuteInEditMode]
    public class CustomTag : MonoBehaviour, ICustomTag 
    {
        [Tooltip("String that defines the start of the tag.")]
        [SerializeField] protected string tagStartSymbol;

        [Tooltip("String that defines the end of the tag.")]
        [SerializeField] protected string tagEndSymbol;

        [Tooltip("String to replace the start tag with.")]
        [SerializeField] protected string replaceTagStartWith;

        [Tooltip("String to replace the end tag with.")]
        [SerializeField] protected string replaceTagEndWith;

        static public List<CustomTag> activeCustomTags = new List<CustomTag>();
        
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

        #region ICustomTag implementation

        public virtual string TagStartSymbol { get { return tagStartSymbol; } }

        public virtual string TagEndSymbol { get { return tagEndSymbol; } }

        public virtual string ReplaceTagStartWith { get { return replaceTagStartWith; } }

        public virtual string ReplaceTagEndWith { get { return replaceTagEndWith; } }

        #endregion
    }
}