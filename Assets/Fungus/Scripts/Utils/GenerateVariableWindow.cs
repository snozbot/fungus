using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void OnGUI()
        {
            DrawMenuPanel();
        }

        private void DrawMenuPanel()
        {
            generator.ClassName = EditorGUILayout.TextField("ClassName", generator.ClassName);
            generator.Category = EditorGUILayout.TextField("Category", generator.Category);
            generator.NamespaceOfClass = EditorGUILayout.TextField("NamespaceOfClass", generator.NamespaceOfClass);

            generator.generateVariableClass = EditorGUILayout.Toggle("Generate Variable", generator.generateVariableClass);
            generator.generatePropertyCommand = EditorGUILayout.Toggle("Generate Property Command", generator.generatePropertyCommand);

            if (GUILayout.Button("Generate Now"))
            {
                try
                {
                    generator.Generate();
                    EditorUtility.DisplayProgressBar("Generating " + generator.ClassName, "Importing Scripts", 0);
                    AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                    //throw e;
                }
                generator = new VariableScriptGenerator();
                EditorUtility.ClearProgressBar();
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

    /// <summary>
    /// Helper script that auto generates the required script for FungusVariables that wrap existing classes.
    /// 
    /// Intended to speed up the creation of fungus wrappers of unity builtin types and custom user types.
    /// </summary>
    public class VariableScriptGenerator
    {
        private string _namespaceOfClass = "";
        public string NamespaceOfClass { get { return _namespaceOfClass; } set { _namespaceOfClass = value; } }
        public string ClassName { get; set; }
        private string _category = "Other";
        public string Category { get { return _category; } set { _category = value; } }
        public bool generateVariableClass = true, generatePropertyCommand = true;


        StringBuilder enumBuilder = new StringBuilder("public enum Property { "), getBuilder = new StringBuilder("switch (property)\n{"), setBuilder = new StringBuilder("switch (property)\n{");

        //data and helper for a single native to single fungus type
        public class FungusVariableTypeHelper
        {

            private HashSet<Type> usedTypes = new HashSet<Type>();
            private List<TypeHandler> handlers = new List<TypeHandler>();

            public class TypeHandler
            {
                public TypeHandler(Type native, Type fungusType, string localName, string nativeAsString = null, string fungusTypeAsString = null)
                {
                    NativeType = native;
                    NativeTypeString = string.IsNullOrEmpty(nativeAsString) ? native.Name : nativeAsString;
                    FungusType = fungusType;
                    FungusTypeString = string.IsNullOrEmpty(fungusTypeAsString) ? fungusType.Name : fungusTypeAsString;
                    LocalVariableName = localName;
                }

                public Type NativeType { get; set; }
                public Type FungusType { get; set; }
                public string NativeTypeString { get; set; }
                public string FungusTypeString { get; set; }
                public string LocalVariableName { get; set; }
                public string GetLocalVariableNameWithDeclaration()
                {
                    return "var " + LocalVariableName + " = inOutVar as " + FungusTypeString + ';';
                }
                public string GetVarPropText()
                {
                    return "typeof(" + FungusTypeString + ")";
                }
            }

            public void AddHandler(TypeHandler t)
            {
                handlers.Add(t);
            }

            public bool IsTypeHandled(Type t)
            {
                return handlers.Find(x => x.NativeType == t) != null;
            }

            public string GetSpecificVariableVarientFromType(Type t)
            {
                usedTypes.Add(t);

                var loc = handlers.Find(x => x.NativeType == t);
                if (loc != null)
                {
                    return loc.LocalVariableName;
                }
                else
                {
                    return "ERROR - Unsupprted type requested";
                }
            }

            public string GetUsedTypeVars()
            {

                StringBuilder sb = new StringBuilder();

                foreach (Type t in usedTypes)
                {
                    var loc = handlers.Find(x => x.NativeType == t);
                    if (loc != null)
                    {
                        sb.AppendLine(loc.GetLocalVariableNameWithDeclaration());
                    }
                }

                return sb.ToString();
            }

            public string GetVariablePropertyTypeOfs()
            {
                StringBuilder sb = new StringBuilder();

                foreach (Type t in usedTypes)
                {
                    var loc = handlers.Find(x => x.NativeType == t);
                    if (loc != null)
                    {
                        if (sb.Length > 0)
                            sb.AppendLine(",");

                        sb.Append(loc.GetVarPropText());
                    }
                }

                return sb.ToString();
            }
        }

        private FungusVariableTypeHelper helper = new FungusVariableTypeHelper();

        #region consts
        const string ScriptLocation = "./Assets/Fungus/Scripts/";
        const string PropertyScriptLocation = ScriptLocation + "Commands/Property/";
        const string VaraibleScriptLocation = ScriptLocation + "VariableTypes/";
        const string EditorScriptLocation = VaraibleScriptLocation + "Editor/";
        const string EditorScriptTemplate = @"using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{{
    [CustomPropertyDrawer(typeof({0}Data))]
    public class {0}DataDrawer : VariableDataDrawer<{0}Variable>
    {{ }}
}}";

        
        const string ScriptTemplate = @"using UnityEngine;
{1}

namespace Fungus
{{
    /// <summary>
    /// {0} variable type.
    /// </summary>
    [VariableInfo(""{3}"", ""{0}"")]
    [AddComponentMenu("""")]
	[System.Serializable]
	public class {0}Variable : VariableBase<{0}>
	{{ }}

	/// <summary>
	/// Container for a {0} variable reference or constant value.
	/// </summary>
	[System.Serializable]
	public struct {0}Data
	{{
		[SerializeField]
		[VariableProperty(""<Value>"", typeof({0}Variable))]
		public {0}Variable {2}Ref;

		[SerializeField]
		public {0} {2}Val;

		public static implicit operator {0}({0}Data {0}Data)
		{{
			return {0}Data.Value;
		}}

		public {0}Data({0} v)
		{{
			{2}Val = v;
			{2}Ref = null;
		}}

		public {0} Value
		{{
			get {{ return ({2}Ref == null) ? {2}Val : {2}Ref.Value; }}
			set {{ if ({2}Ref == null) {{ {2}Val = value; }} else {{ {2}Ref.Value = value; }} }}
		}}

		public string GetDescription()
		{{
			if ({2}Ref == null)
			{{
				return {2}Val.ToString();
			}}
			else
			{{
				return {2}Ref.Key;
			}}
		}}
	}}
}}";
        const string PropertyScriptTemplate = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//0 typeo
//1 prop enum
//2 lower class name
//3 get generated
//4 set generated
//5 used vars
//6 variableProperty Type of

namespace Fungus
{{
    // <summary>
    /// Get or Set a property of a {0} component
    /// </summary>
    [CommandInfo(""{0}"",
                 ""Property"",
                 ""Get or Set a property of a {0} component"")]
    [AddComponentMenu("""")]
    public class {0}Property : BaseVariableProperty
    {{
		//generated property
        {1}
		
        [SerializeField]
        protected Property property;
		
        [SerializeField]
        protected {0}Data {2}Data;

        [SerializeField]
        [VariableProperty({6})]
		
        protected Variable inOutVar;

        public override void OnEnter()
        {{
            {5}

            var target = {2}Data.Value;

            switch (getOrSet)
            {{
                case GetSet.Get:
                    {3}
                    break;
                case GetSet.Set:
                    {4}
                    break;
                default:
                    break;
            }}

            Continue();
        }}

        public override string GetSummary()
        {{
            if ({2}Data.Value == null)
            {{
                return ""Error: no {2} set"";
            }}

            if (inOutVar == null)
            {{
                return ""Error: no variable set to push or pull data to or from"";
            }}

            return getOrSet.ToString() + "" "" + property.ToString();
        }}

        public override Color GetButtonColor()
        {{
            return new Color32(235, 191, 217, 255);
        }}

        public override bool HasReference(Variable variable)
        {{
            if ({2}Data.{2}Ref == variable || inOutVar == variable)
                return true;

            return false;
        }}

    }}
}}";
        const string DefaultCaseFailure = @" default:
                                Debug.Log(""Unsupported get or set attempted"");
                            break;
                    }";


        #endregion
        public VariableScriptGenerator()
        {
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(bool), typeof(BooleanVariable), "iob"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(float), typeof(FloatVariable), "iof"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(int), typeof(IntegerVariable), "ioi"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(string), typeof(StringVariable), "ios"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Animator), typeof(AnimatorVariable), "ioani"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(AudioSource), typeof(AudioSourceVariable), "ioaud"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Color), typeof(ColorVariable), "iocol"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(GameObject), typeof(GameObjectVariable), "iogo"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Material), typeof(MaterialVariable), "iomat"));
            //we skip object
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Rigidbody2D), typeof(Rigidbody2DVariable), "iorb2d"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Sprite), typeof(SpriteVariable), "iospr"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Texture), typeof(TextureVariable), "iotex"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Transform), typeof(TransformVariable), "iot"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Vector2), typeof(Vector2Variable), "iov2"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Vector3), typeof(Vector3Variable), "iov"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Quaternion), typeof(QuaternionVariable), "ioq"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Matrix4x4), typeof(Matrix4x4Variable), "iom4"));
        }

        public void Generate()
        {
            if (ClassName.Length == 0)
                throw new Exception("No Class name provided.");

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToList();

            //must be a valid class
            Type resTargetClass = types.Find(x => x.Name == ClassName);

            if (resTargetClass == null)
                throw new Exception("No Type of name " + ClassName + " exists.");

            //don't allow dups
            var genClassName = (ClassName + "Variable");
            Type resGeneratedClass = types.Find(x => x.Name == genClassName);
            Type resGeneratedDrawerClass = types.Find(x => x.Name == (ClassName + "VariableDrawer"));
            Type resGeneratedPropCommandClass = types.Find(x => x.Name == (ClassName + "Property"));

            EditorUtility.DisplayProgressBar("Generating " + ClassName, "Starting", 0);


            try
            {
                var lowerClassName = Char.ToLowerInvariant(ClassName[0]) + ClassName.Substring(1);
                var fileName = VaraibleScriptLocation + ClassName + "Variable.cs";
                if ((resGeneratedClass == null || System.IO.File.Exists(fileName))
                    && !resTargetClass.IsAbstract && generateVariableClass)
                {
                    NamespaceOfClass = NamespaceOfClass.Length > 0 ? ("using " + NamespaceOfClass + ";") : string.Empty;
                    EditorUtility.DisplayProgressBar("Generating " + ClassName, "Variable", 0);
                    var scriptContents = string.Format(ScriptTemplate, ClassName, NamespaceOfClass, lowerClassName, Category);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(VaraibleScriptLocation));
                    System.IO.File.WriteAllText(fileName, scriptContents);
                    Debug.Log("Created " + fileName);
                }

                fileName = EditorScriptLocation + ClassName + "VariableDrawer.cs";
                if ((resGeneratedDrawerClass == null || System.IO.File.Exists(fileName))
                    && !resTargetClass.IsAbstract && generateVariableClass)
                {
                    EditorUtility.DisplayProgressBar("Generating " + ClassName, "VariableDrawer", 0);
                    var editorScriptContents = string.Format(EditorScriptTemplate, ClassName);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(EditorScriptLocation));
                    System.IO.File.WriteAllText(fileName, editorScriptContents);
                    Debug.Log("Created " + fileName);
                }

                fileName = PropertyScriptLocation + ClassName + "Property.cs";
                if ((resGeneratedPropCommandClass == null || System.IO.File.Exists(fileName))
                    && generatePropertyCommand)
                {
                    EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property", 0);
                    {
                        EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property Scanning Fields", 0);
                        var fields = resTargetClass.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (helper.IsTypeHandled(fields[i].FieldType))
                            {

                                var actualName = fields[i].Name;
                                var upperCaseName = Char.ToUpperInvariant(actualName[0]) + actualName.Substring(1);
                                //add it to the enum
                                AddToEnum(upperCaseName);

                                //add it to the get
                                AddToGet(fields[i].FieldType, upperCaseName, actualName);

                                //add it to the set
                                AddToSet(fields[i].FieldType, upperCaseName, actualName);
                            }
                        }
                    }
                    {
                        EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property Scanning Props", 0);
                        var props = resTargetClass.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                        for (int i = 0; i < props.Length; i++)
                        {
                            if (helper.IsTypeHandled(props[i].PropertyType) && props[i].GetIndexParameters().Length == 0 && !IsObsolete(props[i].GetCustomAttributes(false)))
                            {
                                var actualName = props[i].Name;
                                var upperCaseName = Char.ToUpperInvariant(actualName[0]) + actualName.Substring(1);
                                //add it to the enum
                                AddToEnum(upperCaseName);

                                if (props[i].CanRead)
                                {
                                    //add it to the get
                                    AddToGet(props[i].PropertyType, upperCaseName, actualName);
                                }
                                if (props[i].CanWrite)
                                {
                                    //add it to the set
                                    AddToSet(props[i].PropertyType, upperCaseName, actualName);
                                }
                            }
                        }
                    }


                    EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property Building", 0);
                    
                    //finalise buidlers
                    setBuilder.AppendLine(DefaultCaseFailure);
                    var setcontents = setBuilder.ToString();

                    getBuilder.AppendLine(DefaultCaseFailure);
                    var getcontents = getBuilder.ToString();

                    enumBuilder.AppendLine("}");
                    var enumgen = enumBuilder.ToString();

                    var typeVars = helper.GetUsedTypeVars();
                    var variablePropertyTypes = helper.GetVariablePropertyTypeOfs();


                    //write to file
                    EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property Writing", 0);
                    var propScriptContents = string.Format(PropertyScriptTemplate, ClassName, enumgen, lowerClassName, getcontents, setcontents, typeVars, variablePropertyTypes);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(PropertyScriptLocation));
                    System.IO.File.WriteAllText(fileName, propScriptContents);
                    Debug.Log("Created " + fileName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            EditorUtility.ClearProgressBar();
        }

        private void AddToSet(Type fieldType, string nameEnum, string nameVariable)
        {
            setBuilder.Append("case Property.");
            setBuilder.Append(nameEnum);
            setBuilder.AppendLine(":");
            setBuilder.Append("target.");
            setBuilder.Append(nameVariable);
            setBuilder.Append(" = ");
            setBuilder.Append(helper.GetSpecificVariableVarientFromType(fieldType));
            setBuilder.Append(".Value;\nbreak;\n");
        }

        private void AddToGet(Type fieldType, string nameEnum, string nameVariable)
        {
            getBuilder.Append("case Property.");
            getBuilder.Append(nameEnum);
            getBuilder.AppendLine(":");
            getBuilder.Append(helper.GetSpecificVariableVarientFromType(fieldType));
            getBuilder.Append(".Value = target.");
            getBuilder.Append(nameVariable);
            getBuilder.Append(";\nbreak;\n");
        }

        private void AddToEnum(string name)
        {
            enumBuilder.Append(name);
            enumBuilder.AppendLine(", ");
        }

        private bool IsObsolete(object[] attrs)
        {
            if (attrs.Length > 0)
                return attrs.First(x => x.GetType() == typeof(ObsoleteAttribute)) != null;
            return false;
        }
    }
}
