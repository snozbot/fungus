using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate Add", 
	             "Rotates a game object by the specified angles over time.")]
	[AddComponentMenu("")]
	public class RotateAdd : iTweenCommand, ISerializationCallbackReceiver 
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("offset")] public Vector3 offsetOLD;
		#endregion

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
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateAdd(_targetObject.Value, tweenParams);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public void OnBeforeSerialize()
		{}

		public void OnAfterDeserialize()
		{
			if (offsetOLD != default(Vector3))
			{
				_offset.Value = offsetOLD;
				offsetOLD = default(Vector3);
			}
		}
	}

}