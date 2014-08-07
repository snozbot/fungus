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
			Dictionary<string, VariableType> globalTypes = new Dictionary<string, VariableType>();
			FungusScript[] fungusScripts = GameObject.FindObjectsOfType<FungusScript>();
			foreach (FungusScript fs in fungusScripts)
			{
				foreach (Variable v in fs.variables)
				{
					if (v.scope == VariableScope.Global)
					{
						if (globalTypes.ContainsKey(v.key))
						{
							if (globalTypes[v.key] != v.type)
							{
								GUIStyle errorStyle = new GUIStyle(GUI.skin.label);
								errorStyle.normal.textColor = new Color(1,0,0);
								GUILayout.Label("Error: Global '" + v.key + "' must use the same type in all scripts.", errorStyle);
							}
						}
						globalTypes[v.key] = v.type;
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

			foreach (Variable variable in fungusScript.variables)
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

				switch (variable.type)
				{
				case VariableType.Boolean:
					typeString = "Boolean";
					valueString = variable.BooleanValue ? "True" : "False";
					break;
				case VariableType.Integer:
					typeString = "Integer";
					valueString = variable.IntegerValue.ToString();
					break;
				case VariableType.Float:
					typeString = "Float";
					valueString = variable.FloatValue.ToString();
					break;
				case VariableType.String:
					typeString = "String";

					if (variable.StringValue == null ||
						variable.StringValue.Length == 0)
					{
						valueString = "\"\"";
					}
					else
					{
						valueString = variable.StringValue;
					}
					break;
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