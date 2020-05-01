// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Transform variable type.
    /// </summary>
    [VariableInfo("Other", "Transform")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class TransformVariable : VariableBase<Transform>
    {
    }

    /// <summary>
    /// Container for a Transform variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct TransformData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(TransformVariable))]
        public TransformVariable transformRef;
        
        [SerializeField]
        public Transform transformVal;

        public TransformData(Transform v)
        {
            transformVal = v;
            transformRef = null;
        }
        
        public static implicit operator Transform(TransformData vector3Data)
        {
            return vector3Data.Value;
        }

        public Transform Value
        {
            get { return (transformRef == null) ? transformVal : transformRef.Value; }
            set { if (transformRef == null) { transformVal = value; } else { transformRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (transformRef == null)
            {
                return transformVal != null ? transformVal.ToString() : "Null";
            }
            else
            {
                return transformRef.Key;
            }
        }
    }
}