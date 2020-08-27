// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections.Generic;
using System.Linq;

namespace Fungus
{
    /// <summary>
    /// Serializable container for encoding the variables in the GlobalVariables.
    /// </summary>
    [System.Serializable]
    public class GlobalVariableData
    {
        /// <summary>
        /// Variable name and json value pair.
        /// </summary>
        [System.Serializable]
        public class TypeStringPair : FlowchartData.StringPair
        {
            public string typeName;
        }

        public List<TypeStringPair> typeStringPairs = new List<TypeStringPair>();

        /// <summary>
        /// Stores all serialisable global variables, with their type as well as their key and value.
        /// </summary>
        public static GlobalVariableData Encode()
        {
            var gvd = new GlobalVariableData();

            foreach (var item in FungusManager.Instance.GlobalVariables.GlobalVariableFlowchart.Variables)
            {
                if (item.IsSerializable)
                {
                    gvd.typeStringPairs.Add(new TypeStringPair()
                    {
                        key = item.Key,
                        val = item.GetStringifiedValue(),
                        typeName = item.GetType().Name
                    });
                }
            }

            return gvd;
        }

        /// <summary>
        /// Recreate global vars from previous serialise and restore value.
        /// </summary>
        public void Decode()
        {
            var allVarTypes = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(Variable)))
                .ToList();

            foreach (var item in typeStringPairs)
            {
                var foundType = allVarTypes.First(x => x.Name == item.typeName);
                var v = FungusManager.Instance.GlobalVariables.AddVariable(item.key, foundType);
                v.RestoreFromStringifiedValue(item.val);
            }
        }
    }
}