using UnityEngine;
using System.Collections;

public class BaseClass_Dragoon : BaseClass {

	public BaseClass_Dragoon()
	{
		ClassID = 4;
		ClassName = "Dragoon";
		ClassDescription = "Melee character with high attack and high defense";
		ClassType = "Cavalier";

		BaseHP = 10;
		BaseAttack = 10;
		BaseDefense = 10;

		TileAttack = 1;

		/*plainMove = 4;
		forestMove = 1;
		desertMove = 1;
		mountainMove = 1;
		fortMove = 3;*/
	}
}
