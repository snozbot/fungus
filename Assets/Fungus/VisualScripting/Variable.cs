using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{

	public enum VariableType
	{
		Boolean,
		Integer,
		Float,
		String
	};

	[Serializable]
	public class Variable
	{
		public string key;
		public VariableType type;

		public bool booleanValue;
		public int integerValue;
		public float floatValue;
		public string stringValue;

		public bool IsSameType(Variable other)
		{
			return (this.type == other.type);
		}

		public bool Assign(Variable other)
		{
			if (!IsSameType(other))
			{
				return false;
			}

			booleanValue = other.booleanValue;
			integerValue = other.integerValue;
			floatValue = other.floatValue;
			stringValue = other.stringValue;

			return true;
		}
	}

	[Serializable]
	public class VariableData
	{
	}

	[Serializable]
	public class BooleanData : VariableData
	{
		public bool value;
	}
	
	[Serializable]
	public class IntegerData : VariableData
	{
		public int value;
	}

	[Serializable]
	public class FloatData : VariableData
	{
		public float value;
	}

	[Serializable]
	public class StringData : VariableData
	{
		public string value;
	}
}