using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public enum CompareOperator
	{
		Equals, 				// ==
		NotEquals, 				// !=
		LessThan,				// <
		GreaterThan,			// >
		LessThanOrEquals,		// <=
		GreaterThanOrEquals		// >=
	}
	
	[AddComponentMenu("")]
	public abstract class Condition : Command
	{
		[Tooltip("The type of comparison to be performed")]
		public CompareOperator compareOperator;

		public static string GetOperatorDescription(CompareOperator compareOperator)
		{
			string summary = "";
			switch (compareOperator)
			{
			case CompareOperator.Equals:
				summary += "==";
				break;
			case CompareOperator.NotEquals:
				summary += "!=";
				break;
			case CompareOperator.LessThan:
				summary += "<";
				break;
			case CompareOperator.GreaterThan:
				summary += ">";
				break;
			case CompareOperator.LessThanOrEquals:
				summary += "<=";
				break;
			case CompareOperator.GreaterThanOrEquals:
				summary += ">=";
				break;
			}

			return summary;
		}
	}

}