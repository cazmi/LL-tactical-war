using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMapInfo {

	public Vector3 position;
	public bool reachable = true;
	public bool inRange = false;
	public int heuristic_H = 0;
	public int movementCost_G = 0;
	public int totalCost_F = 0;
	public int index = 0;

	public TileMapInfo parent = null;
	public TileMapInfo north = null;
	public TileMapInfo east = null;
	public TileMapInfo south = null;
	public TileMapInfo west = null;

		
	public void CalculateHeuristic(int destinationIndex)
	{
		int distCheckX = index % TileMap.instance.tileX;
		int distCheckZ = index / TileMap.instance.tileX;
		
		int distDestX = destinationIndex % TileMap.instance.tileX;
		int distDestZ = destinationIndex / TileMap.instance.tileX;
		
		heuristic_H = Mathf.Abs (distCheckX - distDestX) + Mathf.Abs (distCheckZ - distDestZ);
	}

	public void CalculateFValue()
	{
		totalCost_F = movementCost_G + heuristic_H;
	}
}
