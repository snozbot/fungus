// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// </summary>
    public class GlobalVariableSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string GlobalVarKey = "GlobalVarData";
        protected const int GlobalVarDataPriority = 500;

        public string DataTypeKey => GlobalVarKey;

        public int Order => GlobalVarDataPriority;

        public SaveDataItem[] Encode()
        {
            var gvd = new GlobalVariableSaveDataItem();

            foreach (var item in FungusManager.Instance.GlobalVariables.GlobalVariableFlowchart.Variables)
            {
                if (item.IsSerializable)
                {
                    gvd.typeStringPairs.Add(new GlobalVariableSaveDataItem.TypeStringPair()
                    {
                        key = item.Key,
                        val = item.GetStringifiedValue(),
                        typeName = item.GetType().Name
                    });
                }
            }

            return SaveDataItemUtility.CreateSingleElement(DataTypeKey, gvd);
        }

        public bool Decode(SaveDataItem sdi)
        {
            var gvd = JsonUtility.FromJson<GlobalVariableSaveDataItem>(sdi.Data);
            if (gvd == null)
            {
                Debug.LogError("Failed to decode Global Variable save data item");
                return false;
            }

            FungusManager.Instance.GlobalVariables.ClearVars();

            var allVarTypes = System.AppDomain.CurrentDomain.GetAssemblies().
                    SelectMany(x => x.GetTypes())
                    .Where(x => x.IsSubclassOf(typeof(Variable)))
                    .ToList();

            FungusManager.Instance.GlobalVariables.ClearVars();

            foreach (var item in gvd.typeStringPairs)
            {
                var foundType = allVarTypes.First(x => x.Name == item.typeName);
                var v = FungusManager.Instance.GlobalVariables.AddVariable(item.key, foundType);
                v.RestoreFromStringifiedValue(item.val);
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
            public class TypeStringPair : StringPair
            {
                public string typeName;
            }

            public List<TypeStringPair> typeStringPairs = new List<TypeStringPair>();
        }
    }
}