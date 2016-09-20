// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using Fungus.Variables;

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
			if (flowchart.GetVariable<BooleanVariable>("BoolVar").Value != true ||
                flowchart.GetVariable<IntegerVariable>("IntVar").Value != 5 ||
                flowchart.GetVariable<FloatVariable>("FloatVar").Value != 22.1f ||
                flowchart.GetVariable<StringVariable>("StringVar").Value != "a string")
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