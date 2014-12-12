using UnityEngine;
using System.Collections;

public class Apprentice : BaseClass {

	public Apprentice()
	{
		ClassID = 1;
		ClassName = "Apprentice";
		ClassDescription = "Ranged character with powerful magic and low defense";
		ClassType = "Mage";

		BaseHP = 10;
		BaseAttack = 10;
		BaseDefense = 10;
				
		TileAttack = 2;

		/*plainMove = 2;
		forestMove = 1;
		desertMove = 3;
		mountainMove = 3;
		fortMove = 1;*/
	}
}
