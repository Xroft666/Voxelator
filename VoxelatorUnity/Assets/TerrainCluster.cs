using UnityEngine;
using System.Collections;

public class TerrainCluster : MonoBehaviour 
{
	private MeshFilter filter;
	private GRIDCELL[,,,] voxelsData;

	void Awake()
	{
		filter = gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		renderer.material = new Material(Shader.Find("Custom/ColoredDiffuse"));
	}

	public void SetMeshData(int[] indices, Vector3[] vertices, Color[] colors)
	{
		filter.mesh.vertices = vertices;
		filter.mesh.triangles = indices;
		filter.mesh.uv = new Vector2[vertices.Length];


		filter.mesh.colors = colors;
		filter.mesh.RecalculateNormals();
	}

	void OnDestroy()
	{
		if( filter.mesh != null )
			Destroy(filter.mesh);
	}

	void OnApplicationQuit()
	{
		if( filter.mesh != null )
			Destroy(filter.mesh);
	}
}
