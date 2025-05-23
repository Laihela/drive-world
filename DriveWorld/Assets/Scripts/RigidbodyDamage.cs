using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyDamage : MonoBehaviour {

	void OnCollisionEnter(Collision collision) {
		foreach (var x in GetComponentsInChildren<BreakablePanel>()) {
			x.ChangeHealth(-collision.impulse.sqrMagnitude);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
			foreach (var x in GetComponentsInChildren<BreakablePanel>()) {
				x.ChangeHealth(+10f);
			}
		}
	}

}