using System;
using UnityEngine;

namespace Fungus
{
    public class ControlWithDisplay<TDisplayEnum> : Command
    {
        [Tooltip("Display type")]
        public TDisplayEnum display;

        protected bool IsDisplayNone<TEnum>(TEnum enumValue)
        {
            string displayTypeStr = Enum.GetName(typeof (TEnum), enumValue);
            return displayTypeStr == "None";
        }
    }
}