using UnityEngine;
using System.Collections;

public class BaseClass_Fighter : BaseClass {

	public BaseClass_Fighter()
	{
		ClassID = 5;
		ClassName = "Fighter";
		ClassDescription = "Melee character with balanced status";
		ClassType = "Soldier";

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
