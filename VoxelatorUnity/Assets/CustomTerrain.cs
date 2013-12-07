using UnityEngine;
using System.Collections;

public class CustomTerrain : MonoBehaviour 
{
	public int clustersDimension;

	public int clusterSize = 5;
	public float gridScale = 1f;
	public bool optimizeAlgorythm = false;
	public int terrainMinHeight = 0;
	public int terrainMaxHeight = 100;

	public int octaves = 3;
	public float frequency = 40f;
	public float amplitude = 1f;

	public Color32[] terrainColor = new Color32[]{
		new Color32(0, 0, 128, 1),
		new Color32(0, 128, 255, 1 ), 
		new Color32(240, 240, 64, 1),
		new Color32(32, 160, 0, 1),
		new Color32(224, 224, 0, 1),
		new Color32(128, 128, 128, 1),
		new Color32(255, 255, 255, 1)
	};

	private SimplexNoise3D simpleNoise;
	private PerlinNoise perlinNise;

	void Start()
	{
		perlinNise = new PerlinNoise(1);
		simpleNoise = new SimplexNoise3D();
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
				MarchingCubes.FillVoxelData(Fill2DNoise, clasterOffset, clusterSize, gridScale, out grid);
				MarchingCubes.ColorProcessor(AddColors);
				
				Vector3[] vertices;
				int[] indices;
				Color32[] colors;
				MarchingCubes.GenerateChunk(grid, optimizeAlgorythm, out indices, out vertices, out colors);
				terrainChunk.SetMeshData(indices, vertices, colors);
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

	public Color32 AddColors(float height)
	{
		if (height == terrainMinHeight)
			return terrainColor[0];

		else if (height > terrainMinHeight && height < (terrainMaxHeight - terrainMinHeight) / 100*10)
			return terrainColor[1];

		else if (height > (terrainMaxHeight - terrainMinHeight) / 100*10 && height < (terrainMaxHeight - terrainMinHeight) / 100*25)
			return terrainColor[2];

		else if (height > (terrainMaxHeight - terrainMinHeight) / 100*25 && height < (terrainMaxHeight - terrainMinHeight) / 100*50)
			return terrainColor[3];

		else if (height > (terrainMaxHeight - terrainMinHeight) / 100*50 && height < (terrainMaxHeight - terrainMinHeight) / 100*70)
			return terrainColor[4];

		else if (height > (terrainMaxHeight - terrainMinHeight) / 100*70 && height < (terrainMaxHeight - terrainMinHeight) / 100*90)
			return terrainColor[5];

		else if (height > (terrainMaxHeight - terrainMinHeight) / 100*90 && height < terrainMaxHeight)
			return terrainColor[6];

		else
			return new Color32(0,0,0,0);
	


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

		if (y < terrainMinHeight)
			return 1f;
		else if (y > terrainMaxHeight)
			return -1;
		else
			return perlinNise.FractalNoise3D(x,y,z, 3, 40f, 1f);
	}

	float FillSimpleNoise(object[] parameters)
	{
		float x = (float) parameters[0];
		float y = (float) parameters[1];
		float z = (float) parameters[2];
		
		return simpleNoise.CoherentNoise(x,y,z, 3, 40, 1f, 2f, 0.9f);
	}

	float MobraNoise(object[] parameters)
	{
		float x = (float) parameters[0];
		float y = (float) parameters[1];
		float z = (float) parameters[2];



		if (y < terrainMinHeight)
			return 1f;
		else if (y > terrainMaxHeight)
			return -1;
		else
		{
			float gain = 1.0f;
			float sum = 0.0f;
			
			for(int i = 0; i < octaves; i++)
			{
				sum += perlinNise.Noise3D(x*gain/frequency, y*gain/frequency, z*gain/frequency) * amplitude/gain;
				gain *= 2.0f;
			}
			return sum;
		}
	}

	float Fill2DNoise(object[] parameters)
	{
		float x = (float) parameters[0];
		float y = (float) parameters[1];
		float z = (float) parameters[2];
		
		return perlinNise.FractalNoise2D(x,z, 3, 40f, 1f)/* * perlinNise.FractalNoise3D(x,y,z, 5, 20f, 0.5f)*/;
	}
}
