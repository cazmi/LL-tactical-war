using UnityEngine;
using System.Collections;

public class AuraThrust_Projectile : MonoBehaviour {

	int damage;

	public void CalculateDamage(Player caster)
	{
		damage = caster.playerClass.BaseAttack + (caster.playerClass.BaseAttack * 1);
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.tag != transform.tag)
		{
			Player hitPlayer = col.GetComponent<Player>();
			hitPlayer.TakeDamage(damage);
		}
	}
}
