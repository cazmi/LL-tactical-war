using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

	Animator anim;
	NavMeshAgent nav;
	Transform player;
	BotEnemy botEnemy;

	Vector3 targetMovement;
	float attackDistance;

	enum BotState
	{
		Idle,
		//Advancing,
		Targeting,
		Attacking
	}

	BotState botState;

	void Awake () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		anim = GetComponent<Animator>();
		nav = GetComponent<NavMeshAgent>();		
		botEnemy = GetComponent<BotEnemy>();
	}

	void Start()
	{
		botState = BotState.Idle;
		attackDistance = 2.0f;
	}

	// Update is called once per frame
	void Update () {
		switch(botState)
		{
		case BotState.Idle:
			Idle();
			break;
		case BotState.Targeting:
			Targeting();
			break;
		case BotState.Attacking:
			Attacking();
			break;
		}
		botEnemy.playerWeapon.TraceAttack(botEnemy);
	}

	void Idle()
	{
		if(WarSceneManager.instance.battleState == WarSceneManager.BattleState.In_Battle)
		{
			botState = BotState.Targeting;
		}
	}

	void Targeting()
	{
		if(botEnemy.isAlive)
		{
			anim.SetFloat("Speed", nav.speed);
			nav.SetDestination(player.position);
			if(Vector3.Distance(transform.position, player.position) < attackDistance)
			{
				botState = BotState.Attacking;
			}
		}
		else
		{
			nav.enabled = false;
		}
		/*targetMovement = transform.forward * 12 * Time.deltaTime;
		rigidbody.MovePosition(transform.position + targetMovement);
		print(targetMovement);*/
	}

	void Attacking()
	{
		anim.SetBool("Attacking", true);
	}
}