using System;
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
            typeof(ColorVariable),
            typeof(FloatVariable),
            typeof(GameObjectVariable),
            typeof(IntegerVariable),
            typeof(MaterialVariable),
            typeof(ObjectVariable),
            typeof(Rigidbody2DVariable),
            typeof(SpriteVariable),
            typeof(StringVariable),
            typeof(TextureVariable),
            typeof(TransformVariable),
            typeof(Vector2Variable),
            typeof(Vector3Variable),
        };
    }

    [System.Serializable]
    public struct AnyVariableData
    {
        public static readonly string[] PropertyNameByTypeIndex = new string[]
        {
            "animatorData",
            "audioSourceData",
            "booleanData",
            "collectionData",
            "colorData",
            "floatData",
            "gameObjectData",
            "integerData",
            "materialData",
            "objectData",
            "rigidbody2DData",
            "spriteData",
            "stringData",
            "textureData",
            "transformData",
            "vector2Data",
            "vector3Data",
        };

        public AnimatorData     animatorData;
        public AudioSourceData  audioSourceData;
        public BooleanData      booleanData;
        public CollectionData   collectionData;
        public ColorData        colorData;
        public FloatData        floatData;
        public GameObjectData   gameObjectData;
        public IntegerData      integerData;
        public MaterialData     materialData;
        public ObjectData       objectData;
        public Rigidbody2DData  rigidbody2DData;
        public SpriteData       spriteData;
        public StringData       stringData;
        public TextureData      textureData;
        public TransformData    transformData;
        public Vector2Data      vector2Data;
        public Vector3Data      vector3Data;
    }

    [System.Serializable]
    public class AnyVaraibleAndDataPair
    {
        public class TypeActions
        {
            public TypeActions(System.Func<AnyVaraibleAndDataPair, Fungus.CompareOperator, bool> comparer,
                                System.Func<AnyVaraibleAndDataPair, string> desccription,
                                System.Action<AnyVaraibleAndDataPair, Fungus.SetOperator> set)
            {
                CompareFunc = comparer;
                DescFunc = desccription;
                SetFunc = set;
            }

            public System.Func<AnyVaraibleAndDataPair, Fungus.CompareOperator, bool> CompareFunc;
            public System.Func<AnyVaraibleAndDataPair, string> DescFunc;
            public System.Action<AnyVaraibleAndDataPair, Fungus.SetOperator> SetFunc;
        }


        [VariableProperty(VariableInfo.VariableAny.Any)]
        [UnityEngine.SerializeField] public Variable variable;

        [UnityEngine.SerializeField] public AnyVariableData data;

        //needs static lookup function table for 
        public static readonly Dictionary<System.Type, TypeActions> typeActionLookup = new Dictionary<System.Type, TypeActions>()
        {
            { typeof(AnimatorVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.animatorData.Value); }, 
                    (anyVar) => anyVar.data.animatorData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.animatorData.Value)) },
            { typeof(AudioSourceVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.audioSourceData.Value); },
                    (anyVar) => anyVar.data.audioSourceData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.audioSourceData.Value)) },
            { typeof(BooleanVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.booleanData.Value); },
                    (anyVar) => anyVar.data.booleanData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.booleanData.Value)) },
            { typeof(CollectionVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.collectionData.Value); },
                    (anyVar) => anyVar.data.collectionData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.collectionData.Value)) },
            { typeof(ColorVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.colorData.Value); },
                    (anyVar) => anyVar.data.colorData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.colorData.Value)) },
            { typeof(FloatVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.floatData.Value); },
                    (anyVar) => anyVar.data.floatData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.floatData.Value)) },
            { typeof(GameObjectVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.gameObjectData.Value); },
                    (anyVar) => anyVar.data.gameObjectData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.gameObjectData.Value)) },
            { typeof(IntegerVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.integerData.Value); },
                    (anyVar) => anyVar.data.integerData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.integerData.Value)) },
            { typeof(MaterialVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.materialData.Value); },
                    (anyVar) => anyVar.data.materialData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.materialData.Value)) },
            { typeof(ObjectVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.objectData.Value); },
                    (anyVar) => anyVar.data.objectData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.objectData.Value)) },
            { typeof(Rigidbody2DVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.rigidbody2DData.Value); },
                    (anyVar) => anyVar.data.rigidbody2DData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.rigidbody2DData.Value)) },
            { typeof(SpriteVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.spriteData.Value); },
                    (anyVar) => anyVar.data.spriteData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.spriteData.Value)) },
            { typeof(StringVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.stringData.Value); },
                    (anyVar) => anyVar.data.stringData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.stringData.Value)) },
            { typeof(TextureVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.textureData.Value); },
                    (anyVar) => anyVar.data.textureData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.textureData.Value)) },
            { typeof(TransformVariable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.transformData.Value); },
                    (anyVar) => anyVar.data.transformData.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.transformData.Value)) },
            { typeof(Vector2Variable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.vector2Data.Value); },
                    (anyVar) => anyVar.data.vector2Data.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.vector2Data.Value)) },
            { typeof(Vector3Variable),
                new TypeActions( (anyVar, compareOperator) => {return anyVar.variable.Evaluate(compareOperator, anyVar.data.vector3Data.Value); },
                    (anyVar) => anyVar.data.vector3Data.GetDescription(),
                    (anyVar, setOperator) => anyVar.variable.Apply(setOperator, anyVar.data.vector3Data.Value)) },
        };

        public bool HasReference(Variable var)
        {
            return var == variable ||
                 data.animatorData.animatorRef == variable ||
                 data.audioSourceData.audioSourceRef == variable ||
                 data.booleanData.booleanRef == variable ||
                 data.collectionData.collectionRef == variable ||
                 data.colorData.colorRef == variable ||
                 data.floatData.floatRef == variable ||
                 data.gameObjectData.gameObjectRef == variable ||
                 data.integerData.integerRef == variable ||
                 data.materialData.materialRef == variable ||
                 data.objectData.objectRef == variable ||
                 data.rigidbody2DData.rigidbody2DRef == variable ||
                 data.spriteData.spriteRef == variable ||
                 data.stringData.stringRef == variable ||
                 data.textureData.textureRef == variable ||
                 data.transformData.transformRef == variable ||
                 data.vector2Data.vector2Ref == variable ||
                 data.vector3Data.vector3Ref == variable;
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