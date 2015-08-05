using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public enum VariableScope
	{
		Private,
		Public
	}

	public class VariableInfoAttribute : Attribute
	{
		public VariableInfoAttribute(string category, string variableType, int order = 0)
		{
			this.Category = category;
			this.VariableType = variableType;
			this.Order = order;
		}
		
		public string Category { get; set; }
		public string VariableType { get; set; }
		public int Order { get; set; }
	}

	public class VariablePropertyAttribute : PropertyAttribute 
	{
		public VariablePropertyAttribute (params System.Type[] variableTypes) 
		{
			this.VariableTypes = variableTypes;
		}

		public VariablePropertyAttribute (string defaultText, params System.Type[] variableTypes) 
		{
			this.defaultText = defaultText;
			this.VariableTypes = variableTypes;
		}

		public String defaultText = "<None>";

		public System.Type[] VariableTypes { get; set; }
	}

	[RequireComponent(typeof(Flowchart))]
	public abstract class Variable : MonoBehaviour
	{
		public VariableScope scope;

		public string key = "";

		public abstract void OnReset();
	}

	public abstract class VariableBase<T> : Variable
	{
		public T value;
		
		protected T startValue;

		public override void OnReset()
		{
			value = startValue;
		}
		
		public override string ToString()
		{
			return value.ToString();
		}
		
		protected virtual void Start()
		{
			// Remember the initial value so we can reset later on
			startValue = value;
		}
	}
}
