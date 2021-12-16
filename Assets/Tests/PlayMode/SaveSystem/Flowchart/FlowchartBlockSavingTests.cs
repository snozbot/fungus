using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine; 
using UnityEngine.TestTools;
using Fungus.LionManeSaveSys;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartBlockSavingTests : SaveSysPlayModeTest
    {
        public override void SetUp()
        {
            base.SetUp();
            blockSaver = GameObject.FindObjectOfType<BlockSaver>();
            FindBlockToSave();
            SetUpExpectedFields();
        }

        protected virtual void FindBlockToSave()
        {
            var allBlocks = GameObject.FindObjectsOfType<Block>();
            foreach (var blockFound in allBlocks)
            {
                if (blockFound.BlockName == nameOfBlockToSave)
                {
                    blockToSave = blockFound;
                    break;
                }
            }
        }

        protected BlockSaver blockSaver;
        protected string nameOfBlockToSave = "Dialogue";
        protected Block blockToSave;

        protected virtual void SetUpExpectedFields()
        {
            
        }
        
        protected int dialogueBlockIndex = 3, menuBlockIndex = 2;
        protected int[] sayCommandIndexes = new int[] // within the Dialogue block
        {
            3, 4
        };

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator CommandIndexesSaved()
        {
            yield return PostSetUp();

            List<Say> sayCommands = new List<Say>(blockToSave.GetComponents<Say>());
            sayCommands.Sort(SortByIndex);


            throw new System.NotImplementedException();
        }

        protected virtual int SortByIndex(Say firstSay, Say secondSay)
        {
            if (firstSay.CommandIndex > secondSay.CommandIndex)
                return 1;
            else
                return -1;
        }

        protected virtual IEnumerator PostSetUp()
        {
            yield return new WaitForSeconds(1f);
            SaveTheTestBlock();

        }

        protected virtual void SaveTheTestBlock()
        {
            blockState = blockSaver.CreateSaveFrom(blockToSave);
            
        }

        protected BlockSaveUnit blockState;

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator CommandTypeSaved()
        {
            yield return PostSetUp();

            throw new System.NotImplementedException();
        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator ExecutionCountSaved()
        {
            throw new System.NotImplementedException();
        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator SayCommandsSaved()
        {
            throw new System.NotImplementedException();
        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator ItemIDsSaved()
        {
            yield return PostSetUp();

            throw new System.NotImplementedException();
        }
    }
}
