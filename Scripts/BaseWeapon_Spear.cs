using UnityEngine;
using System.Collections;

public class BaseWeapon_Spear : BaseWeapon {

	protected override void OnAwake()
	{
		//base.Awake();
		weapon = GameObject.Find("spear");
		damage = GameObject.Find("WeaponDamage").GetComponent<WeaponDamageContainer>().spear;
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
