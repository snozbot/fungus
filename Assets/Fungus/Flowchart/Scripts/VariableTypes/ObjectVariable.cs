// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Object variable type.
    /// </summary>
    [VariableInfo("Other", "Object")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class ObjectVariable : VariableBase<Object>
    {}

    /// <summary>
    /// Container for an Object variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct ObjectData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(ObjectVariable))]
        public ObjectVariable objectRef;
        
        [SerializeField]
        public Object objectVal;

        public ObjectData(Object v)
        {
            objectVal = v;
            objectRef = null;
        }
        
        public static implicit operator Object(ObjectData objectData)
        {
            return objectData.Value;
        }

        public Object Value
        {
            get { return (objectRef == null) ? objectVal : objectRef.Value; }
            set { if (objectRef == null) { objectVal = value; } else { objectRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (objectRef == null)
            {
                return objectVal.ToString();
            }
            else
            {
                return objectRef.Key;
            }
        }
    }
}