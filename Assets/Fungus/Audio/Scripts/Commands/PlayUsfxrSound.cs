namespace Fungus {
    using System;
    using UnityEngine;
	using UnityEngine.Serialization;

    [CommandInfo("Audio", 
	             "Play Usfxr Sound", 
	             "Plays a usfxr synth sound. Use the usfxr editor [Tools > Fungus > Utilities > Generate usfxr Sound Effects] to create the SettingsString. Set a ParentTransform if using positional sound. See https://github.com/zeh/usfxr for more information about usfxr.")]
	[AddComponentMenu("")]
	public class PlayUsfxrSound : Command, ISerializationCallbackReceiver 
	{
        protected SfxrSynth _synth = new SfxrSynth();

		[Tooltip("Transform to use for positional audio")]
        public Transform ParentTransform = null;

		[Tooltip("Settings string which describes the audio")]
		public StringData _SettingsString = new StringData("");

		[Tooltip("Time to wait before executing the next command")]
		public float waitDuration = 0;

        //Call this if the settings have changed
        protected void UpdateCache() {
            if (_SettingsString.Value != null) {
                _synth.parameters.SetSettingsString(_SettingsString.Value);
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
            if (String.IsNullOrEmpty(_SettingsString.Value)) {
                return "Settings String hasn't been set!";
            }
            if (ParentTransform != null) {
                return "" + ParentTransform.name + ": " + _SettingsString.Value;
            }
            return "Camera.main: " + _SettingsString.Value;
        }

        public override Color GetButtonColor() {
            return new Color32(128, 200, 200, 255);
        }

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("SettingsString")] public String SettingsStringOLD;

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
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
