using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Fungus.EditorUtils
{
    /// <summary>
    /// Adds window that generates the require scripts to create a new FungusVariable that wraps an existing type. 
    /// 
    /// These can then be used in the fungus flowcharts. It also generates a *Property command to allow Gets and Sets
    /// on all the elements of that variable that Fungus Understands. The Quaternion and Matrix4x4 have been auto
    /// generated and then auto formatted in visual studio and set to preview only as examples of it's use. 
    /// 
    /// It can be used to help building variable wrappers for builtin Unity types or your own components or classes.
    /// 
    /// To add new types see the VariableScriptGenerator constructor.
    /// </summary>
    public class GenerateVariableWindow : EditorWindow
    {
        private VariableScriptGenerator generator = new VariableScriptGenerator();
        private string userInputClassName = "";
        private List<Type> typeList = new List<Type>();

        public void OnGUI()
        {
            DrawMenuPanel();
        }

        private void DrawMenuPanel()
        {
            EditorGUI.BeginChangeCheck();

            if(GUILayout.Button("Generate All from List"))
            {
                foreach (var item in VariableScriptGenerator.AllGeneratedVariableTypeClassNames)
                {
                    generator.TargetType = item;
                    generator.Generate();

                    generator = new VariableScriptGenerator();
                }
            }

            userInputClassName = EditorGUILayout.TextField("ClassName", userInputClassName);

            if (EditorGUI.EndChangeCheck())
            {
                generator.TargetType = null;

                try
                {
                    typeList = generator.types.Where(x =>  string.Compare(x.Name,userInputClassName,StringComparison.InvariantCultureIgnoreCase) == 0).ToList();
                }
                catch (Exception)
                {
                }
            }

            try
            {
                int index = typeList.IndexOf(generator.TargetType);
                EditorGUI.BeginChangeCheck();
                index = GUILayout.SelectionGrid(index, typeList.Select(x => x.FullName).ToArray(), 1);

                if (index < 0 || index > typeList.Count)
                    index = 0;

                if (EditorGUI.EndChangeCheck() || generator.TargetType == null)
                    generator.TargetType = typeList[index];
            }
            catch (Exception)
            {
                generator.TargetType = null;
            }


            EditorGUILayout.Space();

            if (generator.TargetType == null)
            {
                EditorGUILayout.HelpBox("Must select a type first", MessageType.Info);
            }
            else
            {
                generator.generateVariableClass = EditorGUILayout.Toggle("Generate Variable", generator.generateVariableClass);
                generator.generateVariableDataClass = EditorGUILayout.Toggle("Generate Variable Data", generator.generateVariableDataClass);
                generator.PreviewOnly = EditorGUILayout.Toggle("Variable List preview only", generator.PreviewOnly);

                if (generator.TargetType.IsAbstract)
                {
                    EditorGUILayout.HelpBox(generator.TargetType.FullName + " is abstract. No Variable will be generated", MessageType.Error);
                    generator.generateVariableClass = false;
                }

                if (generator.generateVariableClass)
                {
                    if (generator.ExistingGeneratedClass != null)
                    {
                        EditorGUILayout.HelpBox("Variable Appears to already exist. Overwriting or errors may occur.", MessageType.Warning);
                    }
                    if (generator.ExistingGeneratedDrawerClass != null)
                    {
                        EditorGUILayout.HelpBox("Variable Drawer Appears to already exist. Overwriting or errors may occur.", MessageType.Warning);
                    }

                    generator.Category = EditorGUILayout.TextField("Category", generator.Category);
                    generator.NamespaceUsingDeclare = EditorGUILayout.TextField("NamespaceUsingDeclare", generator.NamespaceUsingDeclare);
                }

                EditorGUILayout.Space();
                generator.generatePropertyCommand = EditorGUILayout.Toggle("Generate Property Command", generator.generatePropertyCommand);
                if (generator.generatePropertyCommand)
                {
                    generator.generateOnlyDeclaredMembers = EditorGUILayout.Toggle("Only declared members", generator.generateOnlyDeclaredMembers);
                    if (generator.ExistingGeneratedPropCommandClass != null)
                    {
                        EditorGUILayout.HelpBox("Property Appears to already exist. Overwriting or errors may occur.", MessageType.Warning);
                    }
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Generate Now"))
                {
                    try
                    {
                        generator.Generate();
                        EditorUtility.DisplayProgressBar("Generating " + userInputClassName, "Importing Scripts", 0);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                        //throw e;
                    }
                    generator = new VariableScriptGenerator();
                    EditorUtility.ClearProgressBar();
                    userInputClassName = "";
                }
            }
        }

        [MenuItem("Tools/Fungus/Utilities/Generate Fungus Varaible")]
        public static GenerateVariableWindow ShowWindow()
        {
            var w = GetWindow(typeof(GenerateVariableWindow), true, "Generate Fungus Varaible", true);
            w.Show();
            return w as GenerateVariableWindow;
        }
    }
}