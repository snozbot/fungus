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

			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinWidth(Mathf.Max(position.width, 300)));

			FungusScriptEditor fungusScriptEditor = Editor.CreateEditor(fungusScript) as FungusScriptEditor;
			fungusScriptEditor.DrawVariablesGUI();

			GUILayout.EndScrollView();
		}
	}

}