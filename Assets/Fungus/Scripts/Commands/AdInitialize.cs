// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-05-29 - www.ducks-on-the-water.com

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine.Advertisements;

namespace Fungus
{
    /// <summary>
    /// Shows an advertisment and calls Break or Success.
    /// </summary>
    [CommandInfo("Ads",
                 "Intialize Ad",
                 "Prepares the advertising system of Unity. Call this as early as possible.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class AdInitialize : Command
    {
        [Tooltip("Game ID: iOS")]
        [SerializeField] protected StringData gameIdiOS = new StringData();

        [Tooltip("Game ID: Android")]
        [SerializeField] protected StringData gameIdAndroid = new StringData();

        [Tooltip("Run in Test Mode")]
        [SerializeField] protected BooleanData testMode = new BooleanData();


        #region Public members

        public override void OnEnter()
        {
#if UNITY_ADS
#if UNITY_IOS
        Advertisement.Initialize(gameIdiOS.Value, testMode.Value);
#else
#if !UNITY_WEBGL
        Advertisement.Initialize(gameIdAndroid.Value, testMode.Value);
#endif
#endif
#endif
            Continue();
        }

        public override string GetSummary()
        {
            string myOSId;
            string myOS;

#if !UNITY_ADS
            return "Error: Activate Advertising in Unity Services";
#else
#if UNITY_IOS
            myOSId = gameIdiOS.Value;
            myOS = "iOS";
#else
            myOSId = gameIdAndroid.Value;
            myOS = "Android";
#endif
            return "with Id '" + myOSId + "' on " + myOS;
#endif

        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        #endregion

    }
}
