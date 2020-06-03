// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls Pause on given source.
    /// </summary>
    [CommandInfo("Audio",
                 "Play Source Pause",
                     "Calls Pause on given source")]
    [AddComponentMenu("")]
    public class AudioSourcePause : AudioSourceBase
    {
        public override void OnEnter()
        {
            audioSource.Value.Pause();

            Continue();
        }
    }
}