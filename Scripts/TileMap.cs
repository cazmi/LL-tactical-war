using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {

	public static TileMap instance;

	public List<TileMapInfo> tiles = new List<TileMapInfo> ();
	public List<TileMapInfo> inRangeTiles = new List<TileMapInfo>();
	public enum TerrainType
	{
		Plain, 
		Forest,
		Desert,
		Mountain,
		Fort
	}
	public TerrainType terrainType;

	MeshFilter meshFilter;
	MeshRenderer meshRenderer;
	MeshCollider meshCollider;

	public int tileX = 0;
	public int tileZ = 0;
	public float tileSize = 1f;

	int vertsSizeX;
	int vertsSizeZ;

	Vector3[] vertices;		
	Vector3[] normals;
	Vector2[] uv;						// uv map to assign texture

	Texture2D texture;

	void Awake ()
	{
		instance = this;
		
		meshFilter = GetComponent<MeshFilter> ();
		meshRenderer = GetComponent<MeshRenderer> ();
		meshCollider = GetComponent<MeshCollider> ();
	}

	void Update ()
	{
	
	}

	// Use this for initialization
	void Start () 
	{
		BuildMesh ();
	}
	
	void BuildMesh () 
	{
		// a tile has 1 extra vertex of width and height 
		vertsSizeX = tileX + 1;
		vertsSizeZ = tileZ + 1;
		
		int numTiles = tileX * tileZ;					// total tiles 
		int numVerts = vertsSizeX * vertsSizeZ;			// total vertices
		int numTris = numTiles * 2;						// total triangles, each tile is made of 2 triangles
		int[] trianglePoints = new int[numTris * 3];	// each triangle has 3 base constructor

		vertices = new Vector3[numVerts];		
		normals = new Vector3[numVerts];
		uv = new Vector2[numVerts];

		int x = 0;
		int z = 0;

		// create vertices
		for(z = 0; z < vertsSizeZ; z++)
		{
			for(x = 0; x < vertsSizeX; x++)
			{
				int vertsIndex = z * vertsSizeX + x;
				vertices[vertsIndex] = new Vector3( x * tileSize, 0, z * tileSize);
				//Debug.DrawRay(vertices[(z * tile_z) + x], Vector3.up);
				normals[vertsIndex] = Vector3.up;
				uv[vertsIndex] = new Vector2((float)x / vertsSizeX, (float)z / vertsSizeZ);
			}
		}
	
		// create triangles
		for(z = 0; z < tileZ; z++)
		{
			for(x = 0; x < tileX; x++)
			{
				int tileIndex = z * tileX + x;
				int triOffset = tileIndex * 6;

				trianglePoints[triOffset + 0] = z * vertsSizeX + x;
				trianglePoints[triOffset + 1] = z * vertsSizeX + x + vertsSizeX;
				trianglePoints[triOffset + 2] = z * vertsSizeX + x + 1;

				trianglePoints[triOffset + 3] = z * vertsSizeX + x + 1;
				trianglePoints[triOffset + 4] = z * vertsSizeX + x + vertsSizeX;
				trianglePoints[triOffset + 5] = z * vertsSizeX + x + vertsSizeX + 1;
			}
		}
		
		
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = trianglePoints;
		mesh.normals = normals;
		mesh.uv = uv;
		
		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;

		for(int i = 0; i < numTiles; i++)
		{
			TileMapInfo tile = new TileMapInfo();
			tiles.Add(tile);
		}

		GenerateTileInfo();
		DrawTransparentTexture ();
	}

	public void GenerateTileInfo()
	{
		for(int z = 0; z < tileZ; z++)
		{
			for(int x = 0; x < tileX; x++)
			{
				int tileIndex = z * tileX + x;
				tiles[tileIndex].position = TileToWorldPoint(tileIndex);
				tiles[tileIndex].index = tileIndex;
				
				if(z == tileZ - 1) tiles[tileIndex].north = null; else tiles[tileIndex].north = tiles[tileIndex + tileX];
				if(x == tileX - 1) tiles[tileIndex].east = null; else tiles[tileIndex].east = tiles[tileIndex + 1];
				if(z == tileZ - tileZ) tiles[tileIndex].south = null; else tiles[tileIndex].south = tiles[tileIndex - tileX];
				if(x == tileX - tileX) tiles[tileIndex].west = null; else tiles[tileIndex].west = tiles[tileIndex - 1]; 
			}
		}
	}

	public void DrawTransparentTexture()
	{
		int textWidth = tileX + 1;
		int textHeight = tileZ + 1;
		Color highlightColor = Color.blue;
		
		texture = new Texture2D (textWidth, textHeight);
		
		for (int y=0; y<textHeight; y++) 
		{
			for (int x=0; x<textWidth; x++)
			{
				highlightColor.a = 0f;
				texture.SetPixel(x, y, highlightColor);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.Apply ();
		meshRenderer.sharedMaterial.mainTexture = texture;
		
		Color color = meshRenderer.material.color;
		color.a = 1f;
		meshRenderer.material.color = color;
	}

	public void HighlightTiles(int index, Color highlightColor)
	{		
		int pixelX = index - (tileX * (index / tileX));
		int pixelY = index / tileX;
		
		highlightColor.a = 0.5f;
		texture.SetPixel (pixelX, pixelY, highlightColor);
		
		texture.filterMode = FilterMode.Point;
		texture.Apply ();
		meshRenderer.sharedMaterial.mainTexture = texture;
	}

	public void DetermineAvailableTiles(int playerTileIndex, int totalTile)
	{
		int i = 0;
		int row = (totalTile * 2) + 1;
		int j = 0;
		int column = 1;
		int firstColumnTile = 0;
		int nextTile = playerTileIndex;
		
		for(int k=0; k<totalTile; k++)
		{
			nextTile = tiles[nextTile].north.index;
		}
				
		while(i < row)
		{
			j=0;

			if(i <= totalTile)
			{			
				// first column of current row is the last tile of previous row subtracted by previous total column
				firstColumnTile = nextTile - column;		
				column = i * 2 + 1;
			}
			else
			{
				column = column - 2;
				// first column of current row is the last tile of previous row subtracted by current total column
				firstColumnTile = nextTile - column;
			}
						
			while(j < column)
			{	
				if(j == 0 && i > 0)
				{
					nextTile = tiles[firstColumnTile].south.index;
				}
				else if(j > 0 && i > 0)
				{
					nextTile = tiles[nextTile].east.index;
				}

				tiles[nextTile].inRange = true;
				inRangeTiles.Add(tiles[nextTile]);
				HighlightTiles(nextTile, Color.blue);

				j++;
			}

			i++;
		}
	}

	/*int nextAvailableTile(int currentTile, int currentColumn, int maxColumn)
	{	
		if(currentColumn == maxColumn - 1)
		{
			return tiles[currentTile-maxColumn].south.index;
		}
		else
		{
			return tiles[currentTile].east.index;
		}
	}*/

	Vector3 TileToWorldPoint(int index)
	{
		int vertex1_index = tileX + (vertsSizeX * (index / tileX)) - (tileX * ((index / tileX) + 1) - index);
		int vertex2_index = vertex1_index + vertsSizeX + 1;

		return new Vector3(
			(vertices [vertex1_index].x + vertices [vertex2_index].x)/2,
				0f,
			(vertices [vertex1_index].z + vertices [vertex2_index].z)/2
			);
	}

	public int WorldToTilePoint(Vector3 location)
	{
		int x_index = Mathf.FloorToInt (location.x / tileSize);
		int z_index = Mathf.FloorToInt (location.z / tileSize);

		return x_index + (tileX * z_index);
	}
}
