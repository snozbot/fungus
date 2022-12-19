using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus.LionManeSaveSys;
using DateTime = System.DateTime;

namespace SaveSystemTests
{
    public class UISaveUnitTests : SaveSysPlayModeTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SerializedCorrectly()
        {
            // Use the Assert class to test conditions
            UISaveUnit saveUnit = new UISaveUnit();
            saveUnit.LastWritten = DateTime.Now;

            string asJson = JsonUtility.ToJson(saveUnit);

            UISaveUnit deserialized = JsonUtility.FromJson<UISaveUnit>(asJson);
            deserialized.OnDeserialize();

            bool correct = saveUnit.Equals(deserialized);
            Assert.IsTrue(correct);
        }

    }
}
