using UnityEngine;
using System.Collections;

public class BaseClass_Dragoon : BaseClass {

	public BaseClass_Dragoon()
	{
		ClassID = 4;
		ClassName = "Dragoon";
		ClassDescription = "Melee character with high attack and high defense";
		ClassType = "Cavalier";

		BaseHP = 100;
		BaseAttack = 20;
		BaseDefense = 0;
		TotalUnits = 50;

		TileAttack = 1;

		/*plainMove = 4;
		forestMove = 1;
		desertMove = 1;
		mountainMove = 1;
		fortMove = 3;*/
	}
	
	public override void BoostStats()
	{
		BaseHP += 150;
		BaseAttack += 10;
	}

}
