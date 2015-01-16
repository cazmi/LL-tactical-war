using UnityEngine;
using System.Collections;

public class BaseWeapon : MonoBehaviour {
	
	protected GameObject weapon;	
	protected int killableMask;

	protected float weaponLength;
	protected int weaponDamage;

	protected bool inHitFrame;
	ArrayList hitActors = new ArrayList();

	protected Vector3 origin;
	protected Vector3 direction;
	Ray meleeRay;
	RaycastHit hitInfo;

	protected virtual void OnAwake(){}

	void Awake()
	{
		//print ("*** BaseWeapon Class ***");
		killableMask = LayerMask.GetMask("Killable");
		OnAwake();
	}

	// Use this for initialization
	protected void Start () {
		inHitFrame = false;
	}


	public void TraceAttack(Player hitter)
	{
		direction = transform.forward;

		Debug.DrawLine(origin, origin + direction *  weaponLength); 

		if(Physics.Raycast(origin, direction, out hitInfo, weaponLength, killableMask))
		{
			Player hitPlayer = hitInfo.transform.GetComponent<Player>();
			if(hitPlayer.gameObject.tag != hitter.gameObject.tag && hitPlayer.isAlive && inHitFrame && AddToHitActor(hitPlayer))
			{
				hitPlayer.TakeDamage(hitter.modifiedAttack - hitPlayer.modifiedDefense);
			}
		}
	}
	
	public bool ToggleHitFrame()
	{
		ResetHitActorsList();
		return inHitFrame = !inHitFrame;
	}
	
	bool AddToHitActor(Player hitActor)
	{
		for (int i=0; i<hitActors.Count; i++)
		{
			if(hitActors[i] == hitActor)
				return false;
		}
		
		hitActors.Add(hitActor);
		return true;
	}

	public void ResetHitActorsList()
	{
		hitActors.Clear();
	}
}
