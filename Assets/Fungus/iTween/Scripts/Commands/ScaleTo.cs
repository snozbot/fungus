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
	             "Scale To", 
	             "Changes a game object's scale to a specified value over time.")]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class ScaleTo : iTweenCommand
	{
		[Tooltip("Target transform that the GameObject will scale to")]
		public TransformData _toTransform;

		[Tooltip("Target scale that the GameObject will scale to, if no To Transform is set")]
		public Vector3Data _toScale = new Vector3Data(Vector3.one);

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			if (_toTransform.Value == null)
			{
				tweenParams.Add("scale", _toScale.Value);
			}
			else
			{
				tweenParams.Add("scale", _toTransform.Value);
			}
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ScaleTo(_targetObject.Value, tweenParams);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
		[HideInInspector] [FormerlySerializedAs("toScale")] public Vector3 toScaleOLD;

		protected override void OnEnable()
		{
			base.OnEnable();

			if (toTransformOLD != null)
			{
				_toTransform.Value = toTransformOLD;
				toTransformOLD = null;
			}

			if (toScaleOLD != default(Vector3))
			{
				_toScale.Value = toScaleOLD;
				toScaleOLD = default(Vector3);
			}
		}

		#endregion
	}

}