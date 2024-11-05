using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour {

	public AudioClip IdleClip;
	public float IdlePitch = 1f;
	public AudioClip PowerClip;
	public float PowerPitch = 1f;
	public float SpeedPitch = 1f;
	[Range(0f,1f)]
	public float Power = 0f;
	[Range(0f, 1f)]
	public float Speed = 0f;

	AudioSource IdleSound;
	AudioSource PowerSound;

	void Start() {
		IdleSound = gameObject.AddComponent<AudioSource>();
		IdleSound.clip = IdleClip;
		IdleSound.loop = true;
		IdleSound.Play();
		PowerSound = gameObject.AddComponent<AudioSource>();
		PowerSound.clip = PowerClip;
		PowerSound.loop = true;
		PowerSound.Play();
	}

	void FixedUpdate() {

		Power = Mathf.Clamp01(Power);
		Speed = Mathf.Clamp01(Speed);

		IdleSound.pitch = IdlePitch * (1.0f + Speed * SpeedPitch);
		IdleSound.volume = Mathf.Clamp01(1f - Power);
		PowerSound.pitch = PowerPitch * (1.0f + Speed * SpeedPitch);
		PowerSound.volume = Mathf.Clamp01(Power);
	}

}