using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class StageSavingTests : SaveSysPlayModeTest
    {
        protected override string PathToScene => "Prefabs/StageSavingTests";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            GetPortraitsPrepped();
        }

        [UnityTest]
        public IEnumerator WaitForthings()
        {
            yield return new WaitForSeconds(5f);
        }

        protected virtual void GetPortraitsPrepped()
        {
            Flowchart prep = GameObject.Find("PrepPortraits").GetComponent<Flowchart>();
            prep.ExecuteBlock("Execute");
        }

        [Test]
        [Ignore("")]
        public virtual void PortraitPositionsSaved()
        {
            // Act

            // Set up save data that records the states of all portraits

            // Assert
        }

        protected virtual IEnumerator WaitForPortraitPrep()
        {
            yield return new WaitForSeconds(portraitPrepTime);
        }

        protected float portraitPrepTime = 2f;

        protected virtual IEnumerator HavePortraitsReady()
        {
            // Execute the block that prepares the portraits
            
            throw new System.NotImplementedException();
        }

        [Test]
        [Ignore("")]
        public virtual void PortraitsHaveCorrectCharacters()
        {

        }

        [Test]
        [Ignore("")]
        public virtual void PortraitsStageNamesSaved()
        {

        }

        [Test]
        [Ignore("")]
        public virtual void PortraitsAtRightSiblingIndexes()
        {

        }
    }
}
