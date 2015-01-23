using UnityEngine;
using System.Collections;

public class PlayerAIController : MonoBehaviour {
	Animator anim;
	NavMeshAgent nav;
	Player target;
	HumanPlayer humanPlayer;

	bool hasRemoved;
	
	Vector3 targetMovement;
	Vector3 targetDirection;
	float attackDistance;
	
	public float turnSmoothing = 20f;	// A smoothing value for turning the player.
	
	public enum BotState
	{
		Idle,
		//Advancing,
		Targeting,
		Chasing,
		Attacking,
		Covering,
		Dead
	}
	public BotState botState;
	
	void Awake () {
		anim = GetComponent<Animator>();
		nav = GetComponent<NavMeshAgent>();		
		humanPlayer = GetComponent<HumanPlayer>();
	}
	
	void Start()
	{
		botState = BotState.Idle;
		attackDistance = 2f;
		hasRemoved = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!humanPlayer.isAlive)
		{
			botState = BotState.Dead;
		}
		
		switch(botState)
		{
		case BotState.Idle:
			Idle();
			break;
		case BotState.Targeting:
			Targeting();
			break;
		case BotState.Chasing:
			Chasing();
			break;
		case BotState.Attacking:
			Attacking();
			break;
		case BotState.Dead:
			Dead ();
			break;
		}
	}
	
	void Idle()
	{		
		anim.SetFloat("Speed", 0);
		anim.SetBool("Attacking", false);

		/*
		if(WarSceneManager.instance.battleState == WarSceneManager.BattleState.In_Battle)
		{
			target = FindTarget();
			botState = BotState.Targeting;
		}
		*/
	}
	
	void Targeting()
	{
		target = FindTarget();

		if(target != null)
		{
			botState = BotState.Chasing;
		}
	}

	void Chasing()
	{
		anim.SetFloat("Speed", nav.speed);

		nav.SetDestination(target.transform.position);
		Rotating(target.transform.position);
		
		if(Vector3.Distance(transform.position, target.transform.position) <= attackDistance)
		{
			botState = BotState.Attacking;
		}
	}
	
	void Attacking()
	{
		anim.SetFloat("Speed", 0);
		anim.SetBool("Attacking", true);
		
		nav.Stop();
		humanPlayer.playerClass.classWeapon.TraceAttack(humanPlayer);
		
		if(!target.isAlive)
		{
			botState = BotState.Targeting;
		}
		else
		{
			if(Vector3.Distance(transform.position, target.transform.position) > attackDistance)
			{
				botState = BotState.Chasing;
			}
		}
	}

	void Covering()
	{

	}

	public void ToggleHitFrame()
	{
		humanPlayer.playerClass.classWeapon.ToggleHitFrame();
	}

	void Rotating(Vector3 target)
	{
		Vector3 direction = (target - transform.position) / (target - transform.position).magnitude;
		if(direction.x < 0)
			targetDirection = new Vector3(-1f, 0f, 0f);
		if(direction.x > 0)
			targetDirection = new Vector3(1f, 0f, 0f);
		
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		rigidbody.MoveRotation(newRotation);
	}
	
	void Dead()
	{
		nav.enabled = false;
		anim.SetFloat("Speed", 0);
		anim.SetBool("Attacking", false);
		anim.SetTrigger("Dead");

		if(!hasRemoved)
		{
			int index = WarSceneManager.instance.playerTroops.IndexOf(humanPlayer);
			WarSceneManager.instance.playerTroops.Remove(WarSceneManager.instance.playerTroops[index]);
			hasRemoved = true;
		}
	}

	Player FindTarget()
	{
		int random = Random.Range(0, WarSceneManager.instance.enemyTroops.Count-1);
		Player target = WarSceneManager.instance.enemyTroops[random];
		
		return target;
	}
}
