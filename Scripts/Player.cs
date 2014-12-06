using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float moveSpeed = 10.0f;
	public int tilePosition;
	public Vector3 destination;
	public int tileMove;
	public int tileAttack;
	public bool isTurnOver = false;

	
	public bool moveEnabled = true;
	public bool attackEnabled = true;
	public bool waitEnabled = true;

	protected Transform body;

	void Update()
	{

	}

	public void Move(List<TileMapInfo> tmi)
	{
		if (transform.position == tmi [tmi.Count-1].position) 
		{	
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
}
