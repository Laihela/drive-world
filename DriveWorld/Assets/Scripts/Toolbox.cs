using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Toolbox : MonoBehaviour {

	// Properties
	public AudioMixerGroup DefaultMixerGroup;
	public static Toolbox Instance { get; private set; }
	public const float RadToRPM = 9.54929658551f;
	public const float RpmToRad = 0.10471975512f;

	// Public functions
	public static AudioSource CreateLoopingAudioSource(in GameObject host, in AudioClip clip = null) {
		var newSource = CreateAudioSource(host, clip);
		newSource.volume = 0f;
		newSource.loop = true;
		newSource.Play();
		return newSource;
	}
	public static AudioSource CreateAudioSource(in GameObject host, in AudioClip clip = null) {
		AudioSource newSource = host.AddComponent<AudioSource>();
		newSource.outputAudioMixerGroup = Instance.DefaultMixerGroup;
		newSource.clip = clip;
		return newSource;
	}

	// Unity functions
	void Awake() {
		if (Instance == null) Instance = this;
		else throw new System.Exception("More than one instance exists!");
	}

}