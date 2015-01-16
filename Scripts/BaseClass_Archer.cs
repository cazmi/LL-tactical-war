using UnityEngine;
using System.Collections;

public class BaseClass_Archer : BaseClass {

	public BaseClass_Archer()
	{
		ClassID = 2;
		ClassName = "Archer";
		ClassDescription = "Ranged character with high agility and low defense";
		ClassType = "Archer";

		BaseHP = 100;
		BaseAttack = 20;
		BaseDefense = 0;
		TotalUnits = 50;

		TileAttack = 2;

		/*plainMove = 2;
		forestMove = 1;
		desertMove = 1;
		mountainMove = 3;
		fortMove = 3;*/
	}
	
	public override void BoostStats()
	{
		BaseHP += 150;
		BaseAttack += 10;
	}

}
