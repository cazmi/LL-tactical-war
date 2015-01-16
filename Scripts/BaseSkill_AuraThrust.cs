using UnityEngine;
using System.Collections;

public class BaseSkill_AuraThrust : BaseSkill {
	
	public BaseSkill_AuraThrust()
	{
		SkillID = 1;
		SkillName = "Aura Thrust";
		SkillDescription = "One straight line wave normal damage";
		SkillCooldown = 15;
	}

	public override void Cast(Player caster)
	{
		skillPrefab = (Transform)Resources.Load("Skill Prefabs/AuraThrust", typeof(Transform));
		Transform auraThrust = Instantiate(skillPrefab, transform.Find("CastPoint").position, skillPrefab.rotation) as Transform;
		Vector3 direction = caster.GetComponent<PlayerController>().targetDirection;
		auraThrust.rigidbody.velocity = new Vector3(25 * direction.x,0,0);
		auraThrust.tag = caster.tag;
		auraThrust.GetComponent<AuraThrust_Projectile>().GetCaster(caster);
	}
}
