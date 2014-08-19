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

	public class FungusVariable : MonoBehaviour
	{
		public VariableScope scope;
		public string key = "";
	}
}
