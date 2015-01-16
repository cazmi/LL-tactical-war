using UnityEngine;
using System.Collections;

public class AuraThrust_Projectile : MonoBehaviour {

	Player caster;

	public void GetCaster(Player pCaster)
	{
		caster = pCaster;
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.tag != transform.tag)
		{
			Player hitPlayer = col.GetComponent<Player>();
			int damage = (caster.modifiedAttack + (caster.modifiedAttack * 1)) - hitPlayer.modifiedDefense;
			hitPlayer.TakeDamage(damage);
		}
	}
}
