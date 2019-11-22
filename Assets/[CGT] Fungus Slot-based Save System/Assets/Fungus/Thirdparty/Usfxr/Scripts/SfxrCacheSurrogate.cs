using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("")]
public class SfxrCacheSurrogate : MonoBehaviour {

	/**
	 * usfxr
	 *
	 * Copyright 2013 Zeh Fernando
	 *
	 * Licensed under the Apache License, Version 2.0 (the "License");
	 * you may not use this file except in compliance with the License.
	 * You may obtain a copy of the License at
	 *
	 * 	http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 *
	 */

	/**
	 * SfxrCacheSurrogate
	 * This is the (internal) behavior script responsible for calling Coroutines for asynchronous audio generation
	 *
	 * @author Zeh Fernando
	 */

	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void CacheSound(SfxrSynth __synth, Action __callback) {
		StartCoroutine(CacheSoundAsynchronously(__synth, __callback));
	}

	private IEnumerator CacheSoundAsynchronously(SfxrSynth __synth, Action __callback) {
		yield return null;
		__synth.CacheSound(null, true);
		__callback();
		UnityEngine.Object.Destroy(gameObject);
	}

	public void CacheMutations(SfxrSynth __synth, uint __mutationsNum, float __mutationAmount, Action __callback) {
		StartCoroutine(CacheMutationsAsynchronously(__synth, __mutationsNum, __mutationAmount, __callback));
	}

	private IEnumerator CacheMutationsAsynchronously(SfxrSynth __synth, uint __mutationsNum, float __mutationAmount, Action __callback) {
		yield return null;
		__synth.CacheMutations(__mutationsNum, __mutationAmount, null, true);
		__callback();
		UnityEngine.Object.Destroy(gameObject);
	}
}
