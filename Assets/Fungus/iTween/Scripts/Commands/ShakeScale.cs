using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Shake Scale", 
	             "Randomly shakes a GameObject's rotation by a diminishing amount over time.")]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class ShakeScale : iTweenCommand
	{
		[Tooltip("A scale offset in space the GameObject will animate to")]
		public Vector3Data _amount;
		
		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			tweenParams.Add("amount", _amount.Value);
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
            tweenParams.Add("ignoretimescale", ignoreTimeScale);
            tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ShakeScale(_targetObject.Value, tweenParams);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("amount")] public Vector3 amountOLD;

		protected override void OnEnable()
		{
			base.OnEnable();

			if (amountOLD != default(Vector3))
			{
				_amount.Value = amountOLD;
				amountOLD = default(Vector3);
			}
		}

		#endregion
	}
	
}