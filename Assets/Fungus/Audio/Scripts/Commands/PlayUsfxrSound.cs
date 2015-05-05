namespace Fungus {
    using System;
    using UnityEngine;

    [CommandInfo("Audio", 
	             "Play Usfxr Sound", 
	             "Plays a usfxr synth sound. Use the usfxr editor [Tools > Fungus > Utilities > Generate usfxr Sound Effects] to create the SettingsString. Set a ParentTransform if using positional sound. See https://github.com/zeh/usfxr for more information about usfxr.")]
	[AddComponentMenu("")]
	public class PlayUsfxrSound : Command 
	{
        protected SfxrSynth _synth = new SfxrSynth();

		[Tooltip("Transform to use for positional audio")]
        public Transform ParentTransform = null;

		[Tooltip("Settings string which describes the audio")]
		public String SettingsString = "";

		[Tooltip("Time to wait before executing the next command")]
		public float waitDuration = 0;

        //Call this if the settings have changed
        protected void UpdateCache() {
            if (SettingsString != null) {
                _synth.parameters.SetSettingsString(SettingsString);
                _synth.CacheSound();
            }
        }

        public void Awake() {
            //Always build the cache on awake
            UpdateCache();
        }

        public override void OnEnter() {
            _synth.SetParentTransform(ParentTransform);
            _synth.Play();
			if (waitDuration == 0f)
			{
            	Continue();
			}
			else
			{
				Invoke ("DoWait", waitDuration);
			}
        }

		protected void DoWait()
		{
			Continue();
		}

        public override string GetSummary() {
            if (String.IsNullOrEmpty(SettingsString)) {
                return "Settings String hasn't been set!";
            }
            if (ParentTransform != null) {
                return "" + ParentTransform.name + ": " + SettingsString;
            }
            return "Camera.main: " + SettingsString;
        }

        public override Color GetButtonColor() {
            return new Color32(128, 200, 200, 255);
        }
    }
}
