using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Creates Save Units for Fungus Color variables
    /// </summary>
    public class ColorVarSaver : VarSaver
    {
        protected override ContentType SetContentAs => ContentType.jsonString;
    }
}