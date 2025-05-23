using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivetrainDifferential : CarDrivetrainComponent {

	// Properties
	[field:SerializeField]
	public CarDrivetrainComponent OutputA { get; set; }
	[field: SerializeField]
	public CarDrivetrainComponent OutputB { get; set; }
	[field: SerializeField]
	public float ViscousResistance { get; set; } = 0f;
	[field: SerializeField]
	public float StaticResistance { get; set; } = 0f;

	void Start() {
		Scheduler.TransmissionUpdate += TransmissionUpdate;
	}
	
	void OnDestroy() {
		Scheduler.TransmissionUpdate -= TransmissionUpdate;
	}

	void TransmissionUpdate(float delta) {
		print("ello");
		float speed = (OutputA == null ? 0f : OutputA.GetSpeed())
		            - (OutputB == null ? 0f : OutputB.GetSpeed());
		float momentum = speed; // Scale with mass later;
		float resistance = ViscousResistance * speed + StaticResistance * Mathf.Sign(speed);
		float torque = Mathf.Clamp(-0.5f * momentum, -Mathf.Abs(resistance), Mathf.Abs(resistance));
		if (OutputA != null) OutputA.AddTorque(torque);
		if (OutputB != null) OutputB.AddTorque(-torque);
	}

	public override void AddTorque(float torque) {
		if (OutputA != null) OutputA.AddTorque(0.5f * torque);
		if (OutputB != null) OutputB.AddTorque(0.5f * torque);
	}

	public override float GetSpeed() {
		return 0.5f * ((OutputA == null ? 0f : OutputA.GetSpeed()) + (OutputB == null ? 0f : OutputB.GetSpeed()));
	}

}