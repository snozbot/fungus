// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Factory class to create new instances of common POD value types used by Unity.
    /// Supports the same types as the SerializedProperty class: Color, Vector2, Vector3, Vector4, Quaternion & Rect.
    /// MoonSharp doesn't work well with these types due to internal interop issues with c#. 
    /// Use these factory methods to constuct these types instead of using the __new function call in Lua.
    /// </summary>
    public static class PODTypeFactory  
    {
        /// <summary>
        /// Returns a new Color object.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public static Color color(float r, float g, float b, float a)
        {
            return new Color(r,g,b,a);
        }

        /// <summary>
        /// Returns a new Vector2 object.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public static Vector2 vector2(float x, float y)
        {
            return new Vector2(x,y);
        }

        /// <summary>
        /// Returns a new Vector3 object.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public static Vector3 vector3(float x, float y, float z)
        {
            return new Vector3(x,y,z);
        }

        /// <summary>
        /// Returns a new Vector4 object.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="w">The w coordinate.</param>
        public static Vector4 vector4(float x, float y, float z, float w)
        {
            return new Vector4(x,y,z,w);
        }

        /// <summary>
        /// Returns a new Quaternion object representing a rotation.
        /// </summary>
        /// <param name="x">The x rotation in degrees.</param>
        /// <param name="y">The y rotation in degrees.</param>
        /// <param name="z">The z rotation in degrees.</param>
        public static Quaternion quaternion(float x, float y, float z)
        {
            return UnityEngine.Quaternion.Euler(x,y,z);
        }
            
        /// <summary>
        /// Returns a new Rect object.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The rectangle width.</param>
        /// <param name="height">The rectangle height.</param>
        public static Rect rect(float x, float y, float width, float height)
        {
            return new Rect(x,y,width, height);
        }
    }
}