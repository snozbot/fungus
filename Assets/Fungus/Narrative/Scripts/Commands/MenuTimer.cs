using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Narrative", 
	             "Menu Timer", 
	             "Displays a timer bar and executes a target block if the player fails to select a menu option in time.")]
	[AddComponentMenu("")]
	public class MenuTimer : Command, ISerializationCallbackReceiver 
	{
		[Tooltip("Length of time to display the timer for")]
		public FloatData _duration = new FloatData(1);

		[FormerlySerializedAs("targetSequence")]
		[Tooltip("Block to execute when the timer expires")]
		public Block targetBlock;

		public override void OnEnter()
		{
			MenuDialog menuDialog = MenuDialog.GetMenuDialog();

			if (menuDialog != null &&
			    targetBlock != null)
			{
				menuDialog.ShowTimer(_duration.Value, targetBlock);
			}

			Continue();
		}

		public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
		{
			if (targetBlock != null)
			{
				connectedBlocks.Add(targetBlock);
			}		
		}

		public override string GetSummary()
		{
			if (targetBlock == null)
			{
				return "Error: No target block selected";
			}

			return targetBlock.blockName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (durationOLD != default(float))
			{
				_duration.Value = durationOLD;
				durationOLD = default(float);
			}
		}

		#endregion
	}

}