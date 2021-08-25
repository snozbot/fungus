using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This handles encoding numeric variables into Save Data.
    /// </summary>
    public class NumericVarSaveEncoder: MonoBehaviour
    {
        protected static System.Type[] numericTypes = new System.Type[]
        {
            typeof(IntegerVariable),
            typeof(FloatVariable),
            typeof(BooleanVariable) // Yes, we count bools as numerics
        };

        public virtual StringPair Encode(Variable varToEncode)
        {
            // I know you may be looking at this implementation and thinking
            // "why not just have this class encode vars in general?"
            // The reason is, I believe it's best to have encoders and decoders
            // hot-swappable so that users can customize the encoding and decoding
            // processes. After all, what if they decide that they want certain variable
            // types to be encoded in ways different than the default for the sake of
            // security or something?
            StringPair encodedVar = new StringPair();
            encodedVar.key = varToEncode.name;
            encodedVar.val = varToEncode.GetValue().ToString();

            return encodedVar;
        }

        
    }
}