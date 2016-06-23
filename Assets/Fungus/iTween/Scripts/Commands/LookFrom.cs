/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Look From", 
	             "Instantly rotates a GameObject to look at the supplied Vector3 then returns it to it's starting rotation over time.")]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class LookFrom : iTweenCommand
	{
		[Tooltip("Target transform that the GameObject will look at")]
		public TransformData _fromTransform;

		[Tooltip("Target world position that the GameObject will look at, if no From Transform is set")]
		public Vector3Data _fromPosition;

		[Tooltip("Restricts rotation to the supplied axis only")]
		public iTweenAxis axis;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			if (_fromTransform.Value == null)
			{
				tweenParams.Add("looktarget", _fromPosition.Value);
			}
			else
			{
				tweenParams.Add("looktarget", _fromTransform.Value);
			}
			switch (axis)
			{
			case iTweenAxis.X:
				tweenParams.Add("axis", "x");
				break;
			case iTweenAxis.Y:
				tweenParams.Add("axis", "y");
				break;
			case iTweenAxis.Z:
				tweenParams.Add("axis", "z");
				break;
			}
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.LookFrom(_targetObject.Value, tweenParams);
		}	

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
		[HideInInspector] [FormerlySerializedAs("fromPosition")] public Vector3 fromPositionOLD;

		protected override void OnEnable()
		{
			base.OnEnable();

			if (fromTransformOLD != null)
			{
				_fromTransform.Value = fromTransformOLD;
				fromTransformOLD = null;
			}

			if (fromPositionOLD != default(Vector3))
			{
				_fromPosition.Value = fromPositionOLD;
				fromPositionOLD = default(Vector3);
			}
		}

		#endregion
	}

}