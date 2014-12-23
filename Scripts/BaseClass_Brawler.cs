using UnityEngine;
using System.Collections;

public class BaseClass_Brawler : BaseClass {

	public BaseClass_Brawler()
	{
		ClassID = 3;
		ClassName = "Brawler";
		ClassDescription = "Melee character with heavy weapons and high defense";
		ClassType = "Beastman";

		BaseHP = 10;
		BaseAttack = 10;
		BaseDefense = 10;

		TileAttack = 1;

		/*plainMove = 2;
		forestMove = 3;
		desertMove = 1;
		mountainMove = 1;
		fortMove = 3;*/
	}
}
