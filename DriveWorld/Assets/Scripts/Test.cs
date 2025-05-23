using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
	private void FixedUpdate() {
		GetComponent<Rigidbody>().AddTorque(Vector3.right * 9.82f);
	}
}