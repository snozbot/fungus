using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Move To", 
	             "Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).")]
	[AddComponentMenu("")]
	public class MoveTo : iTweenCommand, ISerializationCallbackReceiver 
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
		[HideInInspector] [FormerlySerializedAs("toPosition")] public Vector3 toPositionOLD;
		#endregion

		[Tooltip("Target transform that the GameObject will move to")]
		public TransformData _toTransform;

		[Tooltip("Target world position that the GameObject will move to, if no From Transform is set")]
		public Vector3Data _toPosition;

		[Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			if (_toTransform.Value == null)
			{
				tweenParams.Add("position", _toPosition.Value);
			}
			else
			{
				tweenParams.Add("position", _toTransform.Value);
			}
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.MoveTo(_targetObject.Value, tweenParams);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public override void OnBeforeSerialize()
		{}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			if (toTransformOLD != null)
			{
				_toTransform.Value = toTransformOLD;
				toTransformOLD = null;
			}

			if (toPositionOLD != default(Vector3))
			{
				_toPosition.Value = toPositionOLD;
				toPositionOLD = default(Vector3);
			}
		}
	}

}