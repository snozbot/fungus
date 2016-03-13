using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Stop Tween", 
	             "Stops an active iTween by name.")]
	[AddComponentMenu("")]
	public class StopTween : Command, ISerializationCallbackReceiver 
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("tweenName")] public string tweenNameOLD;
		#endregion

		[Tooltip("Stop and destroy any Tweens in current scene with the supplied name")]
		public StringData _tweenName;

		public override void OnEnter()
		{
			iTween.StopByName(_tweenName.Value);
			Continue();
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (tweenNameOLD != "")
			{
				_tweenName.Value = tweenNameOLD;
				tweenNameOLD = "";
			}
		}
	}

}