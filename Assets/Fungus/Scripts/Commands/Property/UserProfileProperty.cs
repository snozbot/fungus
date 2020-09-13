// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Get or Set the current user profile
    /// </summary>
    [CommandInfo("Property",
                 "User Profile",
                 "Get or Set the current UserProfile")]
    [AddComponentMenu("")]
    public class UserProfileProperty : BaseVariableProperty
    {
        [SerializeField] protected StringData profileName;


        public override void OnEnter()
        {
            switch (getOrSet)
            {
            case GetSet.Get:
                profileName.Value = FungusManager.Instance.UserProfileManager.CurrentUserProfileName;
                break;
            case GetSet.Set:
                FungusManager.Instance.UserProfileManager.ChangeProfile(profileName.Value);
                break;
            default:
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return getOrSet.ToString() + " " + profileName.stringRef?.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (profileName.stringRef == variable)
                return true;

            return base.HasReference(variable);
        }
    }
}