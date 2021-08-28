using System.Collections.Generic;
using Type = System.Type;

namespace Fungus
{
    public class ColorVarSaveEncoder : VarSaveEncoder
    {
        protected override IList<Type> SupportedTypes { get; } = new List<Type>
        {
            typeof(ColorVariable),
        };
    }
}