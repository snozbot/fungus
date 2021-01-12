// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Save and load Fungus Global Variable data, this is done separately from general Fungus System
    ///     save data as it requires a different priority.
    /// </summary>
    public class GlobalVariableSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string GlobalVarKey = "GlobalVarData";
        protected const int GlobalVarDataPriority = 500;

        public string DataTypeKey => GlobalVarKey;

        public int Order => GlobalVarDataPriority;

        public StringPair[] Encode()
        {
            var gvd = new GlobalVariableSaveDataItem();

            var gv = FungusManager.Instance.GlobalVariables;
            List<Variable> vars = null;
            if (gv != null)
                vars = gv.GlobalVariableFlowchart.Variables;

            if (vars != null)
            {
                foreach (var item in vars)
                {
                    if (item.IsSerializable)
                    {
                        gvd.typeStringPairs.Add(new GlobalVariableSaveDataItem.TypeStringPair()
                        {
                            typeName = item.GetType().Name,
                            stringPair = new StringPair
                            {
                                key = item.Key,
                                val = item.GetStringifiedValue()
                            }
                        });
                    }
                }
            }
            return SaveDataItemUtils.CreateSingleElement(DataTypeKey, gvd);
        }

        public bool Decode(StringPair sdi)
        {
            var gvd = JsonUtility.FromJson<GlobalVariableSaveDataItem>(sdi.val);
            if (gvd == null)
            {
                Debug.LogError("Failed to decode Global Variable save data item");
                return false;
            }

            if (gvd.typeStringPairs.Count == 0)
                return true;

            FungusManager.Instance.GlobalVariables.ClearVars();

            var allVarTypes = System.AppDomain.CurrentDomain.GetAssemblies().
                    SelectMany(x => x.GetTypes())
                    .Where(x => x.IsSubclassOf(typeof(Variable)))
                    .ToList();

            FungusManager.Instance.GlobalVariables.ClearVars();

            foreach (var item in gvd.typeStringPairs)
            {
                var foundType = allVarTypes.First(x => x.Name == item.typeName);
                var v = FungusManager.Instance.GlobalVariables.AddVariable(item.stringPair.key, foundType);
                v.RestoreFromStringifiedValue(item.stringPair.val);
            }

            return true;
        }

        public void PreDecode()
        {
        }

        public void PostDecode()
        {
        }

        /// <summary>
        /// Serializable container for encoding the variables in the GlobalVariables.
        /// </summary>
        [System.Serializable]
        public class GlobalVariableSaveDataItem
        {
            /// <summary>
            /// Variable name and json value pair.
            /// </summary>
            [System.Serializable]
            public struct TypeStringPair
            {
                public string typeName;
                public StringPair stringPair;
            }

            public List<TypeStringPair> typeStringPairs = new List<TypeStringPair>();
        }
    }
}
