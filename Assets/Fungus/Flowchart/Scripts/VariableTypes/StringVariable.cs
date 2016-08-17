/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{

    [VariableInfo("", "String")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class StringVariable : VariableBase<string>
    {
        public virtual bool Evaluate(CompareOperator compareOperator, string stringValue)
        {
            string lhs = value;
            string rhs = stringValue;

            bool condition = false;

            switch (compareOperator)
            {
            case CompareOperator.Equals:
                condition = lhs == rhs;
                break;
            case CompareOperator.NotEquals:
            default:
                condition = lhs != rhs;
                break;
            }

            return condition;
        }
    }

    /// <summary>
    /// Can contain a reference to a StringVariable or a string constant.
    /// Appears as a single line property in the inspector.
    /// For a multi-line property, use StringDataMulti.
    /// </summary>
    [System.Serializable]
    public struct StringData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(StringVariable))]
        public StringVariable stringRef;

        [SerializeField]
        public string stringVal;

        public StringData(string v)
        {
            stringVal = v;
            stringRef = null;
        }
        
        public static implicit operator string(StringData spriteData)
        {
            return spriteData.Value;
        }

        public string Value
        {
            get 
            { 
                if (stringVal == null) stringVal = "";
                return (stringRef == null) ? stringVal : stringRef.value; 
            }
            set { if (stringRef == null) { stringVal = value; } else { stringRef.value = value; } }
        }

        public string GetDescription()
        {
            if (stringRef == null)
            {
                return stringVal;
            }
            else
            {
                return stringRef.key;
            }
        }
    }

    /// <summary>
    /// Can contain a reference to a StringVariable or a string constant.
    /// Appears as a multi-line property in the inspector.
    /// For a single-line property, use StringData.
    /// </summary>
    [System.Serializable]
    public struct StringDataMulti
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(StringVariable))]
        public StringVariable stringRef;

        [TextArea(1,10)]
        [SerializeField]
        public string stringVal;

        public StringDataMulti(string v)
        {
            stringVal = v;
            stringRef = null;
        }

        public static implicit operator string(StringDataMulti spriteData)
        {
            return spriteData.Value;
        }

        public string Value
        {
            get 
            {
                if (stringVal == null) stringVal = "";
                return (stringRef == null) ? stringVal : stringRef.value; 
            }
            set { if (stringRef == null) { stringVal = value; } else { stringRef.value = value; } }
        }

        public string GetDescription()
        {
            if (stringRef == null)
            {
                return stringVal;
            }
            else
            {
                return stringRef.key;
            }
        }
    }
        
}