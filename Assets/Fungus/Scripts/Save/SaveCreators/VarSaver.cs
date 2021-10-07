using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

namespace Fungus
{
    /// <summary>
    /// Base class for encoders that handle Fungus Flowchart Variables.
    /// </summary>
    public abstract class VarSaver<TSaveUnit> : MonoBehaviour where TSaveUnit: SaveUnit
    {
        protected abstract IList<Type> SupportedTypes { get; }

        protected enum EncodingValueType
        {
            jsonString,
            normalString,
        }

        protected virtual EncodingValueType EncodeValueAs { get; } = EncodingValueType.jsonString;

        public virtual ISaveUnit Encode(Variable varToEncode)
        {
            EnsureValidVarType(varToEncode);

            // I know you may be looking at this implementation and thinking
            // "why not just have this class encode vars in general?"
            // The reason is, I believe it's best to have encoders and decoders
            // hot-swappable so that users can customize the encoding and decoding
            // processes. After all, what if they decide that they want certain variable
            // types to be encoded in ways different than the default for the sake of
            // security or something?
            StringPair encodedVar = new StringPair();
            encodedVar.key = varToEncode.name;
            encodedVar = EncodeValueAsAppropriate(varToEncode, encodedVar);

            return encodedVar;
        }

        protected virtual StringPair EncodeValueAsAppropriate(Variable varToEncode, StringPair encodingResult)
        {
            if (EncodeValueAs == EncodingValueType.jsonString)
                encodingResult.val = JsonUtility.ToJson(varToEncode.GetValue());
            else if (EncodeValueAs == EncodingValueType.normalString)
                encodingResult.val = varToEncode.GetValue().ToString();

            return encodingResult;
        }

        protected virtual void EnsureValidVarType(Variable toCheck)
        {
            bool varTypeIsValid = SupportedTypes.Contains(toCheck.GetType());

            if (!varTypeIsValid)
            {
                AlertInvalidVarType(toCheck);
            }
        }

        protected virtual void AlertInvalidVarType(Variable invalid)
        {
            Type typeOfThisEncoder = this.GetType();
            string message = string.Format(invalidVarTypeMessageFormat, invalid.name, typeOfThisEncoder.Name);
            throw new System.InvalidOperationException(message);
        }

        protected static string invalidVarTypeMessageFormat = "Variable {0} cannot be encoded by {1}, due to its type not being supported by {1}.";

        public virtual IList<StringPair> Encode(IList<Variable> varsToEncode)
        {
            IList<StringPair> results = new StringPair[varsToEncode.Count];

            for (int i = 0; i < varsToEncode.Count; i++)
            {
                var currentVar = varsToEncode[i];
                EnsureValidVarType(currentVar);
                var encodedVar = Encode(currentVar);
                results[i] = encodedVar;
            }

            return results;
        }
    }
}