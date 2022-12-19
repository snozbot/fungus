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
        public override void SetUp()
        {
            base.SetUp();
            PrepareTestSaveUnit();
        }

        protected virtual void PrepareTestSaveUnit()
        {
            testSaveUnit.LastWritten = DateTime.Now;
            testSaveUnit.Playtime = new System.TimeSpan(12, 58, 29);

            testUnitAsJson = JsonUtility.ToJson(testSaveUnit);

            testUnitDeserialized = JsonUtility.FromJson<UISaveUnit>(testUnitAsJson);
            testUnitDeserialized.OnDeserialize();
        }

        protected UISaveUnit testSaveUnit = new UISaveUnit();
        protected string testUnitAsJson;
        protected UISaveUnit testUnitDeserialized;

        // A Test behaves as an ordinary method
        [Test]
        public void SerializedLastWrittenDateCorrectly()
        {
            bool correct = testSaveUnit.LastWritten.Equals(testUnitDeserialized.LastWritten);
            Assert.IsTrue(correct);
        }

        [Test]
        public void SerializedPlaytimeCorrectly()
        {
            bool correct = testSaveUnit.Playtime.Equals(testUnitDeserialized.Playtime);
            Assert.IsTrue(correct);
        }

    }
}
