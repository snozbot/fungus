using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    [System.Serializable]
    public class StringVar
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class IntVar
    {
        public string key;
        public int value;
    }

    [System.Serializable]
    public class FloatVar
    {
        public string key;
        public float value;
    }

    [System.Serializable]
    public class BoolVar
    {
        public string key;
        public bool value;
    }

    [System.Serializable]
    public class SavePointData
    {
        public string sceneName;
        public string flowchartName;
        public string saveKey;

        public List<StringVar> stringVars = new List<StringVar>();
        public List<IntVar> intVars = new List<IntVar>();
        public List<FloatVar> floatVars = new List<FloatVar>();
        public List<BoolVar> boolVars = new List<BoolVar>();
    }
}