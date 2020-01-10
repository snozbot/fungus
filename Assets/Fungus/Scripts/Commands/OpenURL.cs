// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using Fungus;

namespace Fungus
{
    /// <summary>
    /// Opens the specified URL in the browser.
    /// </summary>
    [CommandInfo("Scripting",
                 "Open URL",
                 "Opens the specified URL in the browser.")]
    public class OpenURL : Command
    {
        [Tooltip("URL to open in the browser")]
        [SerializeField] protected StringData url = new StringData();

        #region Public members

        public override void OnEnter()
        {
            Application.OpenURL(url.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return url.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return url.stringRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}