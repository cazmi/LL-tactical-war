using UnityEngine;
using System.Collections;

public class BaseClass_Archer : BaseClass {

	public BaseClass_Archer()
	{
		ClassID = 2;
		ClassName = "Archer";
		ClassDescription = "Ranged character with high agility and low defense";
		ClassType = "Archer";

		BaseHP = 10;
		BaseAttack = 10;
		BaseDefense = 10;

		TileAttack = 2;

		/*plainMove = 2;
		forestMove = 1;
		desertMove = 1;
		mountainMove = 3;
		fortMove = 3;*/
	}
}
