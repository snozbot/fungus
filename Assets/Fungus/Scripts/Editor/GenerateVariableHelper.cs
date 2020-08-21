// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Helper script that auto generates the required script for FungusVariables that wrap existing classes.
    ///
    /// Intended to speed up the creation of fungus wrappers of unity builtin types and custom user types.
    /// </summary>
    public class VariableScriptGenerator
    {
        public string NamespaceUsingDeclare { get; set; }

        public bool PreviewOnly { get; set; }
        private string _category = "Other";
        public string Category { get { return _category; } set { _category = value; } }

        public List<Type> types { get; private set; }

        public bool generateVariableClass = true,
            generatePropertyCommand = true,
            generateVariableDataClass = true,
            generateOnlyDeclaredMembers = true;

        public string ClassName { get { return TargetType.Name; } }
        public string CamelCaseClassName { get { return Char.ToLowerInvariant(ClassName[0]) + ClassName.Substring(1); } }

        public string GenClassName { get { return ClassName + "Variable"; } }

        public string VariableFileName { get { return VaraibleScriptLocation + ClassName + "Variable.cs"; } }
        public string VariableEditorFileName { get { return EditorScriptLocation + ClassName + "VariableDrawer.cs"; } }
        public string PropertyFileName { get { return PropertyScriptLocation + ClassName + "Property.cs"; } }

        private Type _targetType;

        public Type TargetType
        {
            get
            {
                return _targetType;
            }
            set
            {
                _targetType = value;
                ExistingGeneratedClass = null;
                ExistingGeneratedDrawerClass = null;
                ExistingGeneratedPropCommandClass = null;

                if (_targetType != null)
                {
                    ExistingGeneratedClass = types.Find(x => x.Name == GenClassName);
                    ExistingGeneratedDrawerClass = types.Find(x => x.Name == (ClassName + "VariableDrawer"));
                    ExistingGeneratedPropCommandClass = types.Find(x => x.Name == (ClassName + "Property"));
                    if (ExistingGeneratedPropCommandClass != null)
                    {
                        var nested = ExistingGeneratedPropCommandClass.GetNestedTypes().ToList();
                        ExistingGeneratedPropEnumClass = nested.Find(x => x.Name == "Property");
                    }
                }
            }
        }

        public Type ExistingGeneratedClass { get; private set; }
        public Type ExistingGeneratedDrawerClass { get; private set; }
        public Type ExistingGeneratedPropCommandClass { get; private set; }
        public Type ExistingGeneratedPropEnumClass { get; private set; }

        private StringBuilder getBuilder, setBuilder;// = new StringBuilder("switch (property)\n{");
        private List<string> enumBuilder = new List<string>();

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
                if (handlers.Find(x => x.NativeType == t.NativeType || x.LocalVariableName == t.LocalVariableName) != null)
                {
                    Debug.LogError("AddHandler rejected due to duplicate native field or local variable name");
                    return;
                }

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
                        sb.Append("            ");
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
                        {
                            sb.AppendLine(",");
                            sb.Append("                          ");
                        }
                        sb.Append(loc.GetVarPropText());
                    }
                }

                return sb.ToString();
            }
        }

        private FungusVariableTypeHelper helper = new FungusVariableTypeHelper();

        #region consts

        // need to also track if they are preview only
        static public readonly string[] AllGeneratedVariableTypeClassNames =
        {
            "Animator",
            "AudioSource",
            //"Boolean",
            "Color",
            "Collection",
            "Collider",
            "Collider2D",
            "Collision",
            "Collision2D",
            "ControllerColliderHit",
            //"Float",
            "GameObject",
            //"Integer",
            "Material",
            "Matrix4x4",
            //"Object",
            "Quaternion",
            "Rigidbody",
            "Rigidbody2D",
            "Sprite",
            //"String",
            "Texture",
            "Transform",
            "Vector2",
            "Vector3",
            "Vector4"};

        private const string ScriptLocation = "./Assets/Fungus/Scripts/";
        private const string PropertyScriptLocation = ScriptLocation + "Commands/Property/";
        private const string VaraibleScriptLocation = ScriptLocation + "VariableTypes/";
        private const string EditorScriptLocation = ScriptLocation + "Editor/VariableTypes/";

        private const string EditorDataDrawerScriptTemplate = @"// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

