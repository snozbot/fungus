//#define PERFTEST        //For testing performance of parse/stringify.  Turn on editor profiling to see how we're doing

using UnityEngine;
using UnityEditor;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.Networking;
#endif

/*
Copyright (c) 2010-2019 Matt Schoen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

public class JSONChecker : EditorWindow {
	string JSON = @"{
	""TestObject"": {
		""SomeText"": ""Blah"",
		""SomeObject"": {
			""SomeNumber"": 42,
			""SomeFloat"": 13.37,
			""SomeBool"": true,
			""SomeNull"": null
		},

		""SomeEmptyObject"": { },
		""SomeEmptyArray"": [ ],
		""EmbeddedObject"": ""{\""field\"":\""Value with \\\""escaped quotes\\\""\""}""
	}
}";	  //dat string literal...
	string URL = "";
	JSONObject j;
	[MenuItem("Window/JSONChecker")]
	static void Init() {
		GetWindow(typeof(JSONChecker));
	}
	void OnGUI() {
		JSON = EditorGUILayout.TextArea(JSON);
		GUI.enabled = !string.IsNullOrEmpty(JSON);
		if(GUILayout.Button("Check JSON")) {
#if PERFTEST
            Profiler.BeginSample("JSONParse");
			j = JSONObject.Create(JSON);
            Profiler.EndSample();
            Profiler.BeginSample("JSONStringify");
            j.ToString(true);
            Profiler.EndSample();
#else
			j = JSONObject.Create(JSON);
#endif
			Debug.Log(j.ToString(true));
		}
		EditorGUILayout.Separator();
		URL = EditorGUILayout.TextField("URL", URL);
		if (GUILayout.Button("Get JSON")) {
			Debug.Log(URL);
#if UNITY_2017_1_OR_NEWER
			var test = new UnityWebRequest(URL);
			test.SendWebRequest();
			while (!test.isDone && !test.isNetworkError) ;
#else
			var test = new WWW(URL);
 			while (!test.isDone) ;
#endif
			if (!string.IsNullOrEmpty(test.error)) {
				Debug.Log(test.error);
			} else {
#if UNITY_2017_1_OR_NEWER
				var text = test.downloadHandler.text;
#else
				var text = test.text;
#endif
				Debug.Log(text);
				j = new JSONObject(text);
				Debug.Log(j.ToString(true));
			}
		}
		if(j) {
			//Debug.Log(System.GC.GetTotalMemory(false) + "");
			if(j.type == JSONObject.Type.NULL)
				GUILayout.Label("JSON fail:\n" + j.ToString(true));
			else
				GUILayout.Label("JSON success:\n" + j.ToString(true));

		}
	}
}
