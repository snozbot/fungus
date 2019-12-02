using System.Collections.Generic;
using UnityEngine;

namespace Fungus.SaveSystem
{
    /// <summary>
    /// Generic serializable container for variable types.
    /// </summary>
    [System.Serializable]
    public class Var<T>
    {
        [SerializeField] protected string typeName;
        [SerializeField] protected string key;
        [SerializeField] protected T value;
        protected System.Type type;

        public string TypeName              { get { return typeName; } protected set { typeName = value; } }
        public string Key                   { get { return key; } set { key = value; } }
        public T Value                      { get { return value; } set { this.value = value; } }
        public System.Type Type             
        {
            get 
            { 
                if (type == null) // This property is being accessed after this Var instance got deserialized.
                {
                    // So, we'd best reinitialize the value before returning it, so we don't return null.
                    type =                  System.Type.GetType(typeName);
                }

                return type; 
            } 
            protected set { type = value; } 
        }
        
        public Var()
        {
            if (type != null)
                typeName =                  typeof(T).FullName;
        }

        
    }

    [System.Serializable]
    public class IntVar: Var<int>           {}

    [System.Serializable]
    public class FloatVar: Var<float>       {}
    
    [System.Serializable]
    public class BoolVar: Var<bool>         {}

    [System.Serializable]
    public class StringVar: Var<string>     {}

    [System.Serializable]
    public class ColorVar: Var<Color>       {}

    [System.Serializable]
    public class Vec2Var: Var<Vector2>   {}

    [System.Serializable]
    public class Vec3Var: Var<Vector3>   {}

    [System.Serializable]
    public class FlowchartVariables
    {
        #region Var lists
        [SerializeField] protected List<StringVar> strings =            new List<StringVar>();
        [SerializeField] protected List<IntVar> ints =                  new List<IntVar>();
        [SerializeField] protected List<FloatVar> floats =              new List<FloatVar>();
        [SerializeField] protected List<BoolVar> bools =                new List<BoolVar>();
        [SerializeField] protected List<ColorVar> colors =              new List<ColorVar>();
        [SerializeField] protected List<Vec2Var> vec2s =                new List<Vec2Var>();
        [SerializeField] protected List<Vec3Var> vec3s =             new List<Vec3Var>();
        #endregion

        #region Allowing access to those lists
        public List<StringVar> Strings                  
        { 
            get                                                         { return strings; } 
            set                                                         { strings = value; } 
        }

        public List<IntVar> Ints                  
        { 
            get                                                         { return ints; } 
            set                                                         { ints = value; } 
        }

        public List<FloatVar> Floats                  
        { 
            get                                                         { return floats; } 
            set                                                         { floats = value; } 
        }

        public List<BoolVar> Bools                  
        { 
            get                                                         { return bools; } 
            set                                                         { bools = value; } 
        }

        public List<ColorVar> Colors                  
        { 
            get                                                         { return colors; } 
            set                                                         { colors = value; } 
        }

        public List<Vec2Var> Vec2s                  
        { 
            get                                                         { return vec2s; } 
            set                                                         { vec2s = value; } 
        }

        public List<Vec3Var> Vec3s                  
        { 
            get                                                         { return vec3s; } 
            set                                                         { vec3s = value; } 
        }

        public virtual void Clear()
        {
            Strings.Clear();
            Ints.Clear();
            Floats.Clear();
            Bools.Clear();
            Colors.Clear();
            Vec2s.Clear();
            Vec3s.Clear();
        }

        #endregion
    }

}
