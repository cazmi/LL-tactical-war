using UnityEngine;
using System.Collections;

public class BaseWeapon : MonoBehaviour {
	
	protected GameObject weapon;	
	protected int killableMask;

	protected float length;
	protected int damage;

	protected bool inHitFrame;
	ArrayList hitActors = new ArrayList();

	protected Vector3 origin;
	protected Vector3 direction;
	Ray meleeRay;
	RaycastHit hitInfo;

	protected virtual void OnAwake(){}

	void Awake()
	{
		print ("*** BaseWeapon Class ***");
		killableMask = LayerMask.GetMask("Killable");
		OnAwake();
	}

	// Use this for initialization
	protected void Start () {
		inHitFrame = false;
	}

	public void TraceAttack(Player hitter)
	{
		direction = weapon.transform.forward;

		Debug.DrawLine(origin, origin + direction *  length); 

		if(Physics.Raycast(origin, direction, out hitInfo, length, killableMask))
		{
			Player hitPlayer = hitInfo.transform.GetComponent<Player>();
			if(hitPlayer.gameObject.tag != hitter.gameObject.tag && hitPlayer.isAlive && inHitFrame && AddToHitActor(hitPlayer))
			{
				hitPlayer.TakeDamage(damage);
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
