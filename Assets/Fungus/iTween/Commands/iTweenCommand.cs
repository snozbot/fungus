using UnityEngine;
using System.Collections;

namespace Fungus
{

	public abstract class iTweenCommand : Command 
	{
		public GameObject target;
		public string tweenName;
		public float duration = 1f;
		public iTween.EaseType easeType = iTween.EaseType.easeInOutQuad;
		public iTween.LoopType loopType = iTween.LoopType.none;
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			if (target == null)
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

		protected virtual void OnComplete(object param)
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
			if (target == null)
			{
				return "Error: No target object selected";
			}

			return target.gameObject.name + " over " + duration + " seconds";
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(233, 163, 180, 255);
		}
	}

}