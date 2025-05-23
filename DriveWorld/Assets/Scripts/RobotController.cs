using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {

	float WalkAnimationSpeed = (1.2f + 1.5f) * 2f * 60f / 79f;
	CameraControl CameraControl;
	Rigidbody Rigidbody;

	void Start() {
		CameraControl = FindObjectOfType<CameraControl>();
		if (CameraControl == null) Debug.LogError("CameraControl not found");
		if (!TryGetComponent(out Rigidbody)) Debug.LogError("Rigidbody not found");
	}

	void FixedUpdate() {
		Vector3 moveForward = Vector3.ProjectOnPlane(CameraControl.transform.forward, Vector3.up).normalized;
		Vector3 moveRight = Vector3.ProjectOnPlane(CameraControl.transform.right, Vector3.up).normalized;
		Vector3 moveInput = Input.GetAxisRaw("Horizontal") * moveRight + Input.GetAxisRaw("Vertical") * moveForward;

		Vector3 targetVelocity = WalkAnimationSpeed * moveInput;
		Vector3 targetAcceleration = Vector3.ProjectOnPlane(targetVelocity - Rigidbody.velocity, Vector3.up);
		float speed01 = Rigidbody.velocity.magnitude / WalkAnimationSpeed;

		Rigidbody.AddForce(Vector3.ClampMagnitude(targetAcceleration, 2f * WalkAnimationSpeed * Time.fixedDeltaTime), ForceMode.VelocityChange);

		Vector3 rotate = Vector3.ClampMagnitude(Vector3.Cross(transform.forward, Rigidbody.velocity.normalized), 0.1f);
		transform.Rotate(rotate / Time.fixedDeltaTime);
		transform.eulerAngles = Vector3.Scale(transform.eulerAngles, Vector3.up);


		var animator = GetComponent<Animator>();
		animator.speed = speed01;
		animator.SetFloat("Speed", speed01);
	}

}