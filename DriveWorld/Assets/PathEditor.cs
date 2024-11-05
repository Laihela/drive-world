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
			int c = 0;
			foreach (Transform child in transform) {

			}
		}

		PathMeshVertices = new();
		for (int c = 0; c < transform.childCount - 1; c++) {
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
		while (v < vertices.Count() - 1) {
			PathMeshVertices.Add(from.TransformPoint(vertices[v]));
			PathMeshVertices.Add(to.TransformPoint(vertices[v]));
			PathMeshVertices.Add(to.TransformPoint(vertices[v + 1]));
			PathMeshVertices.Add(from.TransformPoint(vertices[v + 1]));
			v++;
		}
		PathMeshVertices.Add(from.TransformPoint(vertices[v]));
		PathMeshVertices.Add(to.TransformPoint(vertices[v]));
		PathMeshVertices.Add(to.TransformPoint(vertices[0]));
		PathMeshVertices.Add(from.TransformPoint(vertices[0]));
	}

}