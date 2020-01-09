// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using UnityEngine;

namespace Fungus
{
    public class ControlWithDisplay<TDisplayEnum> : Command
    {
        [Tooltip("Display type")]
        [SerializeField] protected TDisplayEnum display;

        protected virtual bool IsDisplayNone<TEnum>(TEnum enumValue)
        {
            string displayTypeStr = Enum.GetName(typeof (TEnum), enumValue);
            return displayTypeStr == "None";
        }

        #region Public members

        public virtual TDisplayEnum Display { get { return display; } }

        #endregion
    }
}