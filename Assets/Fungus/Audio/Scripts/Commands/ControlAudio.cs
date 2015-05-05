using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio", 
	             "Control Audio",
	             "Plays, loops, or stops an audiosource.")]
	public class ControlAudio : Command
	{
		public enum controlType
		{
			PlayOnce,
			PlayLoop,
			PauseLoop,
			StopLoop,
			ChangeVolume
		}

		[Tooltip("What to do to audio")]
		public controlType control;

		[Tooltip("Audio clip to play")]
		public AudioSource audioSource;

		[Range(0,1)]
		[Tooltip("Start audio at this volume")]
		public float startVolume = 1;

		[Range(0,1)]
		[Tooltip("End audio at this volume")]
		public float endVolume = 1;
		
		[Tooltip("Time to fade between current volume level and target volume level.")]
		public float fadeDuration; 

		[Tooltip("Wait until this command has finished before executing the next command.")]
		public bool waitUntilFinished = false;
		
		public override void OnEnter()
		{
			if (audioSource == null)
			{
				Continue();
				return;
			}

			audioSource.volume = endVolume;
			switch(control)
			{
				case controlType.PlayOnce:
					StopAudioWithSameTag();
					PlayOnce();
					break;
				case controlType.PlayLoop:
					StopAudioWithSameTag();
					PlayLoop();
					break;
				case controlType.PauseLoop:
					PauseLoop();
					break;
				case controlType.StopLoop:
					StopLoop(audioSource);
					break;
				case controlType.ChangeVolume:
					ChangeVolume();	
					break;
			}
			if (!waitUntilFinished)
			{
				Continue();
			}
		}

		/**
		 * If there's other music playing in the scene, assign it the same tag as the new music you want to play and
		 * the old music will be automatically stopped.
		 */
		protected void StopAudioWithSameTag()
		{
			AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>();

			foreach (AudioSource a in audioSources)
			{
				if ((a.GetComponent<AudioSource>() != audioSource) && (a.tag == audioSource.tag))
				{
					StopLoop(a.GetComponent<AudioSource>());
				}
			}
		}

		protected void PlayOnce() 
		{
			if (fadeDuration > 0)
			{
				audioSource.volume = 0;
				PlaySoundWithCallback(audioSource.GetComponent<AudioSource>().clip, endVolume, AudioFinished);
				LeanTween.value(audioSource.gameObject,0,endVolume,fadeDuration
				).setOnUpdate(
					(float updateVolume)=>{
					audioSource.volume = updateVolume;
				}
				).setOnComplete(
					()=>{
					if (waitUntilFinished)
					{
						Continue();
					}
				}
				);
			}
			else
			{
				audioSource.volume = 1;
				PlaySoundWithCallback(audioSource.GetComponent<AudioSource>().clip, endVolume, AudioFinished);
			}
		}
		
		protected void PlayLoop()
		{
			if (fadeDuration > 0)
			{
				audioSource.volume = 0;
				audioSource.loop = true;
				audioSource.GetComponent<AudioSource>().Play();
				LeanTween.value(audioSource.gameObject,0,endVolume,fadeDuration
				).setOnUpdate(
					(float updateVolume)=>{
					audioSource.volume = updateVolume;
				}
				).setOnComplete(
					()=>{
					if (waitUntilFinished)
					{
						Continue();
					}
				}
				);
			}
			else
			{
				audioSource.volume = 1;
				audioSource.loop = true;
				audioSource.GetComponent<AudioSource>().Play();
			}
		}

		protected void PauseLoop()
		{
			if (fadeDuration > 0)
			{
				LeanTween.value(audioSource.gameObject,audioSource.volume,0,fadeDuration
				).setOnUpdate(
					(float updateVolume)=>{
					audioSource.volume = updateVolume;
				}
				).setOnComplete(
					()=>{
					
					audioSource.GetComponent<AudioSource>().Pause();
					if (waitUntilFinished)
					{
						Continue();
					}
				}
				);
			}
			else
			{
				audioSource.GetComponent<AudioSource>().Pause();
			}
		}

		protected void StopLoop(AudioSource source)
		{
			if (fadeDuration > 0)
			{
				LeanTween.value(source.gameObject,audioSource.volume,0,fadeDuration
				).setOnUpdate(
					(float updateVolume)=>{
					source.volume = updateVolume;
				}
				).setOnComplete(
					()=>{
					
					source.GetComponent<AudioSource>().Stop();
					if (waitUntilFinished)
					{
						Continue();
					}
				}
				);
			}
			else
			{
				source.GetComponent<AudioSource>().Stop();
			}
		}

		protected void ChangeVolume()
		{
			LeanTween.value(audioSource.gameObject,audioSource.volume,endVolume,fadeDuration
			).setOnUpdate(
				(float updateVolume)=>{
				audioSource.volume = updateVolume;
			}
			);
		}

		// Allows ControlAudio to "Wait Until Finished" playing audio before moving onto the next command
		public delegate void AudioCallback();

		public void PlaySoundWithCallback(AudioClip clip, float endVolume, AudioCallback callback)
		{
			audioSource.GetComponent<AudioSource>().PlayOneShot(audioSource.clip, endVolume);
			StartCoroutine(DelayedCallback(audioSource.clip.length, callback));
		}

		private IEnumerator DelayedCallback(float time, AudioCallback callback)
		{
			yield return new WaitForSeconds(time);
			callback();
		}

		void AudioFinished()
		{
			Continue();
		}

		public override string GetSummary()
		{
			if (audioSource == null)
			{
				return "Error: No sound clip selected";
			}
			string fadeType = "";
			if (fadeDuration > 0)
			{
				fadeType = " Fade out";
				if (control != controlType.StopLoop)
				{
					fadeType = " Fade in volume to " + endVolume;
				}
				if (control == controlType.ChangeVolume)
				{
					fadeType = " to " + endVolume;
				}
				fadeType += " over " + fadeDuration + " seconds.";
			}
			return control.ToString() + " \"" + audioSource.name + "\"" + fadeType;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}
	
}