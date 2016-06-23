/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CommandInfo("Flow", 
	             "If", 
	             "If the test expression is true, execute the following command block.")]
	[AddComponentMenu("")]
	public class If : Condition
	{
		[Tooltip("Variable to use in expression")]
		[VariableProperty(typeof(BooleanVariable),
		                  typeof(IntegerVariable), 
		                  typeof(FloatVariable), 
		                  typeof(StringVariable))]
		public Variable variable;

		[Tooltip("Boolean value to compare against")]
		public BooleanData booleanData;

		[Tooltip("Integer value to compare against")]
		public IntegerData integerData;

		[Tooltip("Float value to compare against")]
		public FloatData floatData;

		[Tooltip("String value to compare against")]
		public StringDataMulti stringData;
		
		public override void OnEnter()
		{
			if (parentBlock == null)
			{
				return;
			}

			if (variable == null)
			{
				Continue();
				return;
			}

			EvaluateAndContinue();
		}

		public bool EvaluateCondition()
		{
			BooleanVariable booleanVariable = variable as BooleanVariable;
			IntegerVariable integerVariable = variable as IntegerVariable;
			FloatVariable floatVariable = variable as FloatVariable;
			StringVariable stringVariable = variable as StringVariable;
			
			bool condition = false;
			
			if (booleanVariable != null)
			{
				condition = booleanVariable.Evaluate(compareOperator, booleanData.Value);
			}
			else if (integerVariable != null)
			{
				condition = integerVariable.Evaluate(compareOperator, integerData.Value);
			}
			else if (floatVariable != null)
			{
				condition = floatVariable.Evaluate(compareOperator, floatData.Value);
			}
			else if (stringVariable != null)
			{
				condition = stringVariable.Evaluate(compareOperator, stringData.Value);
			}

			return condition;
		}

		protected void EvaluateAndContinue()
		{
			if (EvaluateCondition())
			{
				OnTrue();
			}
			else
			{
				OnFalse();
			}
		}

		protected virtual void OnTrue()
		{
			Continue();
		}

		protected virtual void OnFalse()
		{
			// Last command in block
			if (commandIndex >= parentBlock.commandList.Count)
			{
				StopParentBlock();
				return;
			}

			// Find the next Else, ElseIf or End command at the same indent level as this If command
			for (int i = commandIndex + 1; i < parentBlock.commandList.Count; ++i)
			{
				Command nextCommand = parentBlock.commandList[i];

				if (nextCommand == null)
				{
					continue;
				}

				// Find next command at same indent level as this If command
				// Skip disabled commands, comments & labels
				if (!nextCommand.enabled || 
				    nextCommand.GetType() == typeof(Comment) ||
				    nextCommand.GetType() == typeof(Label) ||
				    nextCommand.indentLevel != indentLevel)
				{
					continue;
				}
				
				System.Type type = nextCommand.GetType();
				if (type == typeof(Else) ||
				    type == typeof(End))
				{
					if (i >= parentBlock.commandList.Count - 1)
					{
						// Last command in Block, so stop
						StopParentBlock();
					}
					else
					{
						// Execute command immediately after the Else or End command
						Continue(nextCommand.commandIndex + 1);
						return;
					}
				}
				else if (type == typeof(ElseIf))
				{
					// Execute the Else If command
					Continue(i);

					return;
				}
			}

			// No matching End command found, so just stop the block
			StopParentBlock();
		}

		public override string GetSummary()
		{
			if (variable == null)
			{
				return "Error: No variable selected";
			}

			string summary = variable.key + " ";
			summary += Condition.GetOperatorDescription(compareOperator) + " ";

			if (variable.GetType() == typeof(BooleanVariable))
			{
				summary += booleanData.GetDescription();
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				summary += integerData.GetDescription();
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				summary += floatData.GetDescription();
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				summary += stringData.GetDescription();
			}

			return summary;
		}

		public override bool HasReference(Variable variable)
		{
			return (variable == this.variable);
		}

		public override bool OpenBlock()
		{
			return true;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}