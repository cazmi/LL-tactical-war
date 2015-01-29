using UnityEngine;
using System.Collections;

public class BaseClass_Apprentice : BaseClass {

	public BaseClass_Apprentice()
	{
		ClassID = 1;
		ClassName = "Apprentice";
		ClassDescription = "Ranged character with powerful magic and low defense";
		ClassType = "Mage";

		BaseHP = 100;
		BaseAttack = 20;
		BaseDefense = 0;
		TotalUnits = 100;
				
		TileAttack = 2;

		/*plainMove = 2;
		forestMove = 1;
		desertMove = 3;
		mountainMove = 3;
		fortMove = 1;*/
	}
	
	public override void BoostStats()
	{
		BaseHP += 150;
		BaseAttack += 10;
	}

	void Awake()
	{
		classWeapon = gameObject.GetComponentInChildren<BaseWeapon_Staff>();
		//classSkill = gameObject.AddComponent<BaseSkill_AuraThrust>();
	}

}
