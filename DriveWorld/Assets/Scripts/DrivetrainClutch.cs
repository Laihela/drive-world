using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrivetrainClutch : CarDrivetrainComponent {

	public List<CarDrivetrainComponent> Connections = new();
	public float Force = 10f;

	public override void AddTorque(float torque) {
		throw new System.NotImplementedException();
	}

	public override float GetSpeed() {
		throw new System.NotImplementedException();
	}

	/*
	public override float Speed {
		get => GetSpeed();
		set => SetSpeed(value);
	}

	override public void Start() {
		base.Start();
		Scheduler.ClutchUpdate += ClutchUpdate;
	}
	
	override public void OnDestroy() {
		base.OnDestroy();
		Scheduler.ClutchUpdate -= ClutchUpdate;
	}

	public override void AddTorque(float torque) {
		foreach (var connection in Connections) {
			connection.AddTorque(torque / Connections.Count);
		}
	}



	void ClutchUpdate(float delta) {
		if (Connections.Count == 0 || Force == 0f) return;

		// Loop all unique pairs and add torque between them
		for (int a = 0; a < Connections.Count - 1; a++) {
			for (int b = a + 1; b < Connections.Count; b++) {
				float torque = (Connections[b].Speed - Connections[a].Speed) * Force;
				Connections[a].AddTorque(torque);
				Connections[b].AddTorque(-torque);
				//print($"Added {(Connections[a].Speed - Connections[b].Speed) * Force} torque between {Connections[a].name} and {Connections[b].name}");
			}
		}
	}

	void SetSpeed(float value) {
		float sumMass = GetMass();
		float sumSpeed = GetSpeed();
		foreach (var connection in Connections) {
			connection.AddTorque((value - sumSpeed) * sumMass / Connections.Count);
		}
	}

	float GetMass() {
		float sum = 0f;
		foreach (var connection in Connections) {
			sum += connection.Mass;
		}
		return sum / Connections.Count;
	}

	override public float GetSpeed() {
		float sum = 0f;
		foreach (var connection in Connections) {
			sum += connection.Speed;
		}
		return sum / Connections.Count;
	}
	*/
}