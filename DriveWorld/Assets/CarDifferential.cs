using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDifferential : MonoBehaviour {

	public List<Wheel> Wheels = new();
	[Range(0f, 1f)]
	public float Slip = 0f;

	void Start() {
		Scheduler.TransmissionUpdate += ManualUpdate;
	}
	
	void OnDestroy() {
		Scheduler.TransmissionUpdate -= ManualUpdate;
	}

	void ManualUpdate(float delta) {
		if (Wheels.Count == 0 || Slip == 1f) return;

		float averageSpeed = 0f;
		foreach (var wheel in Wheels) {
			averageSpeed += (wheel.FlipSide ? -1f : 1f) * wheel.CurrentRotationSpeed;
		}
		averageSpeed /= Wheels.Count;
		foreach (var wheel in Wheels) {
			wheel.CurrentRotationSpeed = Mathf.Lerp((wheel.FlipSide ? -1f : 1f) * averageSpeed, wheel.CurrentRotationSpeed, Slip);
		}

	}

}