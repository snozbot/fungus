using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public abstract class BaseVariableProperty : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }
        public GetSet getOrSet = GetSet.Get;
    }
}