/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

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