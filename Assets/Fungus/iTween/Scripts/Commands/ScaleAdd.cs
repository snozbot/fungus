using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Scale Add", 
	             "Changes a game object's scale by a specified offset over time.")]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class ScaleAdd : iTweenCommand
	{
		[Tooltip("A scale offset in space the GameObject will animate to")]
		public Vector3Data _offset;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			tweenParams.Add("amount", _offset.Value);
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
            tweenParams.Add("ignoretimescale", ignoreTimeScale);
            tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ScaleAdd(_targetObject.Value, tweenParams);
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