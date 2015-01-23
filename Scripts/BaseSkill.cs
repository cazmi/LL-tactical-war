using UnityEngine;
using System.Collections;

abstract public class BaseSkill : MonoBehaviour {

	protected Transform skillPrefab;

	int skillID;
	string skillName;
	string skillDescription;
	float skillCooldown;

	public int SkillID { 
		get { return skillID;}
		set { skillID = value; } 
	}
	public string SkillName { 
		get { return skillName;}
		set { skillName = value; } 
	}
	public string SkillDescription {
		get { return skillDescription;}
		set { skillDescription = value; } 
	}
	public float SkillCooldown {
		get { return skillCooldown;}
		set { skillCooldown = value; } 
	}

	abstract public void Cast(Player player, Vector3 direction);
}
