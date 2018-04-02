using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstructiveSolidGeometry;

public class ExtrudePlane : MonoBehaviour {

	public Mesh sourceMesh;
	public MeshExtrusion.Edge[] precomputedEdges;

	public GameObject cubeCutter;

	// Use this for initialization
	void Start () {

		// Getting the mesh of THIS object (the plane).
		this.sourceMesh = GetComponent<MeshFilter>().mesh;
		this.precomputedEdges = MeshExtrusion.BuildManifoldEdges(this.sourceMesh);

		// Defining transformations to extrude the mesh.
		Matrix4x4[] sections = new Matrix4x4[2];

		// Starting point (where the mesh already is, but its needed so the new mesh does not generate with a hole).
		sections[0] = this.transform.localToWorldMatrix;
		// The end point.
		sections[1] = this.transform.worldToLocalMatrix * Matrix4x4.TRS(new Vector3(0, 1, 1), Quaternion.Euler(0, 45, 0), new Vector3(0.75f, 2, 1));

		// Get the mesh again (the script need a different reference to the same mesh for some reason?).
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		// Actually perform the extrusion, the parameter [invertFaces] is extremely important in this case but its kind of hard to programatically tell when it is needed.
		// I would guess you would need to somehow see the direction of the normals before even creating the face.
		MeshExtrusion.ExtrudeMesh(this.sourceMesh, mesh, sections, true);

		// Preparing CSG objects for substraction.
		// Again, passing a new refference to the same mesh is important for some reason.
		CSG plane = CSG.fromMesh(GetComponent<MeshFilter>().mesh, this.transform);
		CSG cube = CSG.fromMesh(this.cubeCutter.GetComponent<MeshFilter>().mesh, this.cubeCutter.transform);

		// Save the operation, this does not affect neither meshes yet.
		CSG result = plane.subtract(cube);

		// Replace the mesh of THIS object with the mesh representation of the operation.
		this.GetComponent<MeshFilter>().mesh = result.toMesh();

		// Hide the cube to visualize the result.
		this.cubeCutter.SetActive(false);
	}
}
