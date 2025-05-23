using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarDrivetrainComponent : MonoBehaviour {

	abstract public void AddTorque(float torque);

	abstract public float GetSpeed();

}
