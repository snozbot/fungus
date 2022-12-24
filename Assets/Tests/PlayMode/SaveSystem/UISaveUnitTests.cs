using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus.LionManeSaveSys;
using DateTime = System.DateTime;
using System.IO;
using Encoding = System.Text.Encoding;

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
            testUnitAsByteArray = Encoding.UTF8.GetBytes(testUnitAsJson);

            testUnitDeserializedFromBaseJson = JsonUtility.FromJson<UISaveUnit>(testUnitAsJson);
            testUnitDeserializedFromBaseJson.OnDeserialize();
        }

        protected UISaveUnit testSaveUnit = new UISaveUnit();
        protected string testUnitAsJson;
        protected byte[] testUnitAsByteArray;
        protected UISaveUnit testUnitDeserializedFromBaseJson;

        // A Test behaves as an ordinary method
        [Test]
        public void SerializedLastWrittenDateCorrectly_BaseJSON()
        {
            bool correct = testSaveUnit.LastWritten.Equals(testUnitDeserializedFromBaseJson.LastWritten);
            Assert.IsTrue(correct);
        }

        [Test]
        public void SerializedPlaytimeCorrectly_BaseJSON()
        {
            bool correct = testSaveUnit.Playtime.Equals(testUnitDeserializedFromBaseJson.Playtime);
            Assert.IsTrue(correct);
        }

        [Test]
        public virtual void SavedToFileCorrectly_NoEncap_ByteArr()
        {
            string byteArrAsString = Encoding.UTF8.GetString(testUnitAsByteArray);
            string path = Path.Combine(savePath, uiSaveUnitOnlyFile);
            File.WriteAllText(path, byteArrAsString);

            string readFromFile = File.ReadAllText(path);

            UISaveUnit deserializedFromReadJSON = JsonUtility.FromJson<UISaveUnit>(readFromFile);
            deserializedFromReadJSON.OnDeserialize();

            bool correct = deserializedFromReadJSON.Equals(testSaveUnit);
            Assert.IsTrue(correct);

        }

        protected string savePath = Path.Combine(Application.dataPath, "Tests",
            "Resources", "SaveFiles");

        protected string uiSaveUnitOnlyFile = "UIUnitOnly.sav";

    }
}
