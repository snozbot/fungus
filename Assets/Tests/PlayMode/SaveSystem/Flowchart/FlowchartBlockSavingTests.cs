using System.Collections;
using NUnit.Framework;
using UnityEngine; 
using UnityEngine.TestTools; 

namespace SaveSystemTests
{
    public class FlowchartBlockSavingTests : SaveSysPlayModeTest
    {
        public override void SetUp()
        {
            base.SetUp();
            SetUpExpectedFields();
        }

        protected virtual void SetUpExpectedFields()
        {

        }

        protected int dialogueBlockIndex = 3, menuBlockIndex = 2;

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator CommandIndexesSaved()
        {
            yield return PostSetUp();

            throw new System.NotImplementedException();
        }

        protected virtual IEnumerator PostSetUp()
        {
            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator CommandTypeSaved()
        {
            yield return PostSetUp();


            throw new System.NotImplementedException();
        }
    }
}
