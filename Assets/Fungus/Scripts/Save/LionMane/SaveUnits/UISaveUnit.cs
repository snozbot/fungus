using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus.LionManeSaveSys;

namespace Fungus
{
    public class UISaveUnit : ISaveUnit<UISaveUnit>
    {
        public object Contents => this;

        public string TypeName => "UISaveUnit";

        UISaveUnit ISaveUnit<UISaveUnit>.Contents => this;
    }
}