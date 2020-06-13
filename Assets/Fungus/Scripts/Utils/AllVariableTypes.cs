// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Static cache of all fungus variable types, used by commands that are designed to work on
    /// any and all variable types supported by Fungus.
    ///
    /// New types created need to be added to the list below and also to AnyVariableData and
    /// AnyVaraibleAndDataPair
    /// </summary>
    public static class AllVariableTypes
    {
        public enum VariableAny
        {
            Any
        }

        public static readonly System.Type[] AllFungusVarTypes = new System.Type[]
        {
            typeof(AnimatorVariable),
            typeof(AudioSourceVariable),
            typeof(BooleanVariable),
            typeof(CollectionVariable),
            typeof(Collider2DVariable),
            typeof(ColliderVariable),
            typeof(Collision2DVariable),
            typeof(CollisionVariable),
            typeof(ColorVariable),
            typeof(ControllerColliderHitVariable),
            typeof(FloatVariable),
            typeof(GameObjectVariable),
            typeof(IntegerVariable),
            typeof(MaterialVariable),
            typeof(Matrix4x4Variable),
            typeof(ObjectVariable),
            typeof(QuaternionVariable),
            typeof(Rigidbody2DVariable),
            typeof(RigidbodyVariable),
            typeof(SpriteVariable),
            typeof(StringVariable),
            typeof(TextureVariable),
            typeof(TransformVariable),
            typeof(Vector2Variable),
            typeof(Vector3Variable),
            typeof(Vector4Variable),
        };
    }

    /// <summary>
    /// Collection of every Fungus VariableData type, used in commands that are designed to
    /// support any and all types. Those command just have a AnyVariableData anyVar or
    /// an AnyVaraibleAndDataPair anyVarDataPair to encapsulate the more unpleasant parts.
    ///
    /// New types created need to be added to the list below and also to AllVariableTypes and
    /// AnyVaraibleAndDataPair
    /// 
    /// Note; when using this in a command ensure that RefreshVariableCache is also handled for
    /// string var substitution.
    /// </summary>
    [System.Serializable]
    public partial struct AnyVariableData
    {
        public AnimatorData animatorData;
        public AudioSourceData audioSourceData;
        public BooleanData booleanData;
        public CollectionData collectionData;
        public Collider2DData collider2DData;
        public ColliderData colliderData;
        public ColorData colorData;
        public FloatData floatData;
        public GameObjectData gameObjectData;
        public IntegerData integerData;
        public MaterialData materialData;
        public Matrix4x4Data matrix4x4Data;
        public ObjectData objectData;
        public QuaternionData quaternionData;
        public Rigidbody2DData rigidbody2DData;
        public RigidbodyData rigidbodyData;
        public SpriteData spriteData;
        public StringData stringData;
        public TextureData textureData;
        public TransformData transformData;
        public Vector2Data vector2Data;
        public Vector3Data vector3Data;
        public Vector4Data vector4Data;

        public bool HasReference(Variable var)
        {
            return animatorData.animatorRef == var ||
                   audioSourceData.audioSourceRef == var ||
                   booleanData.booleanRef == var ||
                   collectionData.collectionRef == var ||
                   collider2DData.collider2DRef == var ||
                   colliderData.colliderRef == var ||
                   colorData.colorRef == var ||
                   floatData.floatRef == var ||
                   gameObjectData.gameObjectRef == var ||
                   integerData.integerRef == var ||
                   materialData.materialRef == var ||
                   matrix4x4Data.matrix4x4Ref == var ||
                   objectData.objectRef == var ||
                   quaternionData.quaternionRef == var ||
                   rigidbody2DData.rigidbody2DRef == var ||
                   rigidbodyData.rigidbodyRef == var ||
                   spriteData.spriteRef == var ||
                   stringData.stringRef == var ||
                   textureData.textureRef == var ||
                   transformData.transformRef == var ||
                   vector2Data.vector2Ref == var ||
                   vector3Data.vector3Ref == var ||
                   vector4Data.vector4Ref == var;
        }
    }

    /// <summary>
    /// Pairing of an AnyVariableData and an variable reference. Internal lookup for
    /// making the right kind of variable with the correct data in the AnyVariableData.
    /// This is the primary mechanism for hiding the ugly need to match variable to
    /// correct data type so we can perform comparisons and operations.
    ///
    /// New types created need to be added to the list below and also to AllVariableTypes and
    /// AnyVariableData
    /// 
    /// Note to ensure use of RefreshVariableCacheHelper in commands, see SetVariable for
    /// example.
    /// </summary>
    [System.Serializable]
    public class AnyVariableAndDataPair
    {
        public class TypeActions
        {
            public TypeActions(string dataPropName,
                               System.Func<AnyVariableAndDataPair, Fungus.CompareOperator, bool> comparer,
                               System.Func<AnyVariableAndDataPair, string> desccription,
                               System.Action<AnyVariableAndDataPair, Fungus.SetOperator> set
                              )
            {
                DataPropName = dataPropName;
                CompareFunc = comparer;
                DescFunc = desccription;
                SetFunc = set;
            }

            // used in AnyVaraibleAndDataPair Drawer to show the correct aspect of the AnyVariableData in the editor
            public string DataPropName { get; set; }

            public System.Func<AnyVariableAndDataPair, Fungus.CompareOperator, bool> CompareFunc;
            public System.Func<AnyVariableAndDataPair, string> DescFunc;
            public System.Action<AnyVariableAndDataPair, Fungus.SetOperator> SetFunc;
        }

        [VariableProperty(AllVariableTypes.VariableAny.Any)]
        [UnityEngine.SerializeField] public Variable variable;

        [UnityEngine.SerializeField] public AnyVariableData data;

        //needs static lookup function table for getting a function or string based on the type
        // all new variable types will need to be added here also to be supported
        public static readonly Dictionary<System.Type, TypeActions> typeActionLookup = new Dictionary<System.Type, TypeActions>()
        {
            { typeof(AnimatorVariable),
                new TypeActions( "animatorData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.animatorData.Value); },
                    (anyVar) => anyVar.data.animatorData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.animatorData.Value)) },
            { typeof(AudioSourceVariable),
                new TypeActions( "audioSourceData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.audioSourceData.Value); },
                    (anyVar) => anyVar.data.audioSourceData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.audioSourceData.Value)) },
            { typeof(BooleanVariable),
                new TypeActions( "booleanData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.booleanData.Value); },
                    (anyVar) => anyVar.data.booleanData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.booleanData.Value)) },
            { typeof(CollectionVariable),
                new TypeActions( "collectionData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.collectionData.Value); },
                    (anyVar) => anyVar.data.collectionData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.collectionData.Value)) },
            { typeof(Collider2DVariable),
                new TypeActions( "collider2DData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.collider2DData.Value); },
                    (anyVar) => anyVar.data.collider2DData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.collider2DData.Value)) },
            { typeof(ColliderVariable),
                new TypeActions( "colliderData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.colliderData.Value); },
                    (anyVar) => anyVar.data.colliderData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.colliderData.Value)) },
            { typeof(ColorVariable),
                new TypeActions( "colorData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.colorData.Value); },
                    (anyVar) => anyVar.data.colorData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.colorData.Value)) },
            { typeof(FloatVariable),
                new TypeActions( "floatData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.floatData.Value); },
                    (anyVar) => anyVar.data.floatData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.floatData.Value)) },
            { typeof(GameObjectVariable),
                new TypeActions( "gameObjectData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.gameObjectData.Value); },
                    (anyVar) => anyVar.data.gameObjectData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.gameObjectData.Value)) },
            { typeof(IntegerVariable),
                new TypeActions( "integerData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.integerData.Value); },
                    (anyVar) => anyVar.data.integerData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.integerData.Value)) },
            { typeof(MaterialVariable),
                new TypeActions( "materialData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.materialData.Value); },
                    (anyVar) => anyVar.data.materialData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.materialData.Value)) },
            { typeof(Matrix4x4Variable),
                new TypeActions( "matrix4x4Data",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.matrix4x4Data.Value); },
                    (anyVar) => anyVar.data.matrix4x4Data.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.matrix4x4Data.Value)) },
            { typeof(ObjectVariable),
                new TypeActions( "objectData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.objectData.Value); },
                    (anyVar) => anyVar.data.objectData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.objectData.Value)) },
            { typeof(QuaternionVariable),
                new TypeActions( "quaternionData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.quaternionData.Value); },
                    (anyVar) => anyVar.data.quaternionData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.quaternionData.Value)) },
            { typeof(Rigidbody2DVariable),
                new TypeActions( "rigidbody2DData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.rigidbody2DData.Value); },
                    (anyVar) => anyVar.data.rigidbody2DData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.rigidbody2DData.Value)) },
            { typeof(RigidbodyVariable),
                new TypeActions( "rigidbodyData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.rigidbodyData.Value); },
                    (anyVar) => anyVar.data.rigidbodyData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.rigidbodyData.Value)) },
            { typeof(SpriteVariable),
                new TypeActions( "spriteData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.spriteData.Value); },
                    (anyVar) => anyVar.data.spriteData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.spriteData.Value)) },
            { typeof(StringVariable),
                new TypeActions( "stringData",
                    (anyVar, compareOperator) =>
                    {
                        var subbedRHS = anyVar.variable.GetFlowchart().SubstituteVariables(anyVar.data.stringData.Value);
                        return anyVar.variable.Evaluate(compareOperator, subbedRHS); 
                    },
                    (anyVar) => anyVar.data.stringData.GetDescription(),
                    (anyVar, setOperator) =>
                    {
                        var subbedRHS = anyVar.variable.GetFlowchart().SubstituteVariables(anyVar.data.stringData.Value);
                        anyVar.variable.Apply(setOperator, subbedRHS); 
                    })},
            { typeof(TextureVariable),
                new TypeActions( "textureData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.textureData.Value); },
                    (anyVar) => anyVar.data.textureData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.textureData.Value)) },
            { typeof(TransformVariable),
                new TypeActions( "transformData",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.transformData.Value); },
                    (anyVar) => anyVar.data.transformData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.transformData.Value)) },
            { typeof(Vector2Variable),
                new TypeActions( "vector2Data",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.vector2Data.Value); },
                    (anyVar) => anyVar.data.vector2Data.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.vector2Data.Value)) },
            { typeof(Vector3Variable),
                new TypeActions( "vector3Data",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.vector3Data.Value); },
                    (anyVar) => anyVar.data.vector3Data.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.vector3Data.Value)) },
            { typeof(Vector4Variable),
                new TypeActions( "vector4Data",
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.vector4Data.Value); },
                    (anyVar) => anyVar.data.vector4Data.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.vector4Data.Value)) },
        };

        public bool HasReference(Variable variable)
        {
            return variable == this.variable || data.HasReference(variable);
        }

#if UNITY_EDITOR
        public void RefreshVariableCacheHelper(Flowchart f, ref List<Variable> referencedVariables)
        {
            if (variable is StringVariable asStringVar && asStringVar != null && !string.IsNullOrEmpty(asStringVar.Value))
                f.DetermineSubstituteVariables(asStringVar.Value, referencedVariables);

            if (!string.IsNullOrEmpty(data.stringData.Value))
                f.DetermineSubstituteVariables(data.stringData.Value, referencedVariables);
        }
#endif

        public string GetDataDescription()
        {
            TypeActions ta = null;
            if (typeActionLookup.TryGetValue(variable.GetType(), out ta))
            {
                return ta.DescFunc(this);
            }
            return "Null";
        }

        public bool Compare(CompareOperator compareOperator, ref bool compareResult)
        {
            TypeActions ta = null;
            if (typeActionLookup.TryGetValue(variable.GetType(), out ta))
            {
                compareResult = ta.CompareFunc(this, compareOperator);
                return true;
            }
            return false;
        }

        public void SetOp(SetOperator setOperator)
        {
            TypeActions ta = null;
            if (typeActionLookup.TryGetValue(variable.GetType(), out ta))
            {
                ta.SetFunc(this, setOperator);
            }
        }
    }
}