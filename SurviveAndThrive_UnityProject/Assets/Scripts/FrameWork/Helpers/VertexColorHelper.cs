using UnityEngine;

[ExecuteInEditMode]
public class VertexColorHelper : MonoBehaviour {

	public Color color = Color.grey;

	void Update () {
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		Color[] colors = new Color[mesh.vertices.Length];

		int i = 0;
		while (i < mesh.vertices.Length) {
			colors[i] = color;
			i++;
		}
		mesh.colors = colors;
	}
}
