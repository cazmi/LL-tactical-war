﻿using UnityEngine;
using System.Collections;

public class BaseClass_Thief : BaseClass {

	public BaseClass_Thief()
	{
		ClassID = 6;
		ClassName = "Thief";
		ClassDescription = "Melee character with high agility and low defense";
		ClassType = "Bandit";

		BaseHP = 100;
		BaseAttack = 20;
		BaseDefense = 0;
		TotalUnits = 50;

		TileAttack = 1;

		/*plainMove = 2;
		forestMove = 3;
		desertMove = 3;
		mountainMove = 1;
		fortMove = 1;*/
	}

	public override void BoostStats()
	{
		BaseHP += 150;
		BaseAttack += 10;
	}
}
