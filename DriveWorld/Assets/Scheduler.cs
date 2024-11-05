using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour {
	
	public delegate void UpdateDelegate(float delta);

	public static UpdateDelegate CarControlUpdate = (x) => { };
	public static UpdateDelegate WheelUpdate = (x) => { };
	public static UpdateDelegate TransmissionUpdate = (x) => { };
	public static UpdateDelegate TireSliderUpdate = (x) => { };

	void Awake() {
		Application.targetFrameRate = 60;
		CarControlUpdate = (x) => { };
		TransmissionUpdate = (x) => { };
		WheelUpdate = (x) => { };
		TireSliderUpdate = (x) => { };
	}

	void FixedUpdate() {
		CarControlUpdate(Time.fixedDeltaTime);
		WheelUpdate(Time.fixedDeltaTime);
		TransmissionUpdate(Time.fixedDeltaTime);
		TireSliderUpdate(Time.fixedDeltaTime);
	}
}