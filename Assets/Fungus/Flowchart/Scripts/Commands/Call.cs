using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CommandInfo("Scripting", 
	             "Call", 
	             "Execute another block in the same Flowchart.")]
	[AddComponentMenu("")]
	public class Call : Command
	{	
		[FormerlySerializedAs("targetSequence")]
		[Tooltip("Block to execute")]
		public Block targetBlock;
	
		public override void OnEnter()
		{
			if (targetBlock != null)
			{
				ExecuteBlock(targetBlock);
			}
			else
			{		
				Continue();
			}
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
				return "<Continue>";
			}

			return targetBlock.blockName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}