using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;
using Fungus;

namespace Fungus
{

    public class RegisterFungusTypes
    {
        /// <summary>
        /// Extension for FungusScript that registers all types used by Fungus.
        /// </summary>
        public static void Register()
        {
            UserData.RegisterType<Flowchart>();
            UserData.RegisterType<Block>();
            UserData.RegisterType<Variable>();
            UserData.RegisterType<BooleanVariable>();
            UserData.RegisterType<ColorVariable>();
            UserData.RegisterType<FloatVariable>();
            UserData.RegisterType<GameObjectVariable>();
            UserData.RegisterType<IntegerVariable>();
            UserData.RegisterType<MaterialVariable>();
            UserData.RegisterType<ObjectVariable>();
            UserData.RegisterType<SpriteVariable>();
            UserData.RegisterType<StringVariable>();
            UserData.RegisterType<TextureVariable>();
            UserData.RegisterType<Vector2Variable>();
            UserData.RegisterType<Vector3Variable>();
            UserData.RegisterType<AnimatorVariable>();
            UserData.RegisterType<AudioSourceVariable>();
            UserData.RegisterType<TransformVariable>();

            // Extension types
            UserData.RegisterExtensionType(typeof(FungusScriptExtensions));
        }
    }

}