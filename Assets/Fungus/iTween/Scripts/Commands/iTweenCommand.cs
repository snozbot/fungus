using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace Fungus
{
	public enum iTweenAxis
	{
		None,
		X,
		Y,
		Z
	}
	
	public abstract class iTweenCommand : Command 
	{
		[Tooltip("Target game object to apply the Tween to")]
		[FormerlySerializedAs("target")]
		public GameObject targetObject;

		[Tooltip("An individual name useful for stopping iTweens by name")]
		public string tweenName;

		[Tooltip("The time in seconds the animation will take to complete")]
		public float duration = 1f;

		[Tooltip("The shape of the easing curve applied to the animation")]
		public iTween.EaseType easeType = iTween.EaseType.easeInOutQuad;

		[Tooltip("The type of loop to apply once the animation has completed")]
		public iTween.LoopType loopType = iTween.LoopType.none;

		[Tooltip("Wait until the tween has finished before executing the next command")]
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			if (targetObject == null)
			{
				Continue();
				return;
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
			if (targetObject == null)
			{
				return "Error: No target object selected";
			}

			return targetObject.name + " over " + duration + " seconds";
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(233, 163, 180, 255);
		}
	}

}