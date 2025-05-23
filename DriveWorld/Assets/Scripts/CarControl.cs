using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CarControl : MonoBehaviour {

	public float TopSpeed = 100f;
	public bool AutoInertiaTensor = true;
	public Vector3 InertiaTensor = Vector3.zero;

	Rigidbody Rigidbody;
	CarEngine Engine;

	void Start() {
		Rigidbody = GetComponent<Rigidbody>();
		Engine = GetComponentInChildren<CarEngine>();
		var CoM = transform.Find("CoM");
		if (CoM != null) Rigidbody.centerOfMass = CoM.localPosition;
		Scheduler.CarControlUpdate += ManualUpdate;
	}

	void OnDestroy() {
		Scheduler.CarControlUpdate -= ManualUpdate;
	}

	void Update() {
		// Gear control
		if (Input.GetKeyDown(KeyCode.LeftShift)) GetComponentInChildren<DriveTrainTransmission>().ShiftUp();
		if (Input.GetKeyDown(KeyCode.LeftControl)) GetComponentInChildren<DriveTrainTransmission>().ShiftDown();
	}

	void ManualUpdate(float delta) {

		// Rigidbody control
		if (AutoInertiaTensor) {
			Rigidbody.ResetInertiaTensor();
			InertiaTensor = Rigidbody.inertiaTensor;
		}
		else Rigidbody.inertiaTensor = InertiaTensor;

		// Engine control
		Engine.Throttle = Input.GetKey(KeyCode.W) ? 1f : 0f;
	}

}