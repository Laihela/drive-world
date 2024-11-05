using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CarControl : MonoBehaviour {

	public float TopSpeed = 100f;

	Rigidbody Rigidbody;
	EngineSound EngineSound;
	List<Wheel> Wheels;

	void Start() {
		Rigidbody = GetComponent<Rigidbody>();
		EngineSound = GetComponent<EngineSound>();
		Wheels = GetComponentsInChildren<Wheel>().ToList();
		var CoM = transform.Find("CoM");
		if (CoM != null) Rigidbody.centerOfMass = CoM.localPosition;
		Scheduler.CarControlUpdate += ManualUpdate;
	}

	void OnDestroy() {
		Scheduler.CarControlUpdate -= ManualUpdate;
	}

	void ManualUpdate(float delta) {

		if (Input.GetKey(KeyCode.W)) {
			//Rigidbody.AddForce(transform.forward * Acceleration);
			EngineSound.Power += 10f * delta;
		}
		else {
			EngineSound.Power -= 10f * delta;
		}
		float speed = 0f;
		foreach (var w in Wheels) {
			if (w.DriveTorque != 0) {
				speed += Mathf.Abs(w.CurrentRotationSpeed);
			}
		}
		EngineSound.Speed = speed / TopSpeed;
	}

}