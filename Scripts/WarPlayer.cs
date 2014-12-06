using UnityEngine;
using System.Collections;

public class WarPlayer : MonoBehaviour {

	Vector3 destination;
	float moveSpeed = 10;

	// Use this for initialization
	void Start () {
		destination = new Vector3(transform.position.x - 40, transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine("Advance");
	}
	
	IEnumerator Advance()
	{
		yield return new WaitForSeconds(1);
		
		if (Vector3.Distance (transform.position, destination) > 0.1f) {
			transform.position += (destination - transform.position).normalized * moveSpeed * Time.deltaTime;
			
			if (Vector3.Distance (transform.position, destination) <= 0.1f) {
				transform.position = destination;
			}
		}
	}
}
