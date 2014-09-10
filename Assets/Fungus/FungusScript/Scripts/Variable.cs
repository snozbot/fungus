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
	}

	public enum VariableScope
	{
		Local,
		Global
	}

	public class Variable : ScriptableObject
	{
		public VariableScope scope;
		public string key = "";
	}
}
