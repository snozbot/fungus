/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "AudioSource")]
	[AddComponentMenu("")]
    [System.Serializable]
	public class AudioSourceVariable : VariableBase<AudioSource>
	{}

	[System.Serializable]
	public struct AudioSourceData
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(AudioSourceVariable))]
		public AudioSourceVariable audioSourceRef;
		
		[SerializeField]
		public AudioSource audioSourceVal;

		public AudioSourceData(AudioSource v)
		{
			audioSourceVal = v;
			audioSourceRef = null;
		}
		
		public static implicit operator AudioSource(AudioSourceData audioSourceData)
		{
			return audioSourceData.Value;
		}

		public AudioSource Value
		{
			get { return (audioSourceRef == null) ? audioSourceVal : audioSourceRef.value; }
			set { if (audioSourceRef == null) { audioSourceVal = value; } else { audioSourceRef.value = value; } }
		}

		public string GetDescription()
		{
			if (audioSourceRef == null)
			{
				return audioSourceVal.ToString();
			}
			else
			{
				return audioSourceRef.key;
			}
		}
	}

}