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
    public class CustomTag : MonoBehaviour 
    {
        [SerializeField] protected string tagStartSymbol;
        public virtual string TagStartSymbol { get { return tagStartSymbol; } }

        [SerializeField] protected string tagEndSymbol;
        public virtual string TagEndSymbol { get { return tagEndSymbol; } }

        [SerializeField] protected string replaceTagStartWith;
        public virtual string ReplaceTagStartWith { get { return replaceTagStartWith; } }

        [SerializeField] protected string replaceTagEndWith;
        public virtual string ReplaceTagEndWith { get { return replaceTagEndWith; } }

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
    }
}