using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{

	public enum VariableType
	{
		Boolean,
		Integer,
		Float,
		String
	};

	public enum VariableScope
	{
		Local,
		Global
	};

	[Serializable]
	public class Variable
	{
		public string key;
		public VariableType type;
		public VariableScope scope;

		bool booleanValue;
		int integerValue;
		float floatValue;
		string stringValue;

		public bool BooleanValue
		{
			get { return booleanValue; }
			set { booleanValue = value; }
		}

		public int IntegerValue
		{
			get { return integerValue; }
			set { integerValue = value; }
		}
		
		public float FloatValue
		{
			get { return floatValue; }
			set { floatValue = value; }
		}
		
		public string StringValue
		{
			get { return stringValue; }
			set { stringValue = value; }
		}

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