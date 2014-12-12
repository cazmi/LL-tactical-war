using UnityEngine;
using System.Collections;

public class WarCameraScript : MonoBehaviour {

	public Transform charPrefab;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(charPrefab.position.x+10, 10, charPrefab.position.z - 35);
	}
}
