using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public class TransformVarSaver : VarSaver
    {
        protected override ContentType SetContentAs => ContentType.jsonString;

        protected override string GetJsonStringOfValueIn(Variable input)
        {
            // We can't simply json-stringify a Transform and expect to be able to use the result
            // to properly load the transform var, so...
            Transform trans = (Transform) input.GetValue();
            TransformState state = new TransformState(trans);
            bool weWantPrettyPrint = true;
            string stringified = JsonUtility.ToJson(state, weWantPrettyPrint);
            return stringified;
        }
    }
}