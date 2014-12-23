using UnityEngine;
using System.Collections;

public class BaseClass : MonoBehaviour {

	int classID;
	string className;
	string classDescription;
	string classType;
	BaseWeapon classWeapon;

	int baseHP;
	float baseAttack;
	int baseDefense;

	int tileMove;
	int tileAttack;

	public int plainMove;
	public int forestMove;
	public int desertMove;
	public int mountainMove;
	public int fortMove;

	public float plainBonus;
	public float forestBonus;
	public float desertBonus;
	public float mountainBonus;
	public float fortBonus;

	public int ClassID {
		get {
			return classID;
		}
		set {
			classID = value;
		}
	}

	public string ClassName {
		get {
			return className;
		}
		set {
			className = value;
		}
	}

	public string ClassDescription {
		get {
			return classDescription;
		}
		set {
			classDescription = value;
		}
	}

	public string ClassType {
		get {
			return classType;
		}
		set {
			classType = value;
		}
	}

	public BaseWeapon ClassWeapon {
		get {
			return classWeapon;
		}
		set {
			classWeapon = value;
		}
	}

	public int BaseHP {
		get {
			return baseHP;
		}
		set {
			baseHP = value;
		}
	}

	public float BaseAttack {
		get {
			return baseAttack;
		}
		set {
			baseAttack = value;
		}
	}

	public int BaseDefense {
		get {
			return baseDefense;
		}
		set {
			baseDefense = value;
		}
	}

	public int TileMove {
		get {
			return tileMove;
		}
		set {
			tileMove = value;
		}
	}

	public int TileAttack {
		get {
			return tileAttack;
		}
		set {
			tileAttack = value;
		}
	}

	public void TerrainEffect(TileMap.TerrainType terrain)
	{
		if(terrain == TileMap.TerrainType.Plain)
		{
			baseAttack += baseAttack * plainBonus;
			tileMove = plainMove;
		}
		else if(terrain == TileMap.TerrainType.Forest)
		{
			baseAttack += baseAttack * forestBonus;
			tileMove = forestMove;
		}
		else if(terrain == TileMap.TerrainType.Desert)
		{
			baseAttack += baseAttack * desertBonus;
			tileMove = desertMove;
		}
		else if(terrain == TileMap.TerrainType.Mountain)
		{
			baseAttack += baseAttack * mountainBonus;
			tileMove = mountainMove;
		}
		else if(terrain == TileMap.TerrainType.Fort)
		{
			baseAttack += baseAttack * fortBonus;
			tileMove = fortMove;
		}
	}
}
