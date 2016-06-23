/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	
	public enum iTweenAxis
	{
		None,
		X,
		Y,
		Z
	}

	[ExecuteInEditMode]
	public abstract class iTweenCommand : Command
	{
		[Tooltip("Target game object to apply the Tween to")]
		public GameObjectData _targetObject;

		[Tooltip("An individual name useful for stopping iTweens by name")]
		public StringData _tweenName;

		[Tooltip("The time in seconds the animation will take to complete")]
		public FloatData _duration = new FloatData(1f);

		[Tooltip("The shape of the easing curve applied to the animation")]
		public iTween.EaseType easeType = iTween.EaseType.easeInOutQuad;

		[Tooltip("The type of loop to apply once the animation has completed")]
		public iTween.LoopType loopType = iTween.LoopType.none;

		[Tooltip("Stop any previously added iTweens on this object before adding this iTween")]
		public bool stopPreviousTweens = false;

		[Tooltip("Wait until the tween has finished before executing the next command")]
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			if (_targetObject.Value == null)
			{
				Continue();
				return;
			}

			if (stopPreviousTweens)
			{
				// Force any existing iTweens on this target object to complete immediately
				iTween[] tweens = _targetObject.Value.GetComponents<iTween>();
				foreach (iTween tween in tweens) {
					tween.time = 0;
					tween.SendMessage("Update");
				}
			}

			DoTween();

			if (!waitUntilFinished)
			{
				Continue();
			}
		}

		public virtual void DoTween()
		{}

		protected virtual void OniTweenComplete(object param)
		{
			Command command = param as Command;
			if (command != null && command.Equals(this))
			{
				if (waitUntilFinished)
				{
					Continue();
				}
			}
		}

		public override string GetSummary()
		{
			if (_targetObject.Value == null)
			{
				return "Error: No target object selected";
			}

			return _targetObject.Value.name + " over " + _duration.Value + " seconds";
		}

		public override Color GetButtonColor()
		{
			return new Color32(233, 163, 180, 255);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("target")] [FormerlySerializedAs("targetObject")] public GameObject targetObjectOLD;
		[HideInInspector] [FormerlySerializedAs("tweenName")] public string tweenNameOLD = "";
		[HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;

		protected virtual void OnEnable()
		{
			if (targetObjectOLD != null)
			{
				_targetObject.Value = targetObjectOLD;
				targetObjectOLD = null;
			}

            if (tweenNameOLD != "")
            {
                _tweenName.Value = tweenNameOLD;
                tweenNameOLD = "";
            }

			if (durationOLD != default(float))
			{
				_duration.Value = durationOLD;
				durationOLD = default(float);
			}		
		}

		#endregion
	}

}