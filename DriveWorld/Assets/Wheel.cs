using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Wheel : MonoBehaviour {

	public float SuspensionLength = 0.35f;
	public float SuspenionsStiffness = 30f;
	public float SuspenionsDamping = 2f;
	public float SteeringAngle = 0f;
	public float DriveTorque = 0f;
	public float BrakeTorque = 0f;
	public bool FlipSide = false;
	public float Friction = 1f;
	public float Radius = 1f;

	public float CurrentSteer = 0f;
	public float CurrentSpringForce = 0f;
	public float CurrentRotationSpeed = 0f;
	public float CurrentSlipForce = 0f;

	float FrictionAttack = 1f;
	float AntiSlideMax = 5f;
	float AntiSlideGain = 5f;

	ParticleSystem SmokeParticles;
	Rigidbody Rigidbody;
	Transform Tire;
	Vector3 AntiSlideVelocity = Vector3.zero;
	Quaternion StartRotation;
	const float WheelMass = 3f;

	void Start() {
		SmokeParticles = transform.parent.Find("TireSlider").GetComponent<ParticleSystem>();
		Rigidbody = GetComponentInParent<Rigidbody>();
		Tire = transform.GetChild(0);
		StartRotation = transform.localRotation;
		Scheduler.WheelUpdate += ManualUpdate;
	}

	void OnDestroy() {
		Scheduler.WheelUpdate -= ManualUpdate;
	}

	public void ManualUpdate(float delta) {

		bool hit = Physics.Raycast(transform.position, -transform.up, out var contact, SuspensionLength);
		if (hit) {
			// Velocity between the tire and contact surfaces
			Vector3 contactVelocity = Rigidbody.GetPointVelocity(contact.point) - CurrentRotationSpeed * Radius * transform.forward;
			// Prevent sliding due to constant forces such as gravity
			AntiSlideVelocity += AntiSlideGain * contactVelocity;
			AntiSlideVelocity = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(AntiSlideVelocity, contact.normal), AntiSlideMax);
			// Spring force
			CurrentSpringForce = (SuspensionLength - contact.distance) * SuspenionsStiffness;
			// Dampener force
			CurrentSpringForce -= Vector3.Dot(contactVelocity, contact.normal) * SuspenionsDamping;
			// Clamp so that the spring doesn't suck the vehicle into the ground
			CurrentSpringForce = Mathf.Max(0f, CurrentSpringForce);
			// Turn spring force into vector
			Vector3 spring = CurrentSpringForce * transform.up;
			// Slipping
			float slipping = Mathf.Clamp01(contactVelocity.magnitude * FrictionAttack - CurrentSpringForce * Friction);
			CurrentSlipForce = slipping * CurrentSpringForce * Friction;
			// Friction force as vector
			Vector3 friction = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(FrictionAttack * -1f * (contactVelocity + AntiSlideVelocity), contact.normal), CurrentSpringForce * Friction);
			// Apply forces
			Rigidbody.AddForceAtPosition(spring + friction, contact.point);
			// Set wheel on the ground
			Tire.position = contact.point + Radius * transform.up;
			// Slow down wheel spin according to friction force
			CurrentRotationSpeed -= 1f / Radius / WheelMass * Vector3.Dot(friction, transform.forward);
			// Tire smoke
			if (friction.magnitude >= CurrentSpringForce * Friction - 0.01f) {
				SmokeParticles.Emit(new() { position = contact.point, velocity = friction }, 1);
			}
		}
		else {
			Tire.localPosition = (Radius - SuspensionLength) * Vector3.up;
			float friction = 10f / WheelMass * delta;
			CurrentRotationSpeed -= Mathf.Clamp(CurrentRotationSpeed, -friction, friction);
			CurrentSlipForce = 0f;
		}

		if (DriveTorque != 0f) Driving(delta);
		if (BrakeTorque != 0f) Braking(delta);
		if (SteeringAngle != 0f) Steering(delta);

		// Rotate the wheel object
		Tire.Rotate(CurrentRotationSpeed * 180f / Mathf.PI * delta, 0f, 0f, Space.Self);
	}

	void Braking(float delta) {
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Space)) {
			float force = BrakeTorque / WheelMass * delta;
			CurrentRotationSpeed -= Mathf.Clamp(CurrentRotationSpeed, -force, force);
		}
	}
	
	void Driving(float delta) {
		if (Input.GetKey(KeyCode.W)) {
			CurrentRotationSpeed += (FlipSide ? -DriveTorque : DriveTorque) / WheelMass * delta;
		}
	}

	void Steering(float delta) {
		if (Input.GetKey(KeyCode.A)) {
			CurrentSteer -= 4f * delta;
		}
		if (Input.GetKey(KeyCode.D)) {
			CurrentSteer += 4f * delta;
		}
		CurrentSteer = Mathf.Clamp(CurrentSteer - Mathf.Clamp(CurrentSteer, -2f * delta, 2f * delta), -1f, 1f);
		transform.localRotation = StartRotation * Quaternion.Euler(0f, CurrentSteer * SteeringAngle, 0f);
	}

}