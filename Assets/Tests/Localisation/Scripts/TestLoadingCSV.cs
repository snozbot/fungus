using UnityEngine;
using System.Collections;
using Ideafixxxer.CsvParser;

namespace Fungus
{

	public class TestLoadingCSV : MonoBehaviour 
	{
		public TextAsset csvFileWindows;
		public TextAsset csvFileMac;

		void Start () 
		{
			if (!ParseCSV(csvFileWindows.text))
			{
				IntegrationTest.Fail("Failed to parse CSV file with Windows line endings");
			}

			if (!ParseCSV(csvFileMac.text))
			{
				IntegrationTest.Fail("Failed to parse CSV file with Mac line endings");
			}

			IntegrationTest.Pass();
		}

		bool ParseCSV(string text)
		{
			Debug.Log (text);

			CsvParser csvParser = new CsvParser();
			string[][] csvTable = csvParser.Parse(text);

			bool passed = true;
			passed &= (csvTable[1][0] == "SAY.LocalizationDemo.12.");
			passed &= (csvTable[1][1] == "");
			passed &= (csvTable[1][2] == "This text is in English");
			passed &= (csvTable[1][3] == "Este texto está en español");
			passed &= (csvTable[1][4] == "Ce texte est en français");

			return passed;
		}
	}

}