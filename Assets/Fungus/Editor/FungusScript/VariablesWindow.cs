using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	public class VariablesWindow : EditorWindow 
	{
		Vector2 scrollPos = new Vector2();

		public void OnInspectorUpdate()
		{
			Repaint();
		}

		void OnGUI()
		{
			FungusScript fungusScript = FungusEditorWindow.GetFungusScript();

			if (fungusScript == null)
			{
				GUILayout.Label("No Fungus Script object selected");
				return;
			}

			// Warn about conflicting global variable types
			Dictionary<string, FungusVariable> globals = new Dictionary<string, FungusVariable>();
			FungusScript[] fungusScripts = GameObject.FindObjectsOfType<FungusScript>();
			foreach (FungusScript fs in fungusScripts)
			{
				FungusVariable[] variables = fs.GetComponents<FungusVariable>();
				foreach (FungusVariable v in variables)
				{
					if (v.scope == VariableScope.Global)
					{
						if (globals.ContainsKey(v.key))
						{
							if (globals[v.key].GetType() != v.GetType())
							{
								GUIStyle errorStyle = new GUIStyle(GUI.skin.label);
								errorStyle.normal.textColor = new Color(1,0,0);
								GUILayout.Label("Error: Global '" + v.key + "' must use the same type in all scripts.", errorStyle);
							}
						}
						globals[v.key] = v;
					}
				}
			}

			bool showValues = Application.isPlaying;

			float columnWidth = (position.width - 40) / (showValues ? 4 : 3);

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Key", GUILayout.Width(columnWidth));
			GUILayout.Label("Type", GUILayout.Width(columnWidth));
			GUILayout.Label("Scope", GUILayout.Width(columnWidth));
			if (showValues)
			{
				GUILayout.Label("Value", GUILayout.Width(columnWidth));
			}
			GUILayout.EndHorizontal();

			GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
			boxStyle.margin.left = 0;
			boxStyle.margin.right = 0;
			boxStyle.margin.top = 0;
			boxStyle.margin.bottom = 0;

			List<FungusVariable> fsVariables = fungusScript.variables;
			foreach (FungusVariable variable in fsVariables)
			{
				GUILayout.BeginHorizontal(boxStyle);

				string keyString = variable.key;
				string typeString = "";
				string scopeString = "";
				string valueString = "";

				switch (variable.scope)
				{
				default:
				case VariableScope.Local:
					scopeString = "Local";
					break;
				case VariableScope.Global:
					scopeString = "Global";
					break;
				}

				if (variable.GetType() == typeof(BooleanVariable))
				{
					typeString = "Boolean";
					valueString = (variable as BooleanVariable).Value ? "True" : "False";
				}
				else if (variable.GetType() == typeof(IntegerVariable))
				{
					typeString = "Integer";
					valueString = (variable as IntegerVariable).Value.ToString();
				}
				else if (variable.GetType() == typeof(FloatVariable))
				{
					typeString = "Float";
					valueString = (variable as FloatVariable).Value.ToString();
				}
				else if (variable.GetType() == typeof(StringVariable))
				{
					typeString = "String";
					valueString = (variable as StringVariable).Value;

					if (valueString == null ||
					    valueString.Length == 0)
					{
						valueString = "\"\"";
					}
				}

				GUILayout.Label(keyString, GUILayout.Width(columnWidth));
				GUILayout.Label(typeString, GUILayout.Width(columnWidth));
				GUILayout.Label(scopeString, GUILayout.Width(columnWidth));
				if (showValues)
				{
					GUILayout.Label(valueString, GUILayout.Width(columnWidth));
				}

				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
	}

}