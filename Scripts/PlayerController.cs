using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

	Animator anim;
	Vector3 targetMovement;
	public Vector3 targetDirection;

	HumanPlayer player;

	public float moveSpeed = 10f;
	public float turnSmoothing = 20f;	// A smoothing value for turning the player.
	
	RaycastHit hitInfo;

	float nextSkill;
	float skillCooldown;

	bool showUI;
	
	void Awake()
	{		
		anim = GetComponent<Animator>();
		//player = WarSceneManager.instance.playerGeneral;
		player = GetComponent<HumanPlayer>();
	}

	void Start()
	{
		targetDirection = new Vector3(1f, 0f, 0f);
		skillCooldown = player.playerClass.classSkill.SkillCooldown;
	}

	void FixedUpdate()
	{		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Move(h, v);
	}

	void Update()
	{
		// attack
		bool attack = Input.GetMouseButtonDown(0);
		anim.SetBool("Attacking", attack);
		player.playerClass.classWeapon.TraceAttack(player);

		// guard


		// skill
		if(Input.GetMouseButtonDown(1) && Time.time >= nextSkill)
		{
			nextSkill = Time.time + skillCooldown;
			player.playerClass.classSkill.Cast(player);
		}

		// tactic


		// command UI
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Time.timeScale = Convert.ToSingle(showUI);
			showUI = !showUI;
		}
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

	public void ToggleHitFrame()
	{
		player.playerClass.classWeapon.ToggleHitFrame();
	}

	void OnGUI()
	{
		GUI.Label (new Rect (10, 0, 200, 20), "HP >> " + (Mathf.Clamp(player.currentHealth, 0, Mathf.Infinity)));
		GUI.Label (new Rect (10, 20, 200, 20), "Skill Cooldown >> " + (Mathf.Clamp(nextSkill - Time.time, 0, Mathf.Infinity)));

		if(showUI)
		{
			GUI.Button(new Rect(0, 40, 100, 20), "Formation");
			GUI.Button(new Rect(0, 60, 100, 20), "Advance");
			GUI.Button(new Rect(0, 80, 100, 20), "Cover Me");
			GUI.Button(new Rect(0, 100, 100, 20), "Regroup");
			GUI.Button(new Rect(0, 120, 100, 20), "Fall Back");
		}
	}
}