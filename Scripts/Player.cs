using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float moveSpeed = 10;
	public int tilePosition;
	public Vector3 destination;
	public bool isTurnOver;
		
	public bool moveEnabled;
	public bool attackEnabled;
	public bool waitEnabled;

	public BaseClass playerClass;

	public int currentHealth;
	public bool isAlive;

	protected virtual void Start()
	{
		isTurnOver = false;

		currentHealth = playerClass.BaseHP;
		isAlive = true;
	}

	public void Move(List<TileMapInfo> tmi)
	{
		if (Vector3.Distance(transform.position, tmi [tmi.Count-1].position) < 0.1f) 
		{	
			//print ("reached next destination");
			if(tmi.Count > 1)
			{
				if(tmi[tmi.Count-1].west == tmi[tmi.Count-2])
				{
					// rotate to left
					transform.rotation = Quaternion.Euler(0,270,0);
				}
				else if(tmi[tmi.Count-1].north == tmi[tmi.Count-2])
				{
					// rotate to up
					transform.rotation = Quaternion.Euler(0,0,0);
					
				}
				else if(tmi[tmi.Count-1].east == tmi[tmi.Count-2])
				{
					// rotate to right
					transform.rotation = Quaternion.Euler(0,90,0);
					
				}
				else if(tmi[tmi.Count-1].south == tmi[tmi.Count-2])
				{
					// rotate to down
					transform.rotation = Quaternion.Euler(0,180,0);
				}
			}
			tmi.Remove(tmi[tmi.Count-1]);
			destination = tmi [tmi.Count-1].position;
		}

		if (Vector3.Distance (transform.position, destination) > 0.1f) {
			transform.position += (destination - transform.position).normalized * moveSpeed * Time.deltaTime;
			/*Vector3 direction = destination - transform.position;	
			direction.Normalize ();

			transform.Translate (direction * moveSpeed * Time.deltaTime);*/
			if (Vector3.Distance (transform.position, destination) <= 0.1f) {
				transform.position = destination;
			}
		}
	}
	
	public void TakeDamage(int damage)
	{
		print ("I take " + damage + " damage");
		currentHealth -= damage;
		if(currentHealth <= 0)
		{
			// ... the enemy is dead.
			Dead ();
		}
	}

	public void Dead()
	{
		isAlive = false;
		collider.isTrigger = true;
		//rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
		//rigidbody.enabled = false;
	}
}
