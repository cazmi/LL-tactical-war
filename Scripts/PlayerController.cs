using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

	Animator anim;
	Vector3 targetMovement;
	Vector3 targetDirection;

	Player player;

	public float moveSpeed = 10f;
	public float turnSmoothing = 20f;	// A smoothing value for turning the player.
	
	RaycastHit hitInfo;

	float nextSkill;
	float skillCooldown;

	bool showUI;
	
	void Awake()
	{		
		anim = GetComponent<Animator>();
		player = WarSceneManager.instance.playerGeneral;
		//player = GetComponent<HumanPlayer>();
	}

	void Start()
	{
		targetDirection = new Vector3(1f, 0f, 0f);
		skillCooldown = player.playerClass.classSkill.SkillCooldown;
	}

	void FixedUpdate()
	{	
		// Only control player when the battle starts
		if(WarSceneManager.instance.battleState == WarSceneManager.BattleState.In_Battle)
		{
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			Move(h, v);
			//Move(v, h);
		}
	}

	void Update()
	{
		// Only control player when the battle starts
		if(WarSceneManager.instance.battleState == WarSceneManager.BattleState.In_Battle)
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
				player.playerClass.classSkill.Cast(player, targetDirection);
			}

			// command UI
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Time.timeScale = Convert.ToSingle(showUI);
				showUI = !showUI;
			}

			// dead
			if(!player.isAlive)
				Dead ();
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

	void Dead()
	{
		anim.SetFloat("Speed", 0);
		anim.SetBool("Attacking", false);
		anim.SetTrigger("Dead");

		Destroy(this);
	}

	void OnGUI()
	{
		if(WarSceneManager.instance.battleState != WarSceneManager.BattleState.Preview)
		{
			GUI.Label (new Rect (10, 0, 100, 20), "HP >> " + (Mathf.Clamp(player.currentHealth, 0, Mathf.Infinity)));
			GUI.Label (new Rect (100, 0, 100, 20), "Atk >> " + player.modifiedAttack);
			GUI.Label (new Rect (200, 0, 200, 20), "Def >> " + player.modifiedDefense);
			GUI.Label (new Rect (10, 20, 200, 20), "Units Remaining >> " + (WarSceneManager.instance.playerTroops.Count - 1).ToString());
			GUI.Label (new Rect (10, 40, 200, 20), "Skill Cooldown >> " + (Mathf.Clamp(nextSkill - Time.time, 0, Mathf.Infinity)));

			if(showUI)
			{
				if(GUI.Button(new Rect(10, 80, 100, 20), "Advance"))
				{
					Time.timeScale = Convert.ToSingle(showUI);
					showUI = !showUI;
					WarSceneManager.instance.AdvanceCommand();
				}
				if(GUI.Button(new Rect(10, 100, 100, 20), "Cover Me"))
				{

				}
				if(GUI.Button(new Rect(10, 120, 100, 20), "Fall Back"))
				{
					Time.timeScale = Convert.ToSingle(showUI);
					showUI = !showUI;
					//moveSpeed = 15f;
					WarSceneManager.instance.FallBackCommand();
				}
			}
		}
	}
}