using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CommandInfo("Scripting", 
	             "If", 
	             "If the test expression is true, execute the following block of commands.")]
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
		public StringData stringData;
		
		public override void OnEnter()
		{
			if (parentSequence == null)
			{
				return;
			}

			if (variable == null)
			{
				Continue();
				return;
			}

			EvaluateCondition();
		}

		protected void EvaluateCondition()
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

			if (condition)
			{
				OnTrue();
			}
			else
			{
				OnFalse();
			}
		}

		public void OnTrue()
		{
			Continue();
		}

		public void OnFalse()
		{
			// Find the next Else or EndIf command at the same indent level as this If command
			for (int i = commandIndex; i < parentSequence.commandList.Count; ++i)
			{
				Command nextCommand = parentSequence.commandList[i];
				
				// Find next command at same indent level as this If command
				// Skip disabled commands & comments
				if (!nextCommand.enabled || 
				    nextCommand.GetType() == typeof(Comment) ||
				    nextCommand.indentLevel != indentLevel)
				{
					continue;
				}
				
				System.Type type = nextCommand.GetType();
				if (type == typeof(Else) || 
				    type == typeof(EndIf) || // Legacy support for old EndIf command
				    type == typeof(End))
				{
					if (i >= parentSequence.commandList.Count - 1)
					{
						// Last command in Sequence, so stop
						Stop();
					}
					else
					{
						// Execute command immediately after the Else or End command
						Continue(nextCommand);
						return;
					}
				}
			}

			// No matching End command found, so just stop the sequence
			Stop();
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