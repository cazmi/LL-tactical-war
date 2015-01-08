using UnityEngine;
using System.Collections;

public class BaseWeapon_Staff : BaseWeapon {

	protected override void OnAwake()
	{
		//base.Awake();
		//weaponDamage = GameObject.Find("WeaponDamage").GetComponent<WeaponDamageContainer>().staff;
	}

	// Use this for initialization
	new void Start () {
		base.Start();
		weaponLength = 5f;
		//damage = 5;
	}
	
	// Update is called once per frame
	void Update () {
		origin = transform.position;	
	}
}
