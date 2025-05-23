using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class PathEditor : MonoBehaviour {

	public Mesh PathMesh;
	public Material PathMaterial = null;

	public string Status = "Normal";
	public bool AutoRotate = false;
	public bool Loop = false;
	public bool Redraw = false;

	MeshFilter MeshFilter = null;
	MeshRenderer MeshRenderer = null;
	List<Vector3> PathMeshVertices = new();

	void Start() {
		if (!TryGetComponent(out MeshFilter)) MeshFilter = gameObject.AddComponent<MeshFilter>();
		if (!TryGetComponent(out MeshRenderer)) MeshRenderer = gameObject.AddComponent<MeshRenderer>();
		if (MeshFilter.sharedMesh == null) {
			MeshFilter.sharedMesh = new Mesh();
		}
		MeshRenderer.sharedMaterial = PathMaterial;
	}

	void Update() {
		if (!Redraw) return;
		Redraw = false;

		if (PathMesh.vertexCount < 3) {
			Status = "At least 3 vertices required for path";
			return;
		}

		if (transform.childCount < 2) {
			Status = "At least two points required for path";
			return;
		}

		if (AutoRotate) {
			int i = 0;
			foreach (Transform child in transform) {
				Vector3 a = Vector3.zero;
				if (i == 0) a = transform.GetChild(transform.childCount - 1).localPosition;
				else a = transform.GetChild(i - 1).localPosition;
				Vector3 b = Vector3.zero;
				if (i == transform.childCount - 1) b = transform.GetChild(0).localPosition;
				else b = transform.GetChild(i + 1).localPosition;
				child.localRotation = Quaternion.LookRotation(a - b, transform.up);
				i++;
			}
		}

		PathMeshVertices = new();
		for (int c = 0; c < transform.childCount - 2; c++) {
			AddSegment(transform.GetChild(c), transform.GetChild(c + 1));
		}

		MeshFilter.sharedMesh.SetVertices(PathMeshVertices);
		MeshFilter.sharedMesh.SetIndices(Enumerable.Range(0, PathMeshVertices.Count).ToArray(), MeshTopology.Quads, 0);
		MeshFilter.sharedMesh.RecalculateNormals();

		Status = $"Created mesh with {PathMeshVertices.Count} vertices";
	}
	void AddSegment(Transform from, Transform to) {
		int v = 0;
		Vector3[] vertices = PathMesh.vertices;
		void addFromVertex(int index) => PathMeshVertices.Add(from.localPosition + from.localRotation * vertices[index]);
		void addToVertex(int index) => PathMeshVertices.Add(to.localPosition + to.localRotation * vertices[index]);
		while (v < vertices.Count() - 1) {
			addFromVertex(v);
			addToVertex(v);
			addToVertex(v + 1);
			addFromVertex(v + 1);
			v++;
		}
		addFromVertex(v);
		addToVertex(v);
		addToVertex(0);
		addFromVertex(0);
	}

}