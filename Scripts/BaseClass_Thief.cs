using UnityEngine;
using System.Collections;

public class BaseClass_Thief : BaseClass {

	public BaseClass_Thief()
	{
		ClassID = 6;
		ClassName = "Thief";
		ClassDescription = "Melee character with high agility and low defense";
		ClassType = "Bandit";
		//ClassWeapon = gameObject.AddComponent<BaseWeapon_Dagger>();

		BaseHP = 10;
		BaseAttack = 10;
		BaseDefense = 10;

		TileAttack = 1;

		/*plainMove = 2;
		forestMove = 3;
		desertMove = 3;
		mountainMove = 1;
		fortMove = 1;*/
	}
}
