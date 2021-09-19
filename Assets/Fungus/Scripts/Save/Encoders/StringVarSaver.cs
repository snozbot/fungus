using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

namespace Fungus
{
    public class StringVarSaver : VarSaver
    {
        protected override IList<Type> SupportedTypes { get; } = new Type[]
        {
            typeof(StringVariable),
        };

        protected override EncodingValueType EncodeValueAs { get; } = EncodingValueType.normalString;
    }
}