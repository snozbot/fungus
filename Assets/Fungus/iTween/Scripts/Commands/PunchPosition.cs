using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Punch Position", 
	             "Applies a jolt of force to a GameObject's position and wobbles it back to its initial position.")]
	[AddComponentMenu("")]
	public class PunchPosition : iTweenCommand, ISerializationCallbackReceiver 
	{
		[Tooltip("A translation offset in space the GameObject will animate to")]
		public Vector3Data _amount;

		[Tooltip("Apply the transformation in either the world coordinate or local cordinate system")]
		public Space space = Space.Self;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			tweenParams.Add("amount", _amount.Value);
			tweenParams.Add("space", space);
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.PunchPosition(_targetObject.Value, tweenParams);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("amount")] public Vector3 amountOLD;

		public override void OnBeforeSerialize()
		{}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			if (amountOLD != default(Vector3))
			{
				_amount.Value = amountOLD;
				amountOLD = default(Vector3);
			}
		}

		#endregion
	}

}