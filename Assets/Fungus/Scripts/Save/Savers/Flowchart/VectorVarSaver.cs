using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public class VectorVarSaver : VarSaver
    {
        protected override ContentType SetContentAs
        {
            get { return ContentType.jsonString; }
        }
    }
}