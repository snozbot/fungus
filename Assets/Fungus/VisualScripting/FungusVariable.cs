using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	public enum VariableScope
	{
		Local,
		Global
	};
	
	public class FungusVariable : MonoBehaviour
	{
		public VariableScope scope;
		public string key;
	}
}
