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
            RegisterStageAndPositions();
        }

        protected virtual void GetPortraitsPrepped()
        {
            Flowchart prep = GameObject.Find("PrepPortraits").GetComponent<Flowchart>();
            prep.ExecuteBlock("Execute");
        }

        protected virtual void RegisterStageAndPositions()
        {
            stageForPortraits = GameObject.Find("TestStage").GetComponent<Stage>();
            onTheRight = GameObject.Find("TestRight").transform.position;
            onTheLeft = GameObject.Find("TestLeft").transform.position;
        }

        protected Stage stageForPortraits;
        protected Vector3 onTheRight, onTheLeft;

        [UnityTest]
        public virtual IEnumerator PortraitPositionsSaved()
        {
            yield return PostSetUp();

            bool yesTheyWereSaved = SherlockSavedAsOnTheRight() && WatsonSavedAsOnTheLeft();
            Assert.IsTrue(yesTheyWereSaved);
        }

        /// <summary>
        /// Every test will need this so they can be sure that the portraits are truly ready
        /// </summary>
        /// <returns></returns>
        protected IEnumerator PostSetUp()
        {
            yield return WaitForPortraitPrep();
            savedPortraitStates = GetStatesOfAllPortraits();
            yield return null;
        }

        protected virtual IEnumerator WaitForPortraitPrep()
        {
            yield return new WaitForSeconds(portraitPrepTime);
        }

        protected float portraitPrepTime = 1.5f;

        protected virtual IList<PortraitSaveState> GetStatesOfAllPortraits()
        {
            IList<Character> allChars = GameObject.FindObjectsOfType<Character>();
            IList<PortraitSaveState> states = new List<PortraitSaveState>();

            foreach (var character in allChars)
            {
                PortraitSaveState saveState = PortraitSaveState.From(character);
                states.Add(saveState);
            }

            return states;
        }

        protected IList<PortraitSaveState> savedPortraitStates = new List<PortraitSaveState>();

        protected virtual bool SherlockSavedAsOnTheRight()
        {
            var sherlockState = GetStateFor("Sherlock Holmes");
            return sherlockState.Position == onTheRight;
        }

        protected virtual PortraitSaveState GetStateFor(string charName)
        {
            foreach (var savedState in savedPortraitStates)
                if (savedState.CharacterName == charName)
                    return savedState;

            throw new System.InvalidOperationException("There is no state registered with " + charName);
        }

        protected virtual bool WatsonSavedAsOnTheLeft()
        {
            var watsonState = GetStateFor("John Watson");
            return watsonState.Position == onTheLeft;
        }


        protected virtual bool StateListsAreTheSame(IList<PortraitSaveState> firstStates, IList<PortraitSaveState> secondStates)
        {
            bool differentContentAmounts = firstStates.Count != secondStates.Count;
            if (differentContentAmounts)
                return false;

            int statesToCheck = firstStates.Count;
            for (int i = 0; i < statesToCheck; i++)
            {
                var firstStateEl = firstStates[i];
                var secondStateEl = secondStates[i];

                bool theyAreTheSame = firstStateEl.Equals(secondStateEl);
                if (!theyAreTheSame)
                    return false;
            }

            return true;
        }
        

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
