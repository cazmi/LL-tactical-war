using UnityEngine;
using System.Collections;

public class WarCameraScript : MonoBehaviour {

	WarSceneManager wsc;
	public Vector3 camOffset;

	// Use this for initialization
	void Start () {
		wsc = WarSceneManager.instance;
	}
	
	// Update is called once per frame
	void Update () {		
		transform.position = wsc.playerGeneral.transform.position + camOffset;
	}
}
