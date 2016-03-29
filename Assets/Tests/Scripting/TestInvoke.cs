using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class TestInvoke : MonoBehaviour 
	{
		public Flowchart flowchart;

		public int passCount;

		public void TestCall()
		{
			passCount++;
		}

		public void TestCall(bool boolParam)
		{
			if (boolParam)
			{
				passCount++;
			}
		}

		public void TestCall(int intParam)
		{
			if (intParam == 10)
			{
				passCount++;
			}
		}

		public void TestCall(float floatParam)
		{
			if (floatParam == 5.2f)
			{
				passCount++;
			}
		}

		public void TestCall(string stringParam)
		{
			if (stringParam == "ok")
			{
				passCount++;
			}
		}

		public bool TestCall(bool boolParam, int intParam, float floatParam, string stringParam)
		{
			if (boolParam && intParam == 10 && floatParam == 5.2f && stringParam == "ok")
			{
				passCount++;
			}

			return true;
		}

		public int TestReturnInteger()
		{
			passCount++;
			return 5; 
		}

		public float TestReturnFloat()
		{
			passCount++;
			return 22.1f; 
		}

		public string TestReturnString()
		{
			passCount++;
			return "a string"; 
		}

		// Test the Call Method command
		public void TestCallMethod()
		{
			passCount++;
		}

		public void DelayedInvokeEvent()
		{
			passCount++;
		}

		public void CheckTestResult()
		{
			if (flowchart == null)
			{
				IntegrationTest.Fail("Flowchart object not selected");
				return;
			}

			// Check Fungus variables are populated with expected values
			if (flowchart.GetVariable<BooleanVariable>("BoolVar").value != true ||
                flowchart.GetVariable<IntegerVariable>("IntVar").value != 5 ||
                flowchart.GetVariable<FloatVariable>("FloatVar").value != 22.1f ||
                flowchart.GetVariable<StringVariable>("StringVar").value != "a string")
			{
				IntegrationTest.Fail("Fungus variables do not match expected values");
				return;
			}

			// Check the right number of methods were invoked successfully
			if (passCount == 11)
			{
				IntegrationTest.Pass();
			}
			else
			{
				IntegrationTest.Fail("A method did not get invoked or parameter was incorrect");
			}
		}
	}

}