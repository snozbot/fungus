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

		public string stringValue;
		public int integerValue;
		public float floatValue;
		public bool booleanValue;
	}

}