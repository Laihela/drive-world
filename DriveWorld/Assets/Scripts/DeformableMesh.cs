using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DeformableMesh : MonoBehaviour {

	[field: SerializeField, Range(0f, 1f)]
	public float Damage { get; private set; } = 0f;
	[field: SerializeField]
	public AudioClip ImpactSound { get; private set; }

	AudioSource impactSource = null;
	float LastDamage = -1f;

	public void OnCollision(Collision collision) {
		print("smash");
		Damage += 0.2f;
		impactSource.Play();
	}

	void Start() {
		if (Application.isPlaying == false) return;
		impactSource = Toolbox.CreateAudioSource(gameObject, ImpactSound);
	}

	void Update() {
		if (LastDamage == Damage) return;
		print("updated");
		LastDamage = Damage;
		UpdateRenderer();
	}

	void UpdateRenderer() {
		foreach (var renderer in transform.GetComponentsInChildren<SkinnedMeshRenderer>()) {
			renderer.SetBlendShapeWeight(0, Damage * 100f);
		}
	}
}