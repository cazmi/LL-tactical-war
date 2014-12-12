using UnityEngine;
using System.Collections;	

public class PlayerController : MonoBehaviour {

	Animator anim;
	public float speedDampTime = 0.1f;	
	public float turnSmoothing = 15f;	// A smoothing value for turning the player.

	void Awake()
	{		
		anim = GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {
	}


	void FixedUpdate()
	{		
		float h = Input.GetAxis("Horizontal");
		MoveManagement(h);
	}

	void Update()
	{
		bool attack = Input.GetMouseButtonDown(0);
		anim.SetBool("Attacking", attack);
	}

	void MoveManagement(float horizontal)
	{
		if(horizontal != 0)
		{
			Rotating(horizontal);
			rigidbody.velocity = new Vector3(horizontal*10, rigidbody.velocity.y, rigidbody.velocity.z);
			anim.SetFloat("Speed", 5f, speedDampTime, Time.deltaTime);
		}
		else 
			anim.SetFloat("Speed", 0);
	}

	void Rotating(float horizontal)
	{
		Vector3 targetDirection = new Vector3(horizontal, 0f, 0f);

		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

		rigidbody.MoveRotation(newRotation);
	}
}