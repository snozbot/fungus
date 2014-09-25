using UnityEngine;

public class SfxrParams {

	/**
	 * SfxrSynth
	 *
	 * Copyright 2010 Thomas Vian
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
	 * SfxrParams
	 * Holds parameters used by SfxrSynth
	 *
	 * @author Zeh Fernando
	 */

	// Properties
	public bool		paramsDirty;					// Whether the parameters have been changed since last time (shouldn't used cached sound)

	private uint	_waveType				= 0;	// Shape of wave to generate (see enum WaveType)

	private float	_masterVolume			= 0.5f;	// Overall volume of the sound (0 to 1)

	private float	_attackTime				= 0.0f;	// Length of the volume envelope attack (0 to 1)
	private float	_sustainTime			= 0.0f;	// Length of the volume envelope sustain (0 to 1)
	private float	_sustainPunch			= 0.0f;	// Tilts the sustain envelope for more 'pop' (0 to 1)
	private float	_decayTime				= 0.0f;	// Length of the volume envelope decay (yes, I know it's called release) (0 to 1)

	private float	_startFrequency			= 0.0f;	// Base note of the sound (0 to 1)
	private float	_minFrequency			= 0.0f;	// If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1)

	private float	_slide					= 0.0f;	// Slides the note up or down (-1 to 1)
	private float	_deltaSlide				= 0.0f;	// Accelerates the slide (-1 to 1)

	private float	_vibratoDepth			= 0.0f;	// Strength of the vibrato effect (0 to 1)
	private float	_vibratoSpeed			= 0.0f;	// Speed of the vibrato effect (i.e. frequency) (0 to 1)

	private float	_changeAmount			= 0.0f;	// Shift in note, either up or down (-1 to 1)
	private float	_changeSpeed			= 0.0f;	// How fast the note shift happens (only happens once) (0 to 1)

	private float	_squareDuty				= 0.0f;	// Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1)
	private float	_dutySweep				= 0.0f;	// Sweeps the duty up or down (-1 to 1)

	private float	_repeatSpeed			= 0.0f;	// Speed of the note repeating - certain variables are reset each time (0 to 1)

	private float	_phaserOffset			= 0.0f;	// Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1)
	private float	_phaserSweep			= 0.0f;	// Sweeps the phase up or down (-1 to 1)

	private float	_lpFilterCutoff			= 0.0f;	// Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1)
	private float	_lpFilterCutoffSweep	= 0.0f;	// Sweeps the low-pass cutoff up or down (-1 to 1)
	private float	_lpFilterResonance		= 0.0f;	// Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1)

	private float	_hpFilterCutoff			= 0.0f;	// Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1)
	private float	_hpFilterCutoffSweep	= 0.0f;	// Sweeps the high-pass cutoff up or down (-1 to 1)

	// From BFXR
	private float	_changeRepeat			= 0.0f;	// Pitch Jump Repeat Speed: larger Values means more pitch jumps, which can be useful for arpeggiation (0 to 1)
	private float	_changeAmount2			= 0.0f;	// Shift in note, either up or down (-1 to 1)
	private float	_changeSpeed2			= 0.0f;	// How fast the note shift happens (only happens once) (0 to 1)

	private float	_compressionAmount		= 0.0f;	// Compression: pushes amplitudes together into a narrower range to make them stand out more. Very good for sound effects, where you want them to stick out against background music (0 to 1)

	private float	_overtones				= 0.0f;	// Harmonics: overlays copies of the waveform with copies and multiples of its frequency. Good for bulking out or otherwise enriching the texture of the sounds (warning: this is the number 1 cause of usfxr slowdown!) (0 to 1)
	private float	_overtoneFalloff		= 0.0f;	// Harmonics falloff: the rate at which higher overtones should decay (0 to 1)

