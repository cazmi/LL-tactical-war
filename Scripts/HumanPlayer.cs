using UnityEngine;
using System.Collections;

public class HumanPlayer : Player {

	// Use this for initialization
	void Start () {
		body = transform.Find("Body");

		tileMove = 3;
		tileAttack = 1;
	}
	
	//Update is called once per frame
	void Update () {
		if(isTurnOver)
			body.renderer.material.color = Color.grey;
		else
			body.renderer.material.color = new Color(29f/255f, 0f/255f, 251f/255f);
	}
}
