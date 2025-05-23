using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class DisguiseController : MonoBehaviour {

	[field:SerializeField]
	public RobotController Host { get; set; }

	[SerializeField]
	Transform Armature;
	[SerializeField]
	Transform HostArmature;

	void LateUpdate() {
		if (Host == null) return;
		transform.SetPositionAndRotation(Host.transform.transform.position, Host.transform.rotation);

		void GetDescendants(List<Transform> children, Transform root) {
			children.Add(root);
			foreach (Transform child in root) GetDescendants(children, child);
		}

		if (Armature == null || HostArmature == null) return;
		List<Transform> armatureBones = new();
		List<Transform> hostArmatureBones = new();
		GetDescendants(armatureBones, Armature);
		GetDescendants(hostArmatureBones, HostArmature);
		foreach (Transform bone in armatureBones) {
			var hostBone = hostArmatureBones.Find(hostBone => hostBone.name == bone.name);
			if (hostBone == null) continue;
			bone.SetPositionAndRotation(hostBone.position, hostBone.rotation);
		}
	}
}