using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public Transform Target = null;
	public float ViewDistance = 5f;
	public float VerticalOffset = 2f;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update() {

		Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		transform.Rotate(-mouseInput.y, mouseInput.x, -transform.eulerAngles.z);

		ViewDistance = Mathf.Clamp(ViewDistance * (1f - 0.1f * Input.mouseScrollDelta.y), 0f, 1000f);

		if (Target != null) {
			transform.position = Target.position - transform.forward * ViewDistance + transform.up * VerticalOffset;
		}
	}

}