using UnityEngine;
using System.Collections;

public enum Algorithm
{
	SPHERE,
	RANDOM,
	PERLIN3D,
	MOBRA,
	HEIGHTMAP
}

public class CustomTerrain : MonoBehaviour 
{
	public int clustersNumber = 5;
	public int voxelDensity = 50;
	public bool optimizeAlgorythm = false;
	public int terrainMinHeight = 0;
	public int terrainMaxHeight = 100;
	public float maxH = 20f;

	public Gradient gradient;
	public Algorithm algorithm = Algorithm.SPHERE;
	public int octaves = 3;
	public float frequency = 40f;
	public float amplitude = 1f;


	private PerlinNoise perlinNise;

	void Start()
	{
		perlinNise = new PerlinNoise(Random.Range(1,100));
		Vector3 clasterOffset = Vector3.zero;

		FillInVoxelsDataProcessor algoDelegate= FillASphere;
		switch(algorithm)
		{
		case Algorithm.RANDOM:
			algoDelegate = FillRandom;
			break;
		case Algorithm.PERLIN3D:
			algoDelegate = FillPerlinNoise;
			break;
		case Algorithm.MOBRA:
			algoDelegate = MobraNoise;
			break;
		case Algorithm.HEIGHTMAP:
			algoDelegate = FillHeightMap;
			break;
		}

		for( int i = 0; i < clustersNumber; i++ )
		{
			for( int j = 0; j < clustersNumber; j++ )
			{
				clasterOffset = new Vector3(i * voxelDensity, 0f, j * voxelDensity );

				GameObject chunkGO = new GameObject("cluster" + i + j);
				chunkGO.transform.parent = transform;
				
				TerrainCluster terrainChunk = chunkGO.AddComponent<TerrainCluster>();
				
				GRIDCELL[,,] grid;
				MarchingCubes.FillVoxelData(algoDelegate, clasterOffset, voxelDensity, out grid, maxH, algorithm);
				
				Vector3[] vertices;
				int[] indices;
				Color[] colors;
				MarchingCubes.GenerateChunk(AddColors, grid, optimizeAlgorythm, out indices, out vertices, out colors);
				terrainChunk.SetMeshData(indices, vertices, colors);

			}
		}
	}

	// Here, you can store filling data procedures for different cases

	float FillASphere(object[] parameters)
	{
		Vector3 pos = (Vector3) parameters[0];

		if (pos.y  < terrainMinHeight || pos.y  > terrainMaxHeight )  
			return 0;
		else
		// spere: x^2 + y^2 + z^2 = R 
			return 	(pos.x - voxelDensity / 2f) * (pos.x - voxelDensity / 2f) + 
					(pos.y - voxelDensity / 2f) * (pos.y - voxelDensity / 2f) + 
					(pos.z - voxelDensity / 2f) * (pos.z - voxelDensity / 2f) <= 
							voxelDensity * voxelDensity / 4f? 1f : -1f;
	}

	public Color AddColors(float height)
	{
		return gradient.Evaluate(height / maxH);

	}
	float FillRandom(object[] parameters)
	{

			return Random.Range(-1,1);;
	}

	float FillPerlinNoise(object[] parameters)
	{
		Vector3 pos = (Vector3) parameters[0] + (Vector3) parameters[1];

		if (pos.y  < terrainMinHeight || pos.y  > terrainMaxHeight )  
			return 0;
		else
			return perlinNise.FractalNoise3D(pos.x,pos.y,pos.z, 3, 40f, 1f);
	}

	float MobraNoise(object[] parameters)
	{
		Vector3 pos = (Vector3) parameters[0] + (Vector3) parameters[1];

		if (pos.y  < terrainMinHeight || pos.y  > terrainMaxHeight )  
			return 0;
		else
		{
			float gain = 1.0f;
			float sum = 0.0f;
			
			for(int i = 0; i < octaves; i++)
			{
				sum += perlinNise.Noise3D(pos.x * gain/frequency, pos.y * gain/frequency, pos.z * gain/frequency) * amplitude/gain;
				gain *= 2.0f;
			}
			return sum;
		}
	}

	float FillHeightMap(object[] parameters)
	{
		Vector3 pos = (Vector3) parameters[0];
		Vector3 pivot = (Vector3) parameters[1];

		float density = perlinNise.FractalNoise2D((pos + pivot).x, (pos + pivot).z, 3, 40f, 1f);

		if (pos.y  < terrainMinHeight )  
			return 0;
		if( pos.y  > terrainMaxHeight )
			return 0;

		return density;
	}
}
