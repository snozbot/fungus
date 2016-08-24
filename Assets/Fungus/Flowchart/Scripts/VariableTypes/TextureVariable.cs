// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Texture variable type.
    /// </summary>
    [VariableInfo("Other", "Texture")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class TextureVariable : VariableBase<Texture>
    {}

    /// <summary>
    /// Container for a Texture variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct TextureData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(TextureVariable))]
        public TextureVariable textureRef;
        
        [SerializeField]
        public Texture textureVal;

        public TextureData(Texture v)
        {
            textureVal = v;
            textureRef = null;
        }
        
        public static implicit operator Texture(TextureData textureData)
        {
            return textureData.Value;
        }

        public Texture Value
        {
            get { return (textureRef == null) ? textureVal : textureRef.Value; }
            set { if (textureRef == null) { textureVal = value; } else { textureRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (textureRef == null)
            {
                return textureVal.ToString();
            }
            else
            {
                return textureRef.Key;
            }
        }
    }
}