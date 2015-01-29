using UnityEngine;
using System.Collections;

public class PlayerAIController : MonoBehaviour {
	Animator anim;
	NavMeshAgent nav;
	Player target;
	HumanPlayer humanPlayer;

	bool isTargetRemoved;
	bool defenseMode;

	Vector3 targetDirection;
	Vector3 targetRetreat;

	float attackDistance;
	float approachDistance;

	public float turnSmoothing = 20f;	// A smoothing value for turning the player.
	
	public enum BotState
	{
		Idle,
		Defense,
		//Advancing,
		Targeting,
		Chasing,
		Attacking,
		Covering,
		FallingBack,
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
		approachDistance = 15f;
		
		isTargetRemoved = false;
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
		case BotState.Defense:
			Defense();
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
		case BotState.FallingBack:
			FallingBack();
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
	}

	void Defense()
	{
		defenseMode = true;
		anim.SetFloat("Speed", 0);
		anim.SetBool("Attacking", false);

		for(int i = 0; i < WarSceneManager.instance.enemyTroops.Count; i++)
		{
			if(Vector3.Distance(transform.position, WarSceneManager.instance.enemyTroops[i].transform.position) <= approachDistance)
			{
				target = WarSceneManager.instance.enemyTroops[i];
				botState = BotState.Chasing;
			}
		}
	}
	
	public void TargetState()
	{
		botState = BotState.Targeting;
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
		anim.SetBool("Attacking", false);

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
			if(!defenseMode)
			{
				botState = BotState.Targeting;
			}
			else
			{
				botState = BotState.FallingBack;
			}
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

	public void ChooseDestination()
	{
		int i = 0;

		do
		{
			targetRetreat = WarSceneManager.instance.initialPlayerPosition[i];
			i++;
		}while(WarSceneManager.instance.occupiedPosition.Contains(targetRetreat)); 

		WarSceneManager.instance.occupiedPosition.Add(targetRetreat);

		botState = BotState.FallingBack;
	}

	void FallingBack()
	{
		nav.speed = 15;

		anim.SetFloat("Speed", nav.speed);
		anim.SetBool("Attacking", false);

		nav.SetDestination(targetRetreat);
		Rotating(targetRetreat);

		if(Vector3.Distance(transform.position, targetRetreat) <= 0.2f)
		{
			nav.speed = 10;
			transform.rotation = Quaternion.Euler(0,90,0);
			botState = BotState.Defense;
		}
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

		if(!isTargetRemoved)
		{
			int index = WarSceneManager.instance.playerTroops.IndexOf(humanPlayer);
			WarSceneManager.instance.playerTroops.Remove(WarSceneManager.instance.playerTroops[index]);
			isTargetRemoved = true;
		}
	}

	Player FindTarget()
	{
		int random = Random.Range(0, WarSceneManager.instance.enemyTroops.Count-1);
		Player target = WarSceneManager.instance.enemyTroops[random];
		
		return target;
	}
}
