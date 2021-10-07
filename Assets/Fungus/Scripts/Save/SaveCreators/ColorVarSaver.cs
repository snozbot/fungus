using System.Collections.Generic;
using Type = System.Type;

namespace Fungus
{
    public class ColorVarSaver : VarSaver
    {
        protected override IList<Type> SupportedTypes { get; } = new Type[]
        {
            typeof(ColorVariable),
        };

    }
}