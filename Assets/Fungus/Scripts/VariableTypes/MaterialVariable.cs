// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Material variable type.
    /// </summary>
    [VariableInfo("Other", "Material")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class MaterialVariable : VariableBase<Material>
    {
    }

    /// <summary>
    /// Container for a Material variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct MaterialData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(MaterialVariable))]
        public MaterialVariable materialRef;
        
        [SerializeField]
        public Material materialVal;

        public MaterialData(Material v)
        {
            materialVal = v;
            materialRef = null;
        }
        
        public static implicit operator Material(MaterialData materialData)
        {
            return materialData.Value;
        }

        public Material Value
        {
            get { return (materialRef == null) ? materialVal : materialRef.Value; }
            set { if (materialRef == null) { materialVal = value; } else { materialRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (materialRef == null)
            {
                return materialVal != null ? materialVal.ToString() : "Null";
            }
            else
            {
                return materialRef.Key;
            }
        }
    }
}