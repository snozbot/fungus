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
			get { return (scope == VariableScope.Local) ? booleanValue : Variables.GetBoolean(key); }
			set { if (scope == VariableScope.Local) { booleanValue = value; } else { Variables.SetBoolean(key, value); } }
		}

		public int IntegerValue
		{
			get { return (scope == VariableScope.Local) ? integerValue : Variables.GetInteger(key); }
			set { if (scope == VariableScope.Local) { integerValue = value; } else { Variables.SetInteger(key, value); } }
		}
		
		public float FloatValue
		{
			get { return (scope == VariableScope.Local) ? floatValue : Variables.GetFloat(key); }
			set { if (scope == VariableScope.Local) { floatValue = value; } else {	Variables.SetFloat(key, value); } }
		}
		
		public string StringValue
		{
			get { return (scope == VariableScope.Local) ? stringValue : Variables.GetString(key); }
			set { if (scope == VariableScope.Local) { stringValue = value; } else { Variables.SetString(key, value); } }
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