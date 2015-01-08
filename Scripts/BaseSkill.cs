using UnityEngine;
using System.Collections;

abstract public class BaseSkill : MonoBehaviour {

	protected Transform skillPrefab;

	int skillID;
	string skillName;
	string skillDescription;
	float skillCooldown;

	public int SkillID { 
				get; 
				set; 
	}
	public string SkillName { 
				get; 
				set; 
	}
	public string SkillDescription {
				get;
				set;
	}
	public float SkillCooldown {
				get;
				set;
	}

	abstract public void Cast(Player player);
}
