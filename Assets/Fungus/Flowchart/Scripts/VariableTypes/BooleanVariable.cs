/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{

	[VariableInfo("", "Boolean")]
	[AddComponentMenu("")]
    [System.Serializable]
	public class BooleanVariable : VariableBase<bool>
	{
		public virtual bool Evaluate(CompareOperator compareOperator, bool booleanValue)
		{
			bool condition = false;
			
			bool lhs = value;
			bool rhs = booleanValue;
			
			switch (compareOperator)
			{
			case CompareOperator.Equals:
				condition = lhs == rhs;
				break;
			case CompareOperator.NotEquals:
			default:
				condition = lhs != rhs;
				break;
			}
			
			return condition;
		}

	}

	[System.Serializable]
	public struct BooleanData
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(BooleanVariable))]
		public BooleanVariable booleanRef;

		[SerializeField]
		public bool booleanVal;

		public BooleanData(bool v)
		{
			booleanVal = v;
			booleanRef = null;
		}
		
		public static implicit operator bool(BooleanData booleanData)
		{
			return booleanData.Value;
		}

		public bool Value
		{
			get { return (booleanRef == null) ? booleanVal : booleanRef.value; }
			set { if (booleanRef == null) { booleanVal = value; } else { booleanRef.value = value; } }
		}

		public string GetDescription()
		{
			if (booleanRef == null)
			{
				return booleanVal.ToString();
			}
			else
			{
				return booleanRef.key;
			}
		}
	}

}