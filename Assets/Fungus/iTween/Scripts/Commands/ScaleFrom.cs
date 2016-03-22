using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Scale From", 
	             "Changes a game object's scale to the specified value and back to its original scale over time.")]
	[AddComponentMenu("")]
	public class ScaleFrom : iTweenCommand, ISerializationCallbackReceiver 
	{
		[Tooltip("Target transform that the GameObject will scale from")]
		public TransformData _fromTransform;

		[Tooltip("Target scale that the GameObject will scale from, if no From Transform is set")]
		public Vector3Data _fromScale;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			if (_fromTransform.Value == null)
			{
				tweenParams.Add("scale", _fromScale.Value);
			}
			else
			{
				tweenParams.Add("scale", _fromTransform.Value);
			}
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ScaleFrom(_targetObject.Value, tweenParams);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
		[HideInInspector] [FormerlySerializedAs("fromScale")] public Vector3 fromScaleOLD;

		public override void OnBeforeSerialize()
		{}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			if (fromTransformOLD != null)
			{
				_fromTransform.Value = fromTransformOLD;
				fromTransformOLD = null;
			}

			if (fromScaleOLD != default(Vector3))
			{
				_fromScale.Value = fromScaleOLD;
				fromScaleOLD = default(Vector3);
			}
		}

		#endregion
	}

}