/*This script has been, partially or completely, generated by the Fungus.GenerateVariableWindow*/
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{{
    [CustomPropertyDrawer(typeof({0}Data))]
    public class {0}DataDrawer : VariableDataDrawer<{0}Variable>
    {{ }}
}}";

        //0 ClassName
        //1 NamespaceOfClass
        //2 lowerClassName
        //3 Category
        //4 previewOnly
        //5 full name

        private const string VariableAndDataScriptTemplate = @"// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

/*This script has been, partially or completely, generated by the Fungus.GenerateVariableWindow*/
using UnityEngine;
{1}

namespace Fungus
{{
    /// <summary>
    /// {0} variable type.
    /// </summary>
    [VariableInfo(""{3}"", ""{0}""{4})]
    [AddComponentMenu("""")]
	[System.Serializable]
	public class {0}Variable : VariableBase<{5}>
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
		public {5} {2}Val;

		public static implicit operator {5}({0}Data {0}Data)
		{{
			return {0}Data.Value;
		}}

		public {0}Data({5} v)
		{{
			{2}Val = v;
			{2}Ref = null;
		}}

		public {5} Value
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

        //0 ClassName
        //1 NamespaceOfClass
        //2 lowerClassName
        //3 Category
        //4 previewOnly
        //5 full name

        private const string VariableOnlyScriptTemplate = @"// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

/*This script has been, partially or completely, generated by the Fungus.GenerateVariableWindow*/
using UnityEngine;
{1}

namespace Fungus
{{
    /// <summary>
    /// {0} variable type.
    /// </summary>
    [VariableInfo(""{3}"", ""{0}""{4})]
    [AddComponentMenu("""")]
	[System.Serializable]
	public class {0}Variable : VariableBase<{5}>
	{{ }}
}}";

        //0 typeo
        //1 prop enum
        //2 lower class name
        //3 get generated
        //4 set generated
        //5 used vars
        //6 variableProperty Type of
        //7 null check summary
        private const string DataBasedPropertyScriptTemplate = @"// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

/*This script has been, partially or completely, generated by the Fungus.GenerateVariableWindow*/
using UnityEngine;

namespace Fungus
{{
    // <summary>
    /// Get or Set a property of a {0} component
    /// </summary>
    [CommandInfo(""Property"",
                 ""{0}"",
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
        {{{7}
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

        //0 typeo
        //1 prop enum
        //2 lower class name
        //3 get generated
        //4 set generated
        //5 used vars
        //6 variableProperty Type of
        //7 null check summary
        private const string VariableOnlyPropertyScriptTemplate = @"// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

/*This script has been, partially or completely, generated by the Fungus.GenerateVariableWindow*/
using UnityEngine;

namespace Fungus
{{
    // <summary>
    /// Get or Set a property of a {0} component
    /// </summary>
    [CommandInfo(""Property"",
                 ""{0}"",
                 ""Get or Set a property of a {0} component"")]
    [AddComponentMenu("""")]
    public class {0}Property : BaseVariableProperty
    {{
		//generated property
        {1}

        [SerializeField]
        protected Property property;

        [SerializeField]
        [VariableProperty(typeof({0}Variable))]
        protected {0}Variable {2}Var;

        [SerializeField]
        [VariableProperty({6})]
        protected Variable inOutVar;

        public override void OnEnter()
        {{
{5}

            var target = {2}Var.Value;

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
        {{{7}
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
            if ({2}Var == variable || inOutVar == variable)
                return true;

            return false;
        }}
    }}
}}";

        private const string DefaultCaseFailure = @"                        default:
                            Debug.Log(""Unsupported get or set attempted"");
                            break;
                    }";

        private const string DataNullCheckSummary = @"
            if ({0}Data.Value == null)
            {{
                return ""Error: no {0} set"";
            }}";

        private const string VariableOnlyNullCheckSummary = @"
            if ({0}Var == null)
            {{
                return ""Error: no {0}Var set"";
            }}";

        #endregion consts

        public VariableScriptGenerator()
        {
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Animator), typeof(AnimatorVariable), "ioani"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(AudioSource), typeof(AudioSourceVariable), "ioaud"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(bool), typeof(BooleanVariable), "iob"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Collider2D), typeof(Collider2DVariable), "ioc2d"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Collider), typeof(ColliderVariable), "ioc"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Collection), typeof(CollectionVariable), "iocollect"));
            //we don't need to do collision varaibles
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Color), typeof(ColorVariable), "iocol"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(float), typeof(FloatVariable), "iof"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(GameObject), typeof(GameObjectVariable), "iogo"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(int), typeof(IntegerVariable), "ioi"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Material), typeof(MaterialVariable), "iomat"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Matrix4x4), typeof(Matrix4x4Variable), "iom4"));
            //we skip object
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Quaternion), typeof(QuaternionVariable), "ioq"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Rigidbody2D), typeof(Rigidbody2DVariable), "iorb2d"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Rigidbody), typeof(RigidbodyVariable), "iorb"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Sprite), typeof(SpriteVariable), "iospr"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(string), typeof(StringVariable), "ios"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Texture), typeof(TextureVariable), "iotex"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Transform), typeof(TransformVariable), "iot"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Vector2), typeof(Vector2Variable), "iov2"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Vector3), typeof(Vector3Variable), "iov"));
            helper.AddHandler(new FungusVariableTypeHelper.TypeHandler(typeof(Vector4), typeof(Vector4Variable), "iov4"));

            types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToList();
        }

        public void Generate()
        {
            if (TargetType == null)
                throw new Exception("No type given");

            EditorUtility.DisplayProgressBar("Generating " + ClassName, "Starting", 0);
            try
            {
                if (generateVariableClass)
                {
                    Func<string> lam = () =>
                    {
                        var usingDec = !string.IsNullOrEmpty(NamespaceUsingDeclare) ? ("using " + NamespaceUsingDeclare + ";") : string.Empty;
                        return string.Format(generateVariableDataClass ? VariableAndDataScriptTemplate : VariableOnlyScriptTemplate,
                            ClassName, usingDec, CamelCaseClassName, Category, PreviewOnly ? ", IsPreviewedOnly = true" : "", TargetType.FullName);
                    };
                    FileSaveHelper("Variable", VaraibleScriptLocation, VariableFileName, lam);
                }

                if (generateVariableClass && generateVariableDataClass)
                {
                    Func<string> lam = () => { return string.Format(EditorDataDrawerScriptTemplate, ClassName); };
                    FileSaveHelper("VariableDrawer", EditorScriptLocation, VariableEditorFileName, lam);
                }

                if (generatePropertyCommand)
                {
                    GeneratePropertyCommand();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            EditorUtility.ClearProgressBar();
        }

        private void GeneratePropertyCommand()
        {
            enumBuilder.Add("public enum Property \n        { \n");
            getBuilder = new StringBuilder("switch (property)\n                    {\n".Replace("\n", System.Environment.NewLine));
            setBuilder = new StringBuilder("switch (property)\n                    {\n".Replace("\n", System.Environment.NewLine));

            AddAllExistingEnumNames();

            EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property", 0);
            int numAdded = PropertyFieldLogic();
            numAdded += PropertyPropLogic();

            EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property Building", 0);

            //finalise buidlers
            setBuilder.AppendLine(DefaultCaseFailure);
            var setcontents = setBuilder.ToString();

            getBuilder.AppendLine(DefaultCaseFailure);
            var getcontents = getBuilder.ToString();

            enumBuilder.Add("        }\n");
            var enumgen = String.Join(null, enumBuilder.ToArray());
            enumgen = enumgen.Replace("\n", System.Environment.NewLine);

            var typeVars = helper.GetUsedTypeVars();
            var variablePropertyTypes = helper.GetVariablePropertyTypeOfs();

            string nullCheck = "";

            if (TargetType.IsClass)
            {
                nullCheck = string.Format(generateVariableDataClass ? DataNullCheckSummary : VariableOnlyNullCheckSummary, CamelCaseClassName);
            }

            //write to file
            Func<string> propContentOp = () =>
            {
                return string.Format(generateVariableDataClass ? DataBasedPropertyScriptTemplate : VariableOnlyPropertyScriptTemplate,
ClassName, enumgen, CamelCaseClassName, getcontents, setcontents, typeVars, variablePropertyTypes, nullCheck);
            };

            //only generate the file if we actually found variables to get and or set
            if (numAdded > 0)
            {
                FileSaveHelper("Property", PropertyScriptLocation, PropertyFileName, propContentOp);
            }
            else
            {
                Debug.LogWarning("Attempted to generate property class for " + ClassName + " but found no variables to get or set");
            }
        }

        private void AddAllExistingEnumNames()
        {
            if (ExistingGeneratedPropEnumClass != null)
            {
                if (ExistingGeneratedPropEnumClass.IsEnum)
                {
                    var res = Enum.GetNames(ExistingGeneratedPropEnumClass);
                    for (int i = 0; i < res.Length; i++)
                    {
                        AddToEnum(res[i]);
                    }
                }
            }
        }

        private System.Reflection.BindingFlags GetBindingFlags()
        {
            if (generateOnlyDeclaredMembers)
                return System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly;
            return System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
        }

        private int PropertyFieldLogic()
        {
            int retval = 0;

            EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property Scanning Fields", 0);
            var fields = TargetType.GetFields(GetBindingFlags());
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
                    retval++;
                }
            }

            return retval;
        }

        private int PropertyPropLogic()
        {
            int retval = 0;

            EditorUtility.DisplayProgressBar("Generating " + ClassName, "Property Scanning Props", 0);
            var props = TargetType.GetProperties(GetBindingFlags());
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

                    if (props[i].CanRead || props[i].CanWrite)
                        retval++;
                }
            }

            return retval;
        }

        private void AddToSet(Type fieldType, string nameEnum, string nameVariable)
        {
            setBuilder.Append("                        case Property.");
            setBuilder.Append(nameEnum);
            setBuilder.AppendLine(":");
            setBuilder.Append("                            target.");
            setBuilder.Append(nameVariable);
            setBuilder.Append(" = ");
            setBuilder.Append(helper.GetSpecificVariableVarientFromType(fieldType));
            setBuilder.AppendLine(".Value;");
            setBuilder.AppendLine("                            break;");
        }

        private void AddToGet(Type fieldType, string nameEnum, string nameVariable)
        {
            getBuilder.Append("                        case Property.");
            getBuilder.Append(nameEnum);
            getBuilder.AppendLine(":");
            getBuilder.Append("                            " + helper.GetSpecificVariableVarientFromType(fieldType));
            getBuilder.Append(".Value = target.");
            getBuilder.Append(nameVariable);
            getBuilder.AppendLine(";");
            getBuilder.AppendLine("                            break;");
        }

        private void AddToEnum(string name)
        {
            if (enumBuilder.IndexOf(name) == -1)
            {
                enumBuilder.Add("            ");
                enumBuilder.Add(name);
                enumBuilder.Add(", \n");
            }
        }

        private bool IsObsolete(object[] attrs)
        {
            if (attrs.Length > 0)
                return attrs.FirstOrDefault(x => x.GetType() == typeof(ObsoleteAttribute)) != null;
            return false;
        }

        private void FileSaveHelper(string op, string loc, string filename, Func<string> opLambda)
        {
            EditorUtility.DisplayProgressBar("Generating " + ClassName, op, 0);
            var scriptContents = opLambda();
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(loc));
            System.IO.File.WriteAllText(filename, scriptContents);
            Debug.Log("Created " + filename);
        }
    }
}