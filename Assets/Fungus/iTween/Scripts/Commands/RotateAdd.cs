using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate Add", 
	             "Rotates a game object by the specified angles over time.")]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class RotateAdd : iTweenCommand
	{
		[Tooltip("A rotation offset in space the GameObject will animate to")]
		public Vector3Data _offset;

		[Tooltip("Apply the transformation in either the world coordinate or local cordinate system")]
		public Space space = Space.Self;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			tweenParams.Add("amount", _offset.Value);
			tweenParams.Add("space", space);
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateAdd(_targetObject.Value, tweenParams);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("offset")] public Vector3 offsetOLD;

		protected override void OnEnable()
		{
			base.OnEnable();

			if (offsetOLD != default(Vector3))
			{
				_offset.Value = offsetOLD;
				offsetOLD = default(Vector3);
			}
		}

		#endregion
	}

}