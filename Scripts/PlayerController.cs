using UnityEngine;
using System.Collections;	

public class PlayerController : MonoBehaviour {

	Animator anim;
	Vector3 targetMovement;
	Vector3 targetDirection;

	HumanPlayer player;

	public float moveSpeed = 10f;
	public float turnSmoothing = 20f;	// A smoothing value for turning the player.
	
	RaycastHit hitInfo;

	
	void Awake()
	{		
		anim = GetComponent<Animator>();
		//player = WarSceneManager.instance.playerGeneral;
		player = GetComponent<HumanPlayer>();
	}

	void Start()
	{
		targetDirection = new Vector3(1f, 0f, 0f);
	}

	void FixedUpdate()
	{		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Move(h, v);
	}

	void Update()
	{
		bool attack = Input.GetMouseButtonDown(0);
		anim.SetBool("Attacking", attack);
		player.playerWeapon.TraceAttack(player);
	}

	void Move(float horizontal, float vertical)
	{
		targetMovement.Set(horizontal, 0f, vertical);
		targetMovement = targetMovement.normalized * moveSpeed * Time.deltaTime;

		if(horizontal != 0 || vertical != 0)
		{
			rigidbody.MovePosition(transform.position + targetMovement);
			Rotating(horizontal);
			anim.SetFloat("Speed", moveSpeed);
		}
		else 
		{
			anim.SetFloat("Speed", 0);
		}
	}

	void Rotating(float horizontal)
	{
		if(Input.GetKey(KeyCode.A))
			targetDirection = new Vector3(-1f, 0f, 0f);
		if(Input.GetKey(KeyCode.D))
			targetDirection = new Vector3(1f, 0f, 0f);

		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

		rigidbody.MoveRotation(newRotation);
	}
}