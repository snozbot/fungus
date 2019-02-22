// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using UnityEngine;
using UnityEngine.Serialization;

﻿namespace Fungus 
{
    /// <summary>
    /// Plays a usfxr synth sound. Use the usfxr editor [Tools > Fungus > Utilities > Generate usfxr Sound Effects] to create the SettingsString. Set a ParentTransform if using positional sound. See https://github.com/zeh/usfxr for more information about usfxr.
    /// </summary>
    [CommandInfo("Audio", 
                 "Play Usfxr Sound", 
                 "Plays a usfxr synth sound. Use the usfxr editor [Tools > Fungus > Utilities > Generate usfxr Sound Effects] to create the SettingsString. Set a ParentTransform if using positional sound. See https://github.com/zeh/usfxr for more information about usfxr.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class PlayUsfxrSound : Command
    {
        [Tooltip("Transform to use for positional audio")]
        [SerializeField] protected Transform ParentTransform = null;

        [Tooltip("Settings string which describes the audio")]
        [SerializeField] protected StringDataMulti _SettingsString = new StringDataMulti("");

        [Tooltip("Time to wait before executing the next command")]
        [SerializeField] protected float waitDuration = 0;

        protected SfxrSynth _synth = new SfxrSynth();

        //Call this if the settings have changed
        protected virtual void UpdateCache() 
        {
            if (_SettingsString.Value != null) 
            {
                _synth.parameters.SetSettingsString(_SettingsString.Value);
                _synth.CacheSound();
            }
        }

        protected virtual void Awake() 
        {
            //Always build the cache on awake
            UpdateCache();
        }

        protected void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter() 
        {
            _synth.SetParentTransform(ParentTransform);
            _synth.Play();
            if (Mathf.Approximately(waitDuration, 0f))
            {
                Continue();
            }
            else
            {
                Invoke ("DoWait", waitDuration);
            }
        }

        public override string GetSummary() 
        {
            if (String.IsNullOrEmpty(_SettingsString.Value)) 
            {
                return "Settings String hasn't been set!";
            }
            if (ParentTransform != null) 
            {
                return "" + ParentTransform.name + ": " + _SettingsString.Value;
            }
            return "Camera.main: " + _SettingsString.Value;
        }

        public override Color GetButtonColor() 
        {
            return new Color32(128, 200, 200, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return variable == _SettingsString.stringRef;
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("SettingsString")] public String SettingsStringOLD = "";

        protected virtual void OnEnable()
        {
            if (SettingsStringOLD != "")
            {
                _SettingsString.Value = SettingsStringOLD;
                SettingsStringOLD = "";
            }
        }

        #endregion
    }
}
