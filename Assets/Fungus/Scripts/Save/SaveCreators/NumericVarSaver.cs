using System.Collections.Generic;
using Type = System.Type;

namespace Fungus
{
    /// <summary>
    /// This handles encoding numeric variables into Save Data.
    /// </summary>
    public class NumericVarSaver: VarSaver
    {
        protected override IList<Type> SupportedTypes { get; } = new Type[]
        {
            typeof(IntegerVariable),
            typeof(FloatVariable),
            typeof(BooleanVariable) // Yes, we count bools as numerics
        };

        protected override EncodingValueType EncodeValueAs { get; } = EncodingValueType.normalString;

    }
}