using UnityEngine;
using System.Collections;

public class TacticCameraScript : MonoBehaviour {

	GameObject pointer;

	public float speed = 0;
	float xAxisValue;
	float zAxisValue;

	void Start()
	{
		pointer = GameObject.Find("Pointer");
	}

	// Update is called once per frame
	void FixedUpdate () {

		transform.position = new Vector3(pointer.transform.position.x, transform.position.y, pointer.transform.position.z);
		/*if(Input.GetKey(KeyCode.W))
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
		if(Input.GetKey(KeyCode.S))
			transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
		if(Input.GetKey(KeyCode.A))
			transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
		if(Input.GetKey(KeyCode.D))
			transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
		*/
	}
}
