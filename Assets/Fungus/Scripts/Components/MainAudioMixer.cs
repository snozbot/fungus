// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    /// <summary>
    /// Central access point for the expected default, main mixer within the project. Primarily expected to be used by 
    /// classes that create AudioSources at runtime and cannot have groups manually assigned via the inspector.
    /// </summary>
    public class MainAudioMixer : MonoBehaviour
    {
        public AudioMixer Mixer { get; protected set; }
        public AudioMixerGroup MusicGroup {get; protected set; }
        public AudioMixerGroup SFXGroup { get; protected set; }
        public AudioMixerGroup VoiceGroup { get; protected set; }

        public virtual void Init()
        {
            Mixer = Resources.Load(FungusConstants.FungusAudioMixer) as AudioMixer;
            MusicGroup = Mixer.FindMatchingGroups("Music")[0];
            SFXGroup = Mixer.FindMatchingGroups("SFX")[0];
            VoiceGroup = Mixer.FindMatchingGroups("Voice")[0];
        }
    }
}