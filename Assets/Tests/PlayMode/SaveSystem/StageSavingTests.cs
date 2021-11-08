using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;
using UnityEngine.UI;

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
            RegisterCharacters();
        }

        protected virtual void GetPortraitsPrepped()
        {
            Flowchart prep = GameObject.Find("PrepPortraits").GetComponent<Flowchart>();
            prep.ExecuteBlock("Execute");
        }

        protected virtual void RegisterStageAndPositions()
        {
            stageForPortraits = GameObject.Find("TestStage").GetComponent<Stage>();
            onTheRight = GameObject.Find("TestRight").transform.name;
            onTheLeft = GameObject.Find("TestLeft").transform.name;
        }

        protected Stage stageForPortraits;
        protected string onTheRight, onTheLeft;

        protected virtual void RegisterCharacters()
        {
            sherlock = GameObject.Find("Sherlock Holmes").GetComponent<Character>();
            watson = GameObject.Find("John Watson").GetComponent<Character>();
        }

        protected Character sherlock, watson;

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
            DimAsNeeded();
            savedPortraitStates = GetStatesOfAllPortraits();
            sherlockSaveState = GetStateFor(sherlockName);
            watsonSaveState = GetStateFor(watsonName);
            dudeSaveState = GetStateFor(dudeName);
            yield return new WaitForSeconds(1f); // Need to wait a little more for the dim effect
        }

        protected virtual IEnumerator WaitForPortraitPrep()
        {
            yield return new WaitForSeconds(portraitPrepTime);
        }

        protected float portraitPrepTime = 1.5f;

        protected virtual void DimAsNeeded()
        {
            stageForPortraits.SetDimmed(watson, true);
        }

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

        protected PortraitSaveState sherlockSaveState, watsonSaveState, dudeSaveState;

        protected virtual bool SherlockSavedAsOnTheRight()
        {
            return sherlockSaveState.PositionName == onTheRight;
        }

        protected string sherlockName = "Sherlock Holmes";

        protected virtual PortraitSaveState GetStateFor(string charName)
        {
            foreach (var savedState in savedPortraitStates)
                if (savedState.CharacterName == charName)
                    return savedState;

            return PortraitSaveState.Null;
        }

        protected virtual bool WatsonSavedAsOnTheLeft()
        {
            return watsonSaveState.PositionName == onTheLeft;
        }

        protected string watsonName = "John Watson";
        protected string dudeName = "Dude";

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

        [UnityTest]
        public virtual IEnumerator PortraitsStageNamesSaved()
        {
            yield return PostSetUp();
            
            bool foundStateWithWrongStageName = false;
            
            IList<PortraitSaveState> onScreenPortraitStates = new List<PortraitSaveState>()
            { 
                sherlockSaveState, 
                watsonSaveState 
            };
            // Char states with onScreen set to false don't even have their portraits existing in the
            // inspector, hence why we need to check only sherlock's and dude's here

            foreach (var savedState in onScreenPortraitStates) 
                if (savedState.StageName != testStageName)
                {
                    foundStateWithWrongStageName = true;
                    break;
                }

            bool thingsWereSavedProperly = !foundStateWithWrongStageName;
            Assert.IsTrue(thingsWereSavedProperly);

        }

        protected string testStageName = "TestStage";

        [UnityTest]
        public virtual IEnumerator PortraitsHaveCorrectCharacters()
        {
            yield return PostSetUp();

            bool onlyThreeStates = savedPortraitStates.Count == 3;
            bool oneForSherlock = !GetStateFor(sherlockName).Equals(PortraitSaveState.Null);
            bool oneForWatson = !GetStateFor(watsonName).Equals(PortraitSaveState.Null);

            bool thingsWentWell = onlyThreeStates && oneForSherlock && oneForWatson;
            Assert.IsTrue(thingsWentWell);
        }

        [UnityTest]
        public virtual IEnumerator PortraitDimStatesSaved()
        {
            yield return PostSetUp();

            bool sherlockNotDimmed = sherlockSaveState.Dimmed == false;
            bool watsonDimmed = watsonSaveState.Dimmed == true;

            bool savedCorrectly = sherlockNotDimmed && watsonDimmed;

            Assert.IsTrue(savedCorrectly);
        }

        [UnityTest]
        public virtual IEnumerator FacingDirectionsSaved()
        {
            yield return PostSetUp();

            bool sherlockFacingLeft = sherlockSaveState.FacingDirection == FacingDirection.Left;
            bool watsonFacingRight = watsonSaveState.FacingDirection == FacingDirection.Right;
            bool dudeNotFacingAtAll = dudeSaveState.FacingDirection == FacingDirection.None;

            bool thingsAreAsIntended = sherlockFacingLeft && watsonFacingRight;
            Assert.IsTrue(thingsAreAsIntended);
        }

        [UnityTest]
        public virtual IEnumerator PortraitIndexesSaved()
        {
            yield return PostSetUp();

            Image sherlockPortrait = sherlock.State.portraitImage;
            int sherlockPortraitIndex = sherlock.State.allPortraits.IndexOf(sherlockPortrait);
            bool sherlockCorrectIndex = sherlockSaveState.PortraitIndex == sherlockPortraitIndex;

            Image watsonPortrait = watson.State.portraitImage;
            int watsonPortraitIndex = watson.State.allPortraits.IndexOf(watsonPortrait);
            bool watsonCorrectIndex = watsonSaveState.PortraitIndex == watsonPortraitIndex;

            bool correctlySaved = sherlockCorrectIndex && watsonCorrectIndex;
            Assert.IsTrue(correctlySaved);

        }

        [UnityTest]
        public virtual IEnumerator OnScreenStatesSaved()
        {
            yield return PostSetUp();

            bool sherlockOnScreen = sherlockSaveState.OnScreen == true;
            bool watsonOnScreen = watsonSaveState.OnScreen == true;
            bool dudeNotOnScreen = dudeSaveState.OnScreen == false;

            bool savedProperly = sherlockOnScreen && watsonOnScreen && dudeNotOnScreen;
            Assert.IsTrue(savedProperly);
        }

        [UnityTest]
        public virtual IEnumerator PortraitNamesSaved()
        {
            yield return PostSetUp();
            bool sherlockCorrectPortraitName = sherlockSaveState.PortraitName == sherlockPleased;
            bool watsonCorrectPortraitName = watsonSaveState.PortraitName == watsonSuspicious;
            bool dudeCorrectPortraitName = dudeSaveState.PortraitName == dudeNull;

            bool savedCorrectly = sherlockCorrectPortraitName && watsonCorrectPortraitName && dudeCorrectPortraitName;
            Assert.IsTrue(savedCorrectly);

        }

        protected string sherlockPleased = "pleased", watsonSuspicious = "suspicious", dudeNull = "Null";

    }
}
