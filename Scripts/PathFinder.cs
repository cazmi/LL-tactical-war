using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {

	List<TileMapInfo> openList = new List<TileMapInfo> ();
	List<TileMapInfo> closedList = new List<TileMapInfo> ();
	List<TileMapInfo> pathList = new List<TileMapInfo> ();

	TileMap tm;
	TileMapInfo destinationTile = null;

	bool targetFound = false;
	int baseMovementCost = 1;

	void Awake()
	{
		tm = TileMap.instance;
	}

	public void FindPath(TileMapInfo currentTile, TileMapInfo destTile)
	{
		destinationTile = destTile;
		print ("current tile :" + currentTile.index);
		print ("destination :" + destinationTile.index);
		ResetPathInfo();

		if(destinationTile.reachable == false)
			return;

		while (targetFound == false)
		{
			if(currentTile == null)
			{
				destinationTile.reachable = false;
				break;
			}

			if (currentTile.north != null) 
			{
				DetermineValue(currentTile, currentTile.north);
			}
			if (currentTile.east != null) 
			{
				DetermineValue(currentTile, currentTile.east);
			}
			if (currentTile.south != null) 
			{
				DetermineValue(currentTile, currentTile.south);
			}
			if (currentTile.west != null) 
			{
				DetermineValue(currentTile, currentTile.west);
			}

			
			if (targetFound == true) 
			{
				TraceBackPath();		
				break;
			}
			
			openList.Remove(currentTile);
			closedList.Add(currentTile);

			currentTile = GetSmallestFValue();
		}
	}

	void DetermineValue(TileMapInfo currentTile, TileMapInfo adjacentTile)
	{
		if (adjacentTile == null || !adjacentTile.reachable || !adjacentTile.inRange)
				return;

		if (adjacentTile == destinationTile) {
				destinationTile.parent = currentTile;
				targetFound = true;
				return;
		}

		if (closedList.Contains (adjacentTile) == false) 
		{
			if(openList.Contains(adjacentTile) == true)
			{
				// do some check
				int newGCost = currentTile.movementCost_G + baseMovementCost;
				if(newGCost < adjacentTile.movementCost_G)
				{
					adjacentTile.movementCost_G = newGCost;
					adjacentTile.parent = currentTile;
					adjacentTile.CalculateFValue();
				}
			}
			else
			{
				adjacentTile.parent = currentTile;
				adjacentTile.movementCost_G = currentTile.movementCost_G + baseMovementCost;
				adjacentTile.CalculateHeuristic(destinationTile.index);
				adjacentTile.CalculateFValue();
				openList.Add(adjacentTile);
			}
		}
	}

	TileMapInfo GetSmallestFValue()
	{
		int[] temp = new int[openList.Count];
		for (int i=0; i<openList.Count; i++) 
		{
			temp[i] = openList[i].totalCost_F;
			//print ("tile : " + openList[i].index + ", cost : " + temp[i]);
		}
		int minFValue = Mathf.Min(temp);
		return openList.Find(tile => tile.totalCost_F == minFValue);
	}

	void TraceBackPath()
	{
		TileMapInfo tile = destinationTile;
		while (tile != null) 
		{
			tm.HighlightTiles(tile.index, Color.red);
			pathList.Add(tile);
			tile = tile.parent;
		}
	}

	void ResetPathInfo ()
	{
		targetFound = false;

		for (int i = 0; i < openList.Count; i++) {
			openList[i].parent = null;
		}

		for (int i = 0; i < closedList.Count; i++) {
			
			closedList[i].parent = null;
		}

		for (int i = 0; i < pathList.Count; i++) {
			pathList[i].parent = null;
		}

		openList.Clear();
		closedList.Clear();
		pathList.Clear();
	}

	public List<TileMapInfo> GetPathList()
	{
		return pathList;
	}
}
