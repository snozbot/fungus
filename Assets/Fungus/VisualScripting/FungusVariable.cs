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
	}

	public enum VariableScope
	{
		Local,
		Global
	}

	[System.Serializable]
	public class Variable
	{
		public string key;
		public VariableType type;
		public VariableScope scope;
		public BooleanData booleanData;
		public IntegerData integerData;
		public FloatData floatData;
		public StringData stringData;
	}

	public class FungusVariable : MonoBehaviour
	{
		public VariableScope scope;
		public string key;
	}
}
