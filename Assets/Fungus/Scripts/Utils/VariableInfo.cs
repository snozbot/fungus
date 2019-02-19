using System.Collections.Generic;

namespace Fungus
{
    public static class VariableInfo
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

    [System.Serializable]
    public struct AnyVariableData
    {
        public AnimatorData     animatorData;
        public AudioSourceData  audioSourceData;
        public BooleanData      booleanData;
        public CollectionData   collectionData;
        public Collider2DData   collider2DData;
        public ColliderData     colliderData;
        public ColorData        colorData;
        public FloatData        floatData;
        public GameObjectData   gameObjectData;
        public IntegerData      integerData;
        public MaterialData     materialData;
        public Matrix4x4Data    matrix4x4Data;
        public ObjectData       objectData;
        public QuaternionData   quaternionData;
        public Rigidbody2DData  rigidbody2DData;
        public RigidbodyData    rigidbodyData;
        public SpriteData       spriteData;
        public StringData       stringData;
        public TextureData      textureData;
        public TransformData    transformData;
        public Vector2Data      vector2Data;
        public Vector3Data      vector3Data;
        public Vector4Data      vector4Data;
    }

    [System.Serializable]
    public class AnyVaraibleAndDataPair
    {
        public class TypeActions
        {
            public TypeActions(string dataPropName,
                               System.Func<AnyVaraibleAndDataPair, Fungus.CompareOperator, bool> comparer,
                               System.Func<AnyVaraibleAndDataPair, string> desccription,
                               System.Action<AnyVaraibleAndDataPair, Fungus.SetOperator> set
#if UNITY_EDITOR
                               ,System.Action<UnityEngine.Rect, UnityEditor.SerializedProperty> customDrawFunc = null
#endif
                )
            {
                DataPropName = dataPropName;
                CompareFunc = comparer;
                DescFunc = desccription;
                SetFunc = set;
#if UNITY_EDITOR
                CustomDraw = customDrawFunc;
#endif
            }

            public string DataPropName { get; set; }
            public System.Func<AnyVaraibleAndDataPair, Fungus.CompareOperator, bool> CompareFunc;
            public System.Func<AnyVaraibleAndDataPair, string> DescFunc;
            public System.Action<AnyVaraibleAndDataPair, Fungus.SetOperator> SetFunc;
#if UNITY_EDITOR
            public System.Action<UnityEngine.Rect, UnityEditor.SerializedProperty> CustomDraw;
#endif
        }


        [VariableProperty(VariableInfo.VariableAny.Any)]
        [UnityEngine.SerializeField] public Variable variable;

        [UnityEngine.SerializeField] public AnyVariableData data;

        //needs static lookup function table for 
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
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.quaternionData.Value)
#if UNITY_EDITOR
                    ,(rect, valueProp) => {valueProp.quaternionValue = UnityEngine. Quaternion.Euler(UnityEditor.EditorGUI.Vector3Field(rect, new UnityEngine.GUIContent(""), valueProp.quaternionValue.eulerAngles)); }
#endif
                    ) },
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
                    (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.stringData.Value); },
                    (anyVar) => anyVar.data.stringData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.stringData.Value)) },
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
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.vector4Data.Value)
#if UNITY_EDITOR
                    ,(rect, valueProp) => {valueProp.vector4Value = UnityEditor.EditorGUI.Vector4Field(rect, new UnityEngine.GUIContent(""), valueProp.vector4Value); }
#endif
                    ) },
        };

        public bool HasReference(Variable var)
        {
            return var == variable ||
                data.animatorData.animatorRef == var ||
            data.audioSourceData.audioSourceRef == var ||
            data.booleanData.booleanRef == var ||
            data.collectionData.collectionRef == var ||
            data.collider2DData.collider2DRef == var ||
            data.colliderData.colliderRef == var ||
            data.colorData.colorRef == var ||
            data.floatData.floatRef == var ||
            data.gameObjectData.gameObjectRef == var ||
            data.integerData.integerRef == var ||
            data.materialData.materialRef == var ||
            data.matrix4x4Data.matrix4x4Ref == var ||
            data.objectData.objectRef == var ||
            data.quaternionData.quaternionRef == var ||
            data.rigidbody2DData.rigidbody2DRef == var ||
            data.rigidbodyData.rigidbodyRef == var ||
            data.spriteData.spriteRef == var ||
            data.stringData.stringRef == var ||
            data.textureData.textureRef == var ||
            data.transformData.transformRef == var ||
            data.vector2Data.vector2Ref == var ||
            data.vector3Data.vector3Ref == var ||
            data.vector4Data.vector4Ref == var;
        }

        public string GetDataDescription()
        {
            TypeActions ta = null;
            if (typeActionLookup.TryGetValue(variable.GetType(), out ta))
            {
                return ta.DescFunc(this);
            }
            return string.Empty;
        }

        public bool Compare(CompareOperator compareOperator, ref bool compareResult)
        {
            TypeActions ta = null;
            if(typeActionLookup.TryGetValue(variable.GetType(), out ta))
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