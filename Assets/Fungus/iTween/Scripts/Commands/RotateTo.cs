using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate To", 
	             "Rotates a game object to the specified angles over time.")]
	[AddComponentMenu("")]
	public class RotateTo : iTweenCommand, ISerializationCallbackReceiver 
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
		[HideInInspector] [FormerlySerializedAs("toRotation")] public Vector3 toRotationOLD;
		#endregion

		[Tooltip("Target transform that the GameObject will rotate to")]
		public TransformData _toTransform;

		[Tooltip("Target rotation that the GameObject will rotate to, if no To Transform is set")]
		public Vector3Data _toRotation;

		[Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			if (_toTransform.Value == null)
			{
				tweenParams.Add("rotation", _toRotation.Value);
			}
			else
			{
				tweenParams.Add("rotation", _toTransform.Value);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateTo(_targetObject.Value, tweenParams);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public void OnBeforeSerialize()
		{}

		public void OnAfterDeserialize()
		{
			if (toTransformOLD != null)
			{
				_toTransform.Value = toTransformOLD;
				toTransformOLD = null;
			}

			if (toRotationOLD != default(Vector3))
			{
				_toRotation.Value = toRotationOLD;
				toRotationOLD = default(Vector3);
			}
		}
	}

}