using UnityEngine;
using System.Collections;

public class GeneralAIController : MonoBehaviour {
	
	Animator anim;
	NavMeshAgent nav;
	Player target;
	Player botEnemy;
	
	Vector3 targetMovement;
	Vector3 targetDirection;
	float attackDistance;
	
	public float turnSmoothing = 20f;	// A smoothing value for turning the player.
	
	enum BotState
	{
		Idle,
		//Advancing,
		Targeting,
		Attacking,
		Dead
	}
	
	BotState botState;
	
	void Awake () {
		anim = GetComponent<Animator>();
		nav = GetComponent<NavMeshAgent>();		
		botEnemy = WarSceneManager.instance.enemyGeneral;
	}
	
	void Start()
	{
		botState = BotState.Idle;
		attackDistance = 4f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!botEnemy.isAlive)
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
		
		if(WarSceneManager.instance.battleState == WarSceneManager.BattleState.In_Battle)
		{
			target = FindTarget();
			botState = BotState.Targeting;
		}
	}
	
	void Targeting()
	{
		anim.SetFloat("Speed", nav.speed);
		anim.SetBool("Attacking", false);
		
		nav.SetDestination(target.transform.position);
		Rotating(target.transform.position);
		
		if(Vector3.Distance(transform.position, target.transform.position) <= attackDistance)
		{
			botState = BotState.Attacking;
		}
		/*targetMovement = transform.forward * 12 * Time.deltaTime;
		rigidbody.MovePosition(transform.position + targetMovement);
		print(targetMovement);*/
	}
	
	void Attacking()
	{
		anim.SetFloat("Speed", 0);
		anim.SetBool("Attacking", true);
		
		nav.Stop();
		botEnemy.playerClass.classWeapon.TraceAttack(botEnemy);
		
		if(!target.isAlive)
		{
			botState = BotState.Idle;
		}
		else
		{
			if(Vector3.Distance(transform.position, target.transform.position) > attackDistance)
			{
				botState = BotState.Targeting;
			}
		}
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
	
	public void ToggleHitFrame()
	{
		botEnemy.playerClass.classWeapon.ToggleHitFrame();
	}	
	
	void Dead()
	{
		nav.enabled = false;
		anim.SetFloat("Speed", 0);
		anim.SetBool("Attacking", false);
		anim.SetTrigger("Dead");

		WarSceneManager.instance.enemyTroops.Remove(botEnemy);
	}
	
	Player FindTarget()
	{
		int random = Random.Range(0, WarSceneManager.instance.playerTroops.Count-1);
		Player target = WarSceneManager.instance.playerTroops[random];
		return target;
	}
}