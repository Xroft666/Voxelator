using UnityEngine;
using System.Collections;

public class CustomTerrain : MonoBehaviour 
{
	public int clustersDimension;

	public int clusterSize = 5;
	public float gridScale = 1f;
	public bool optimizeAlgorythm = false;


	private PerlinNoise perlinNise;

	void Start()
	{
		perlinNise = new PerlinNoise(1);

		Vector3 clasterOffset = Vector3.zero;

		for( int i = 0; i < clustersDimension; i++ )
		{
			for( int j = 0; j < clustersDimension; j++ )
			{
				clasterOffset = new Vector3(i * clusterSize, 0f, j * clusterSize );

				GameObject chunkGO = new GameObject("cluster" + i + j);
				chunkGO.transform.parent = transform;
				
				TerrainCluster terrainChunk = chunkGO.AddComponent<TerrainCluster>();
				
				GRIDCELL[,,] grid;
				MarchingCubes.FillVoxelData(FillPerlinNoise, clasterOffset, clusterSize, gridScale, out grid);
				
				Vector3[] vertices;
				int[] indices;
				MarchingCubes.GenerateChunk(grid, optimizeAlgorythm, out indices, out vertices);
				
				terrainChunk.SetMeshData(indices, vertices);
			}
		}
	}

	// Here, you can store filling data procedures for different cases

	float FillASphere(object[] parameters)
	{
		float x = (float) parameters[0];
		float y = (float) parameters[1];
		float z = (float) parameters[2];

		// spere: x^2 + y^2 + z^2 = R 

		return 	(x - clusterSize / 2f) * (x - clusterSize / 2f) + 
				(y - clusterSize / 2f) * (y - clusterSize / 2f) + 
				(z - clusterSize / 2f) * (z - clusterSize / 2f) <= clusterSize * clusterSize / 4f? 1f : -1f;
	}

	float FillRandom(object[] parameters)
	{
		return Random.value;
	}

	float FillPerlinNoise(object[] parameters)
	{
		float x = (float) parameters[0];
		float y = (float) parameters[1];
		float z = (float) parameters[2];

		return perlinNise.FractalNoise3D(x,y,z, 3, 40f, 1f);
	}
}
