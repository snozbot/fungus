/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Jump", 
	             "Move execution to a specific Label command in the same block")]
	[AddComponentMenu("")]
    [ExecuteInEditMode]
	public class Jump : Command
	{
        [Tooltip("Name of a label in this block to jump to")]
        public StringData _targetLabel = new StringData("");

		public override void OnEnter()
		{
			if (_targetLabel.Value == "")
			{
				Continue();
				return;
			}

			foreach (Command command in parentBlock.commandList)
			{
				Label label = command as Label;
				if (label != null &&
				    label.key == _targetLabel.Value)
				{
					Continue(label.commandIndex + 1);
					return;
				}
			}

            // Label not found
            Debug.LogWarning("Label not found: " + _targetLabel.Value);
            Continue();
		}

		public override string GetSummary()
		{
			if (_targetLabel.Value == "")
			{
				return "Error: No label selected";
			}

			return _targetLabel.Value;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("targetLabel")] public Label targetLabelOLD;

        protected virtual void OnEnable()
        {
            if (targetLabelOLD != null)
            {
                _targetLabel.Value = targetLabelOLD.key;
                targetLabelOLD = null;
            }
        }

        #endregion
	}

}