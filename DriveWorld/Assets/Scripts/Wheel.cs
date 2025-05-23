using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : CarDrivetrainComponent {
	// LAST EDIT: 24/12/2024

	public float Radius = 1f;
	public float AngularMass = 0.02f;
	public float SuspensionLength = 0.35f;
	public float SuspensionStiffness = 30f;
	[Range(0f, 1f)]
	public float SuspensionDamping = 0.1f;
	[Range(0.2f, 2f)]
	public float SuspensionDampingLinearity = 1f;
	[Range(-60f, 60f)]
	public float SteerAngle = 0f;
	[Range(0f, 2f)]
	public float FrictionCoefficient = 1f;
	[Range(0f, 2f)]
	public float FrictionStiffness = 1f;
	public float ModelRotation = 5f;
	public float CompressionModelRotation = -10f;

	float CurrentSteerAngle = 0f;
	float CurrentWheelRotation = 0f;
	float CurrentAgularSpeed = 0f;

	Rigidbody Rigidbody;
	Transform WheelObject;
	Quaternion StartRotation;

	void Start() {
		Rigidbody = GetComponentInParent<Rigidbody>();
		WheelObject = transform.GetChild(0);
		StartRotation = transform.localRotation;
		Scheduler.WheelUpdate += WheelUpdate;
	}

	void OnDestroy() {
		Scheduler.WheelUpdate -= WheelUpdate;
	}

	public override float GetSpeed() {
		return CurrentAgularSpeed;
	}

	public override void AddTorque(float torque) {
		// Return if NaN, inf, -inf, or zero.
		if (float.IsSubnormal(torque)) return;
		CurrentAgularSpeed += torque / AngularMass * Time.fixedDeltaTime;
	}

	public void WheelUpdate(float delta) {

		if (SteerAngle != 0f) Steering(delta);

		bool hit = Physics.Raycast(transform.position, -transform.up, out var contact, SuspensionLength);
		Vector3 contactVelocity = hit ? Rigidbody.GetPointVelocity(contact.point) : Vector3.zero;

		DoThaTing(delta, contact, contactVelocity);
	}

	void DoThaTing(float delta, in RaycastHit contact, Vector3 contactVelocity) {
		float springCompression = 0f;
		// Passive friction
		AddTorque(-100f * AngularMass * Mathf.Sign(CurrentAgularSpeed) * delta);
		if (contact.collider != null) {
			Vector3 contactForward = Vector3.Cross(transform.right, contact.normal);
			Vector3 slipVelocity = contactVelocity - CurrentAgularSpeed * Radius * contactForward;
			// Suspension
			float spring = SuspensionStiffness * (SuspensionLength - contact.distance) / SuspensionLength;
			float dampener = -SuspensionStiffness * SuspensionDamping * Lin(Vector3.Dot(slipVelocity, contact.normal), SuspensionDampingLinearity);
			float suspension = Mathf.Max(0f, spring + dampener);
			// Friction
			float frictionResponse = Mathf.Lerp(2f, FrictionStiffness, 0.1f * CurrentAgularSpeed);
			float lateralFriction = -Vector3.Dot(contactVelocity * frictionResponse, transform.right) * FrictionCoefficient * suspension;
			float forwardFriction = (CurrentAgularSpeed - Vector3.Dot(contactForward, contactVelocity) / Radius) * AngularMass / delta;
			Vector3 frictionForce = Vector3.ClampMagnitude(lateralFriction * transform.right + forwardFriction * contactForward, FrictionCoefficient * suspension);
			//Vector3 friction = Vector3.ClampMagnitude(-slipVelocityProjected * 300f, FrictionCoefficient * suspension);
			Rigidbody.AddForceAtPosition(( suspension * contact.normal + frictionForce ), contact.point);
			AddTorque(-Vector3.Dot(frictionForce, contactForward));
			springCompression = (1f - contact.distance / SuspensionLength);
			WheelObject.transform.localPosition = (Radius - contact.distance) * Vector3.up;
		}
		CurrentWheelRotation += CurrentAgularSpeed * 180f / Mathf.PI * delta;
		CurrentWheelRotation %= 360f;
		WheelObject.localEulerAngles = (ModelRotation + CompressionModelRotation * springCompression) * Vector3.forward;
		WheelObject.Rotate(CurrentWheelRotation * Vector3.right);
		WheelObject.localPosition = (Radius - SuspensionLength * (1f - springCompression)) * Vector3.up;
	}

	float Lin(float value, float linearity) {
		return Mathf.Sign(value) * Mathf.Pow(Mathf.Abs(value), linearity);
	}

	void Steering(float delta) {
		if (Input.GetKey(KeyCode.A)) {
			CurrentSteerAngle -= 4f * delta;
		}
		if (Input.GetKey(KeyCode.D)) {
			CurrentSteerAngle += 4f * delta;
		}
		CurrentSteerAngle = Mathf.Clamp(CurrentSteerAngle - Mathf.Clamp(CurrentSteerAngle, -2f * delta, 2f * delta), -1f, 1f);
		transform.localRotation = StartRotation * Quaternion.Euler(0f, CurrentSteerAngle * SteerAngle, 0f);
	}
}