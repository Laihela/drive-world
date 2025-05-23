using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DriveTrainTransmission : CarDrivetrainComponent {

	// Properties
	[field:SerializeField]
	public CarDrivetrainComponent Output	{ get; set; }
	[field: SerializeField]
	public float FinalDriveRatio			{ get; set; } = 3f;
	[field: SerializeField]
	public List<Gear> Gears					{ get; set; }
	public int CurrentGear					{ get; set; } = 1;
	public string CurrentGearName			{ get; set; } = "";
	public bool Engaged						{ get; set; } = true;

	override public void AddTorque(float torque) => Output.AddTorque(torque * CurrentGearRatio);
	override public float GetSpeed() => Output.GetSpeed() * CurrentGearRatio;

	public void ShiftUp() => SetGear(CurrentGear + 1);
	public void ShiftDown() => SetGear(CurrentGear - 1);
	public void SetGear(int gearIndex) {
		CurrentGear = Mathf.Clamp(gearIndex, 0, Gears.Count - 1);
		CurrentGearName = Gears[CurrentGear].Name;
		CurrentGearRatio = Gears[CurrentGear].Ratio * FinalDriveRatio;
		pid.Reset();
	}

	[System.Serializable]
	public struct Gear { public string Name; public float Ratio; }

	PID pid = new(0.20f, 150.00f, 0.00f);
	float CurrentGearRatio = 1f;
	float ShiftTimer = 0f;

	void TransmissionUpdate(float delta) {
		float speed = Output.GetSpeed();
		float redLineSpeed = transform.parent.GetComponentInChildren<CarEngine>().RedLineRPM * Toolbox.RpmToRad;

		if (ShiftTimer > 0f) ShiftTimer -= delta;
		else {
			int safety = 0;
			while (safety < 1000 && CurrentGear < Gears.Count - 1 && speed * CurrentGearRatio > 0.95f * redLineSpeed) {
				ShiftUp();
				safety++;
			}
			while (safety < 1000 && CurrentGear > 1 && speed * Gears[CurrentGear - 1].Ratio * FinalDriveRatio < 0.9f * redLineSpeed) {
				ShiftDown();
				safety++;
			}
			if (safety > 999) Debug.LogError("Infinite loop detected");
			ShiftTimer = 0.5f;
		}
	}
	
	void Start() {
		SetGear(CurrentGear);
		Scheduler.TransmissionUpdate += TransmissionUpdate;
	}

	void OnDestroy() {
		Scheduler.TransmissionUpdate -= TransmissionUpdate;
	}
}