	private float	_bitCrush				= 0.0f;	// Bit crush: resamples the audio at a lower frequency (0 to 1)
	private float	_bitCrushSweep			= 0.0f;	// Bit crush sweep: sweeps the Bit Crush filter up or down (-1 to 1)


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	/** Shape of the wave (0:square, 1:sawtooth, 2:sin, 3:noise) */
	public uint waveType {
		get { return _waveType; }
		set { _waveType = value > 8 ? 0 : value; paramsDirty = true; }
	}

	/** Overall volume of the sound (0 to 1) */
	public float masterVolume {
		get { return _masterVolume; }
		set { _masterVolume = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Length of the volume envelope attack (0 to 1) */
	public float attackTime {
		get { return _attackTime; }
		set { _attackTime = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Length of the volume envelope sustain (0 to 1) */
	public float sustainTime {
		get { return _sustainTime; }
		set { _sustainTime = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Tilts the sustain envelope for more 'pop' (0 to 1) */
	public float sustainPunch {
		get { return _sustainPunch; }
		set { _sustainPunch = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Length of the volume envelope decay (yes, I know it's called release) (0 to 1) */
	public float decayTime {
		get { return _decayTime; }
		set { _decayTime = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Base note of the sound (0 to 1) */
	public float startFrequency {
		get { return _startFrequency; }
		set { _startFrequency = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1) */
	public float minFrequency {
		get { return _minFrequency; }
		set { _minFrequency = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Slides the note up or down (-1 to 1) */
	public float slide {
		get { return _slide; }
		set { _slide = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** Accelerates the slide (-1 to 1) */
	public float deltaSlide {
		get { return _deltaSlide; }
		set { _deltaSlide = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** Strength of the vibrato effect (0 to 1) */
	public float vibratoDepth {
		get { return _vibratoDepth; }
		set { _vibratoDepth = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Speed of the vibrato effect (i.e. frequency) (0 to 1) */
	public float vibratoSpeed {
		get { return _vibratoSpeed; }
		set { _vibratoSpeed = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Shift in note, either up or down (-1 to 1) */
	public float changeAmount {
		get { return _changeAmount; }
		set { _changeAmount = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** How fast the note shift happens (only happens once) (0 to 1) */
	public float changeSpeed {
		get { return _changeSpeed; }
		set { _changeSpeed = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1) */
	public float squareDuty {
		get { return _squareDuty; }
		set { _squareDuty = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Sweeps the duty up or down (-1 to 1) */
	public float dutySweep {
		get { return _dutySweep; }
		set { _dutySweep = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** Speed of the note repeating - certain variables are reset each time (0 to 1) */
	public float repeatSpeed {
		get { return _repeatSpeed; }
		set { _repeatSpeed = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1) */
	public float phaserOffset {
		get { return _phaserOffset; }
		set { _phaserOffset = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** Sweeps the phase up or down (-1 to 1) */
	public float phaserSweep {
		get { return _phaserSweep; }
		set { _phaserSweep = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1) */
	public float lpFilterCutoff {
		get { return _lpFilterCutoff; }
		set { _lpFilterCutoff = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Sweeps the low-pass cutoff up or down (-1 to 1) */
	public float lpFilterCutoffSweep {
		get { return _lpFilterCutoffSweep; }
		set { _lpFilterCutoffSweep = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1) */
	public float lpFilterResonance {
		get { return _lpFilterResonance; }
		set { _lpFilterResonance = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1) */
	public float hpFilterCutoff {
		get { return _hpFilterCutoff; }
		set { _hpFilterCutoff = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Sweeps the high-pass cutoff up or down (-1 to 1) */
	public float hpFilterCutoffSweep {
		get { return _hpFilterCutoffSweep; }
		set { _hpFilterCutoffSweep = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	// From BFXR

	/** Pitch Jump Repeat Speed: larger Values means more pitch jumps, which can be useful for arpeggiation (0 to 1) */
	public float changeRepeat {
		get { return _changeRepeat; }
		set { _changeRepeat = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Shift in note, either up or down (-1 to 1) */
	public float changeAmount2 {
		get { return _changeAmount2; }
		set { _changeAmount2 = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}

	/** How fast the note shift happens (only happens once) (0 to 1) */
	public float changeSpeed2 {
		get { return _changeSpeed2; }
		set { _changeSpeed2 = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Pushes amplitudes together into a narrower range to make them stand out more. Very good for sound effects, where you want them to stick out against background music (0 to 1) */
	public float compressionAmount {
		get { return _compressionAmount; }
		set { _compressionAmount = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Harmonics: overlays copies of the waveform with copies and multiples of its frequency. Good for bulking out or otherwise enriching the texture of the sounds (warning: this is the number 1 cause of bfxr slowdown!) (0 to 1) */
	public float overtones {
		get { return _overtones; }
		set { _overtones = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Harmonics falloff: The rate at which higher overtones should decay (0 to 1) */
	public float overtoneFalloff {
		get { return _overtoneFalloff; }
		set { _overtoneFalloff = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Bit crush: resamples the audio at a lower frequency (0 to 1) */
	public float bitCrush {
		get { return _bitCrush; }
		set { _bitCrush = Mathf.Clamp(value, 0, 1); paramsDirty = true; }
	}

	/** Bit crush sweep: sweeps the Bit Crush filter up or down (-1 to 1) */
	public float bitCrushSweep {
		get { return _bitCrushSweep; }
		set { _bitCrushSweep = Mathf.Clamp(value, -1, 1); paramsDirty = true; }
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	// Generator methods

	/**
	 * Sets the parameters to generate a pickup/coin sound
	 */
	public void GeneratePickupCoin() {
		resetParams();

		_startFrequency = 0.4f + GetRandom() * 0.5f;

		_sustainTime = GetRandom() * 0.1f;
		_decayTime = 0.1f + GetRandom() * 0.4f;
		_sustainPunch = 0.3f + GetRandom() * 0.3f;

		if (GetRandomBool()) {
			_changeSpeed = 0.5f + GetRandom() * 0.2f;
			int cnum = (int)(GetRandom()*7f) + 1;
			int cden = cnum + (int)(GetRandom()*7f) + 2;
			_changeAmount = (float)cnum / (float)cden;
		}
	}

	/**
	 * Sets the parameters to generate a laser/shoot sound
	 */
	public void GenerateLaserShoot() {
		resetParams();

		_waveType = (uint)(GetRandom() * 3);
		if (_waveType == 2 && GetRandomBool()) _waveType = (uint)(GetRandom() * 2f);

		_startFrequency = 0.5f + GetRandom() * 0.5f;
		_minFrequency = _startFrequency - 0.2f - GetRandom() * 0.6f;
		if (_minFrequency < 0.2f) _minFrequency = 0.2f;

		_slide = -0.15f - GetRandom() * 0.2f;

		if (GetRandom() < 0.33f) {
			_startFrequency = 0.3f + GetRandom() * 0.6f;
			_minFrequency = GetRandom() * 0.1f;
			_slide = -0.35f - GetRandom() * 0.3f;
		}

		if (GetRandomBool()) {
			_squareDuty = GetRandom() * 0.5f;
			_dutySweep = GetRandom() * 0.2f;
		} else {
			_squareDuty = 0.4f + GetRandom() * 0.5f;
			_dutySweep = -GetRandom() * 0.7f;
		}

		_sustainTime = 0.1f + GetRandom() * 0.2f;
		_decayTime = GetRandom() * 0.4f;
		if (GetRandomBool()) _sustainPunch = GetRandom() * 0.3f;

		if (GetRandom() < 0.33f) {
			_phaserOffset = GetRandom() * 0.2f;
			_phaserSweep = -GetRandom() * 0.2f;
		}

		if (GetRandomBool()) _hpFilterCutoff = GetRandom() * 0.3f;
	}

	/**
	 * Sets the parameters to generate an explosion sound
	 */
	public void GenerateExplosion() {
		resetParams();

		_waveType = 3;

		if (GetRandomBool()) {
			_startFrequency = 0.1f + GetRandom() * 0.4f;
			_slide = -0.1f + GetRandom() * 0.4f;
		} else {
			_startFrequency = 0.2f + GetRandom() * 0.7f;
			_slide = -0.2f - GetRandom() * 0.2f;
		}

		_startFrequency *= _startFrequency;

		if (GetRandom() < 0.2f) _slide = 0.0f;
		if (GetRandom() < 0.33f) _repeatSpeed = 0.3f + GetRandom() * 0.5f;

		_sustainTime = 0.1f + GetRandom() * 0.3f;
		_decayTime = GetRandom() * 0.5f;
		_sustainPunch = 0.2f + GetRandom() * 0.6f;

		if (GetRandomBool()) {
			_phaserOffset = -0.3f + GetRandom() * 0.9f;
			_phaserSweep = -GetRandom() * 0.3f;
		}

		if (GetRandom() < 0.33f) {
			_changeSpeed = 0.6f + GetRandom() * 0.3f;
			_changeAmount = 0.8f - GetRandom() * 1.6f;
		}
	}

	/**
	 * Sets the parameters to generate a powerup sound
	 */
	public void GeneratePowerup() {
		resetParams();

		if (GetRandomBool()) {
			_waveType = 1;
		} else {
			_squareDuty = GetRandom() * 0.6f;
		}

		if (GetRandomBool()) {
			_startFrequency = 0.2f + GetRandom() * 0.3f;
			_slide = 0.1f + GetRandom() * 0.4f;
			_repeatSpeed = 0.4f + GetRandom() * 0.4f;
		} else {
			_startFrequency = 0.2f + GetRandom() * 0.3f;
			_slide = 0.05f + GetRandom() * 0.2f;

			if (GetRandomBool()) {
				_vibratoDepth = GetRandom() * 0.7f;
				_vibratoSpeed = GetRandom() * 0.6f;
			}
		}

		_sustainTime = GetRandom() * 0.4f;
		_decayTime = 0.1f + GetRandom() * 0.4f;
	}

	/**
	 * Sets the parameters to generate a hit/hurt sound
	 */
	public void GenerateHitHurt() {
		resetParams();

		_waveType = (uint)(GetRandom() * 3f);
		if (_waveType == 2) {
			_waveType = 3;
		} else if (_waveType == 0) {
			_squareDuty = GetRandom() * 0.6f;
		}

		_startFrequency = 0.2f + GetRandom() * 0.6f;
		_slide = -0.3f - GetRandom() * 0.4f;

		_sustainTime = GetRandom() * 0.1f;
		_decayTime = 0.1f + GetRandom() * 0.2f;

		if (GetRandomBool()) _hpFilterCutoff = GetRandom() * 0.3f;
	}

	/**
	 * Sets the parameters to generate a jump sound
	 */
	public void GenerateJump() {
		resetParams();

		_waveType = 0;
		_squareDuty = GetRandom() * 0.6f;
		_startFrequency = 0.3f + GetRandom() * 0.3f;
		_slide = 0.1f + GetRandom() * 0.2f;

		_sustainTime = 0.1f + GetRandom() * 0.3f;
		_decayTime = 0.1f + GetRandom() * 0.2f;

		if (GetRandomBool()) _hpFilterCutoff = GetRandom() * 0.3f;
		if (GetRandomBool()) _lpFilterCutoff = 1.0f - GetRandom() * 0.6f;
	}

	/**
	 * Sets the parameters to generate a blip/select sound
	 */
	public void GenerateBlipSelect() {
		resetParams();

		_waveType = (uint)(GetRandom() * 2f);
		if (_waveType == 0) _squareDuty = GetRandom() * 0.6f;

		_startFrequency = 0.2f + GetRandom() * 0.4f;

		_sustainTime = 0.1f + GetRandom() * 0.1f;
		_decayTime = GetRandom() * 0.2f;
		_hpFilterCutoff = 0.1f;
	}

	/**
	 * Resets the parameters, used at the start of each generate function
	 */
	protected void resetParams() {
		paramsDirty = true;

		_waveType = 0;
		_startFrequency			= 0.3f;
		_minFrequency			= 0.0f;
		_slide					= 0.0f;
		_deltaSlide				= 0.0f;
		_squareDuty				= 0.0f;
		_dutySweep				= 0.0f;

		_vibratoDepth			= 0.0f;
		_vibratoSpeed			= 0.0f;

		_attackTime				= 0.0f;
		_sustainTime			= 0.3f;
		_decayTime				= 0.4f;
		_sustainPunch			= 0.0f;

		_lpFilterResonance		= 0.0f;
		_lpFilterCutoff			= 1.0f;
		_lpFilterCutoffSweep	= 0.0f;
		_hpFilterCutoff			= 0.0f;
		_hpFilterCutoffSweep	= 0.0f;

		_phaserOffset			= 0.0f;
		_phaserSweep			= 0.0f;

		_repeatSpeed			= 0.0f;

		_changeSpeed			= 0.0f;
		_changeAmount			= 0.0f;

		// From BFXR
		_changeRepeat			= 0.0f;
		_changeAmount2			= 0.0f;
		_changeSpeed2			= 0.0f;

		_compressionAmount		= 0.3f;

		_overtones				= 0.0f;
		_overtoneFalloff		= 0.0f;

		_bitCrush				= 0.0f;
		_bitCrushSweep			= 0.0f;
	}


	// Randomization methods

	/**
	 * Randomly adjusts the parameters ever so slightly
	 */
	public void Mutate(float __mutation = 0.05f) {
		if (GetRandomBool()) startFrequency			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) minFrequency			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) slide					+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) deltaSlide				+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) squareDuty				+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) dutySweep				+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) vibratoDepth			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) vibratoSpeed			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) attackTime				+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) sustainTime			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) decayTime				+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) sustainPunch			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) lpFilterCutoff			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) lpFilterCutoffSweep	+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) lpFilterResonance		+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) hpFilterCutoff			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) hpFilterCutoffSweep	+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) phaserOffset			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) phaserSweep			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) repeatSpeed			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) changeSpeed			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) changeAmount			+= GetRandom() * __mutation * 2f - __mutation;

		// From BFXR
		if (GetRandomBool()) changeRepeat			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) changeAmount2			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) changeSpeed2			+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) compressionAmount		+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) overtones				+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) overtoneFalloff		+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) bitCrush				+= GetRandom() * __mutation * 2f - __mutation;
		if (GetRandomBool()) bitCrushSweep			+= GetRandom() * __mutation * 2f - __mutation;
	}

	/**
	 * Sets all parameters to random values
	 */
	public void Randomize() {
		resetParams();

		_waveType = (uint)(GetRandom() * 9f);

		_attackTime				= Pow(GetRandom() * 2f - 1f, 4);
		_sustainTime			= Pow(GetRandom() * 2f - 1f, 2);
		_sustainPunch			= Pow(GetRandom() * 0.8f, 2);
		_decayTime				= GetRandom();

		_startFrequency			= (GetRandomBool()) ? Pow(GetRandom() * 2f - 1f, 2) : (Pow(GetRandom() * 0.5f, 3) + 0.5f);
		_minFrequency			= 0.0f;

		_slide					= Pow(GetRandom() * 2f - 1f, 3);
		_deltaSlide				= Pow(GetRandom() * 2f - 1f, 3);

		_vibratoDepth			= Pow(GetRandom() * 2f - 1f, 3);
		_vibratoSpeed			= GetRandom() * 2f - 1f;

		_changeAmount			= GetRandom() * 2f - 1f;
		_changeSpeed			= GetRandom() * 2f - 1f;

		_squareDuty				= GetRandom() * 2f - 1f;
		_dutySweep				= Pow(GetRandom() * 2f - 1f, 3);

		_repeatSpeed			= GetRandom() * 2f - 1f;

		_phaserOffset			= Pow(GetRandom() * 2f - 1f, 3);
		_phaserSweep			= Pow(GetRandom() * 2f - 1f, 3);

		_lpFilterCutoff			= 1f - Pow(GetRandom(), 3);
		_lpFilterCutoffSweep	= Pow(GetRandom() * 2f - 1f, 3);
		_lpFilterResonance		= GetRandom() * 2f - 1f;

		_hpFilterCutoff			= Pow(GetRandom(), 5);
		_hpFilterCutoffSweep	= Pow(GetRandom() * 2f - 1f, 5);

		if (_attackTime + _sustainTime + _decayTime < 0.2f) {
			_sustainTime		= 0.2f + GetRandom() * 0.3f;
			_decayTime			= 0.2f + GetRandom() * 0.3f;
		}

		if ((_startFrequency > 0.7f && _slide > 0.2) || (_startFrequency < 0.2 && _slide < -0.05)) {
			_slide				= -_slide;
		}

		if (_lpFilterCutoff < 0.1f && _lpFilterCutoffSweep < -0.05f) {
			_lpFilterCutoffSweep = -_lpFilterCutoffSweep;
		}

		// From BFXR
		_changeRepeat			= GetRandom();
		_changeAmount2			= GetRandom() * 2f - 1f;
		_changeSpeed2			= GetRandom();

		_compressionAmount		= GetRandom();

		_overtones				= GetRandom();
		_overtoneFalloff		= GetRandom();

		_bitCrush				= GetRandom();
		_bitCrushSweep			= GetRandom() * 2f - 1f;
	}


	// Setting string methods

	/**
	 * Returns a string representation of the parameters for copy/paste sharing in the old format (24 parameters, SFXR/AS3SFXR compatible)
	 * @return	A comma-delimited list of parameter values
	 */
	public string GetSettingsStringLegacy() {
		string str = "";

		// 24 params

		str += waveType.ToString() + ",";
		str += To4DP(_attackTime) + ",";
		str += To4DP(_sustainTime) + ",";
		str += To4DP(_sustainPunch) + ",";
		str += To4DP(_decayTime) + ",";
		str += To4DP(_startFrequency) + ",";
		str += To4DP(_minFrequency) + ",";
		str += To4DP(_slide) + ",";
		str += To4DP(_deltaSlide) + ",";
		str += To4DP(_vibratoDepth) + ",";
		str += To4DP(_vibratoSpeed) + ",";
		str += To4DP(_changeAmount) + ",";
		str += To4DP(_changeSpeed) + ",";
		str += To4DP(_squareDuty) + ",";
		str += To4DP(_dutySweep) + ",";
		str += To4DP(_repeatSpeed) + ",";
		str += To4DP(_phaserOffset) + ",";
		str += To4DP(_phaserSweep) + ",";
		str += To4DP(_lpFilterCutoff) + ",";
		str += To4DP(_lpFilterCutoffSweep) + ",";
		str += To4DP(_lpFilterResonance) + ",";
		str += To4DP(_hpFilterCutoff) + ",";
		str += To4DP(_hpFilterCutoffSweep) + ",";
		str += To4DP(_masterVolume);

		return str;
	}

	/**
	 * Returns a string representation of the parameters for copy/paste sharing in the new format (32 parameters, BFXR compatible)
	 * @return	A comma-delimited list of parameter values
	 */
	public string GetSettingsString() {
		string str = "";

		// 32 params

		str += waveType.ToString() + ",";
		str += To4DP(_masterVolume) + ",";
		str += To4DP(_attackTime) + ",";
		str += To4DP(_sustainTime) + ",";
		str += To4DP(_sustainPunch) + ",";
		str += To4DP(_decayTime) + ",";
		str += To4DP(_compressionAmount) + ",";
		str += To4DP(_startFrequency) + ",";
		str += To4DP(_minFrequency) + ",";
		str += To4DP(_slide) + ",";
		str += To4DP(_deltaSlide) + ",";
		str += To4DP(_vibratoDepth) + ",";
		str += To4DP(_vibratoSpeed) + ",";
		str += To4DP(_overtones) + ",";
		str += To4DP(_overtoneFalloff) + ",";
		str += To4DP(_changeRepeat) + ","; // _changeRepeat?
		str += To4DP(_changeAmount) + ",";
		str += To4DP(_changeSpeed) + ",";
		str += To4DP(_changeAmount2) + ","; // changeamount2
		str += To4DP(_changeSpeed2) + ","; // changespeed2
		str += To4DP(_squareDuty) + ",";
		str += To4DP(_dutySweep) + ",";
		str += To4DP(_repeatSpeed) + ",";
		str += To4DP(_phaserOffset) + ",";
		str += To4DP(_phaserSweep) + ",";
		str += To4DP(_lpFilterCutoff) + ",";
		str += To4DP(_lpFilterCutoffSweep) + ",";
		str += To4DP(_lpFilterResonance) + ",";
		str += To4DP(_hpFilterCutoff) + ",";
		str += To4DP(_hpFilterCutoffSweep) + ",";
		str += To4DP(_bitCrush) + ",";
		str += To4DP(_bitCrushSweep);

		return str;
	}

	/**
	 * Parses a settings string into the parameters
	 * @param	string	Settings string to parse
	 * @return			If the string successfully parsed
	 */
	public bool SetSettingsString(string __string) {
		string[] values = __string.Split(new char[] { ',' });

		if (values.Length == 24) {
			// Old format (SFXR): 24 parameters
			resetParams();

			waveType = 				ParseUint(values[0]);
			attackTime =  			ParseFloat(values[1]);
			sustainTime =  			ParseFloat(values[2]);
			sustainPunch =  		ParseFloat(values[3]);
			decayTime =  			ParseFloat(values[4]);
			startFrequency =  		ParseFloat(values[5]);
			minFrequency =  		ParseFloat(values[6]);
			slide =  				ParseFloat(values[7]);
			deltaSlide =  			ParseFloat(values[8]);
			vibratoDepth =  		ParseFloat(values[9]);
			vibratoSpeed =  		ParseFloat(values[10]);
			changeAmount =  		ParseFloat(values[11]);
			changeSpeed =  			ParseFloat(values[12]);
			squareDuty =  			ParseFloat(values[13]);
			dutySweep =  			ParseFloat(values[14]);
			repeatSpeed =  			ParseFloat(values[15]);
			phaserOffset =  		ParseFloat(values[16]);
			phaserSweep =  			ParseFloat(values[17]);
			lpFilterCutoff =  		ParseFloat(values[18]);
			lpFilterCutoffSweep =	ParseFloat(values[19]);
			lpFilterResonance =  	ParseFloat(values[20]);
			hpFilterCutoff =  		ParseFloat(values[21]);
			hpFilterCutoffSweep =	ParseFloat(values[22]);
			masterVolume = 			ParseFloat(values[23]);
		} else if (values.Length >= 32) {
			// New format (BFXR): 32 parameters (or more, but locked parameters are ignored)
			resetParams();

			waveType				= ParseUint(values[0]);
			masterVolume			= ParseFloat(values[1]);
			attackTime				= ParseFloat(values[2]);
			sustainTime				= ParseFloat(values[3]);
			sustainPunch			= ParseFloat(values[4]);
			decayTime				= ParseFloat(values[5]);
			compressionAmount		= ParseFloat(values[6]);
			startFrequency			= ParseFloat(values[7]);
			minFrequency			= ParseFloat(values[8]);
			slide					= ParseFloat(values[9]);
			deltaSlide				= ParseFloat(values[10]);
			vibratoDepth			= ParseFloat(values[11]);
			vibratoSpeed			= ParseFloat(values[12]);
			overtones				= ParseFloat(values[13]);
			overtoneFalloff			= ParseFloat(values[14]);
			changeRepeat			= ParseFloat(values[15]);
			changeAmount			= ParseFloat(values[16]);
			changeSpeed				= ParseFloat(values[17]);
			changeAmount2			= ParseFloat(values[18]);
			changeSpeed2			= ParseFloat(values[19]);
			squareDuty				= ParseFloat(values[20]);
			dutySweep				= ParseFloat(values[21]);
			repeatSpeed				= ParseFloat(values[22]);
			phaserOffset			= ParseFloat(values[23]);
			phaserSweep				= ParseFloat(values[24]);
			lpFilterCutoff			= ParseFloat(values[25]);
			lpFilterCutoffSweep		= ParseFloat(values[26]);
			lpFilterResonance		= ParseFloat(values[27]);
			hpFilterCutoff			= ParseFloat(values[28]);
			hpFilterCutoffSweep		= ParseFloat(values[29]);
			bitCrush				= ParseFloat(values[30]);
			bitCrushSweep			= ParseFloat(values[31]);
		} else {
			Debug.LogError("Could not paste settings string: parameters contain " + values.Length + " values (was expecting 24 or >32)");
			return false;
		}

		return true;
	}


	// Copying methods

	/**
	 * Returns a copy of this SfxrParams with all settings duplicated
	 * @return	A copy of this SfxrParams
	 */
	public SfxrParams Clone() {
		SfxrParams outp = new SfxrParams();
		outp.CopyFrom(this);

		return outp;
	}

	/**
	 * Copies parameters from another instance
	 * @param	params	Instance to copy parameters from
	 */
	public void CopyFrom(SfxrParams __params, bool __makeDirty = false) {
		bool wasDirty = paramsDirty;
		SetSettingsString(GetSettingsString());
		paramsDirty = wasDirty || __makeDirty;
	}


	// Utility methods

	/**
	 * Faster power function; this function takes about 36% of the time Mathf.Pow() would take in our use cases
	 * @param	base		Base to raise to power
	 * @param	power		Power to raise base by
	 * @return				The calculated power
	 */
	private float Pow(float __pbase, int __power) {
		switch(__power) {
			case 2: return __pbase * __pbase;
			case 3: return __pbase * __pbase * __pbase;
			case 4: return __pbase * __pbase * __pbase * __pbase;
			case 5: return __pbase * __pbase * __pbase * __pbase * __pbase;
		}

		return 1f;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	/**
	 * Returns the number as a string to 4 decimal places
	 * @param	value	Number to convert
	 * @return			Number to 4dp as a string
	 */
	private string To4DP(float __value) {
		if (__value < 0.0001f && __value > -0.0001f) return "";
		return __value.ToString("#.####");
	}

	/**
	 * Parses a string into an uint value; also returns 0 if the string is empty, rather than an error
	 */
	private uint ParseUint(string __value) {
		if (__value.Length == 0) return 0;
		return uint.Parse(__value);
	}

	/**
	 * Parses a string into a float value; also returns 0 if the string is empty, rather than an error
	 */
	private float ParseFloat(string __value) {
		if (__value.Length == 0) return 0;
		return float.Parse(__value);
	}

	/**
	 * Returns a random value: 0 <= n < 1
	 * This function is needed so we can follow the original code more strictly; Unity's Random.value returns 0 <= n <= 1
	 */
	private float GetRandom() {
		return UnityEngine.Random.value % 1;
	}

	/**
	 * Returns a boolean value
	 */
	private bool GetRandomBool() {
		return UnityEngine.Random.value > 0.5f;
	}
}