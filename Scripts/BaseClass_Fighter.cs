using UnityEngine;
using System.Collections;

public class BaseClass_Fighter : BaseClass {

	public BaseClass_Fighter()
	{
		ClassID = 5;
		ClassName = "Fighter";
		ClassDescription = "Melee character with balanced status";
		ClassType = "Soldier";

		BaseHP = 100;
		BaseAttack = 20;
		BaseDefense = 0;
		TotalUnits = 50;

		TileAttack = 1;

		/*plainMove = 2;
		forestMove = 3;
		desertMove = 1;
		mountainMove = 1;
		fortMove = 3;*/
	}
	
	public override void BoostStats()
	{
		BaseHP += 150;
		BaseAttack += 10;
	}

	void Awake()
	{
		print ("Fighter class awaken");
		classWeapon = gameObject.GetComponentInChildren<BaseWeapon_Dagger>();
		classSkill = gameObject.GetComponent<BaseSkill_AuraThrust>();
	}
}
