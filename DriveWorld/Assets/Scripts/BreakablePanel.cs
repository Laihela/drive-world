using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BreakablePanel : MonoBehaviour {

	[field:SerializeField]
	public float MaxHealth { get; set; } = 100f;
	[field: SerializeField]
	public float DetatchHealth { get; set; } = 0f;
	public float Health { get; private set; }

	[field:SerializeField]
	AudioClip DamageSound;
	AudioSource AudioSource;
	SkinnedMeshRenderer MeshRenderer;
	bool detatched = false;

	public void ChangeHealth(float amount) {
		Health = Mathf.Clamp(Health + amount, 0f, MaxHealth);
		if (amount < 0f && detatched == false) {
			PlayImpactSound(-amount / MaxHealth);
		}
		UpdateRenderer();
		if (Health <= DetatchHealth && detatched == false) {
			SpawnScrap();
			MeshRenderer.enabled = false;
			detatched = true;
		}
		else if (Health > DetatchHealth && detatched) {
			MeshRenderer.enabled = true;
			detatched = false;
		}
	}

	void Start() {
		TryGetComponent(out MeshRenderer);
		if (MeshRenderer == null) {
			Debug.LogWarning("BreakablePanel self-destructing due to missing SkinnedMeshRenderer");
			Destroy(this);
			return;
		}
		AudioSource = Toolbox.CreateAudioSource(gameObject, DamageSound);
		Health = MaxHealth;
		UpdateRenderer();
	}

	void PlayImpactSound(float strength) {
		AudioSource.pitch = Mathf.Clamp(2f / (1f + strength), 0.2f, 1f);
		AudioSource.PlayOneShot(DamageSound, Mathf.Pow(strength, 0.5f));
	}

	void SpawnScrap() {
		var rigidbody = GetComponentInParent<Rigidbody>();
		var scrap = Instantiate(gameObject, rigidbody.transform.parent);
		Destroy(scrap.GetComponent<BreakablePanel>());
		var body = scrap.AddComponent<Rigidbody>();
		body.mass = rigidbody.mass * 0.1f;
		body.velocity = rigidbody.velocity;
		body.angularVelocity = rigidbody.angularVelocity;
		body.maxAngularVelocity = rigidbody.maxAngularVelocity;
		var collider = scrap.AddComponent<MeshCollider>();
		collider.convex = true;
		collider.sharedMesh = MeshRenderer.sharedMesh;
		scrap.transform.SetPositionAndRotation(transform.position, transform.rotation);
	}

	void UpdateRenderer() {
		MeshRenderer.SetBlendShapeWeight(0, (1f - Health / MaxHealth) * 100f);
	}

}
