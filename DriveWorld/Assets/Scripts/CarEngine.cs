using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour {

	public CarDrivetrainComponent Output;

	public float Friction = 0.25f;
	public AnimationCurve TorqueCurve;
	public AnimationCurve PowerCurve;
	public string PeakTorque = "";
	public string PeakPower = "";
	/// <summary>Newton-meters</summary>
	public float MaxTorque = 200f;
	public AudioClip EngineSound;
	public float EngineSoundPitch = 1f;
	public float EngineSoundVolume = 1f;
	public bool RedrawCurves = false;

	[Range(0f,1f)]
	public float Throttle = 0f;
	public float RedLineRPM = 4000f;
	public float LowPassMin = 4000f;
	public float LowPassMax = 8000f;
	public float IdleRPM = 800f;
	public float VibratoHz = 60f;
	[Range(0f, 1f)]
	public float VibratoGain = 0.03f;
	[Range(0f, 1f)]
	public float VibratoDecay = 0.05f;

	AudioSource EngineSoundSource;
	//AudioDistortionFilter Distortion;
	AudioLowPassFilter LowPass;

	float LastSpeed = 0f;
	float LastThrottle = 0f;
	float Vibrato = 0f;

	void Start() {
		EngineSoundSource = Toolbox.CreateLoopingAudioSource(gameObject, EngineSound);
		//Distortion = gameObject.AddComponent<AudioDistortionFilter>();
		//Distortion.distortionLevel = 0.5f;
		LowPass = gameObject.AddComponent<AudioLowPassFilter>();
		//FullThrottleSource = Toolbox.CreateAudioSource(gameObject, FullThrottleSound);
		//StarterSource = Toolbox.CreateAudioSource(gameObject, StarterSound);
		Scheduler.TransmissionUpdate += TransmissionUpdate;
	}

	void OnDestroy() {
		Scheduler.TransmissionUpdate -= TransmissionUpdate;
	}

	void DrawCurves() {
		/*
		float maxTorqueNm = 0f;
		TorqueCurve = new();
		for (float f = 0f; f < 1f; f += 0.02f) {
			float torque = Mathf.Max(0f, CombustionTorque.Evaluate(f) - StaticResistance * Mathf.Sign(f) - DynamicResistance * f);
			TorqueCurve.AddKey(new(f, torque, 0, 0, 0, 0));
			float Nm = torque * MaxTorque;
			maxTorqueNm = Mathf.Max(maxTorqueNm, Nm);
		}
		PeakTorque = maxTorqueNm.ToString("0.0Nm");
		*/

		float maxPowerWatt = 0f;
		float maxPowerHorse = 0f;
		PowerCurve = new();
		for (float f = 0f; f < 1f; f += 0.02f) {
			float power = f * (TorqueCurve.Evaluate(f));
			PowerCurve.AddKey(new(f, power, 0, 0, 0, 0));
			float mPW = power * MaxTorque * RedLineRPM * Toolbox.RpmToRad;
			float mPH = mPW / 745.7f; // 1hp = 745.7w
			maxPowerWatt = Mathf.Max(maxPowerWatt, mPW * 0.001f);
			maxPowerHorse = Mathf.Max(maxPowerHorse, mPH);
		}
		PeakPower = maxPowerWatt.ToString("0.0kW ") + maxPowerHorse.ToString("0.0HP");
	}

	void TransmissionUpdate(float delta) {
		Throttle = LastThrottle + Mathf.Clamp(Throttle - LastThrottle, -10f * delta, 10f * delta);
		if (RedrawCurves) {
			DrawCurves();
			RedrawCurves = false;
		}
		float speed01 = Output.GetSpeed() * Toolbox.RadToRPM / RedLineRPM;
		float torque01 = (Throttle * (1f + Friction) * TorqueCurve.Evaluate(Mathf.Max(speed01, IdleRPM / RedLineRPM)));

		Output.AddTorque((torque01 - Friction * Mathf.Sign(speed01)) * MaxTorque * 0.001f); // Output kW instead of W

		//Vibrato = Vibrato * (1f - VibratoDecay) + Mathf.Pow(Mathf.Abs(speed01 - LastSpeed), 2f) * VibratoGain / delta;
		EngineSoundSource.pitch = EngineSoundPitch * Mathf.Max(speed01, IdleRPM / RedLineRPM) * (1f + Vibrato * Mathf.Sin(Time.fixedTime * VibratoHz));
		EngineSoundSource.volume = EngineSoundVolume * Mathf.Max(TorqueCurve.Evaluate(speed01), 0.5f);

		LowPass.cutoffFrequency = Mathf.Lerp(LowPassMin, LowPassMax, Throttle * TorqueCurve.Evaluate(speed01));

		LastSpeed = speed01;
		LastThrottle = Throttle;
	}
}