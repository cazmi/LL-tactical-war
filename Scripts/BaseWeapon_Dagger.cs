using UnityEngine;
using System.Collections;

public class BaseWeapon_Dagger : BaseWeapon {

	protected override void OnAwake()
	{
		//base.Awake();
		weapon = GameObject.Find("dagger");		
		damage = GameObject.Find("WeaponDamage").GetComponent<WeaponDamageContainer>().dagger;
	}
	
	// Use this for initialization
	new void Start () {
		base.Start();
		length = 2f;
		//damage = 5;
	}

	// Update is called once per frame
	void Update () {
		origin = weapon.transform.position - weapon.transform.forward;
	}
}
