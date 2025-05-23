using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TireSlider : MonoBehaviour {

	public AudioClip SlideSound;
	public float SlipScale = 0.02f;
	public float SlipLinearity = 2f;
	public float MaxPitch = 3f;

	AudioSource SlideAudio;
	List<Wheel> Wheels;

	void Start() {
		SlideAudio = gameObject.AddComponent<AudioSource>();
		SlideAudio.clip = SlideSound;
		SlideAudio.loop = true;
		SlideAudio.volume = 0f;
		SlideAudio.Play();
		Wheels = transform.parent.GetComponentsInChildren<Wheel>().ToList();
		Scheduler.TireSliderUpdate += ManualUpdate;
	}

	void OnDestroy() {
		Scheduler.TireSliderUpdate -= ManualUpdate;
	}

	void ManualUpdate(float delta) {
		/*
		float slip = 0f;
		foreach (var w in Wheels) slip += w.CurrentSlipForce;
		slip = Mathf.Pow(SlipScale * slip, SlipLinearity);
		SlideAudio.pitch = MaxPitch / (1f + slip);
		SlideAudio.volume = slip;
		*/
	}

}