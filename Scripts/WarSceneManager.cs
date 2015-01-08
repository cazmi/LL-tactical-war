using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarSceneManager : MonoBehaviour {

	public static WarSceneManager instance;

	GameObject warScene;

	Transform playerSpawner;	
	Transform playerUnitsSpawner;
	Transform enemySpawner;
	Transform enemyUnitsSpawner;

	public Player playerGeneral;
	public Player playerUnit;
	public List<Player> playerUnits = new List<Player>();
	public Player enemyGeneral;
	public Player enemyUnit;
	public List<Player> enemyUnits = new List<Player>();

	public int playerUnitsRow;
	public int playerUnitsColumn;
	public int enemyUnitsRow;
	public int enemyUnitsColumn;

	Vector3 firstRowPlayerUnits;
	Vector3 firstRowEnemyUnits;

	public enum BattleState
	{
		Pre_Battle,
		In_Battle,
		Halt_Battle,
		Post_Battle
	}

	public BattleState battleState;

	void Awake()
	{
		instance = this;

		warScene = GameObject.Find("WarScene");

		playerSpawner = transform.Find("PlayerSpawner");
		playerUnitsSpawner = transform.Find("PlayerUnitsSpawner");
		enemySpawner = transform.Find("EnemySpawner");
		enemyUnitsSpawner = transform.Find("EnemyUnitsSpawner");
	}
	
	public void InitializeWar(Player player, Player enemy)
	{
		battleState = BattleState.Pre_Battle;

		/*
		 * Spawn Player's General
		 */
		playerGeneral = Instantiate(player, playerSpawner.position, Quaternion.Euler(new Vector3(0,90,0))) as Player;

		CopyComponent(player.gameObject.GetComponent<Player>(), playerGeneral.gameObject);

		/*print (player.playerClass.BaseAttack);
		playerGeneral.transform.parent = warScene.transform;
		playerGeneral.gameObject.AddComponent<PlayerController>();
		playerUnits.Add(playerGeneral);*/

		/*
		 * Spawn Player's squads
		 *
		firstRowPlayerUnits = playerUnitsSpawner.position;
		for(int i = 0; i < playerUnitsRow; i++)
		{
			for(int j = 0; j < playerUnitsColumn; j++)
			{
				playerUnit = Instantiate(player, firstRowPlayerUnits, Quaternion.Euler(new Vector3(0,90,0))) as Player;
				playerUnit.transform.parent = warScene.transform;
				playerUnit.gameObject.AddComponent<PlayerAIController>();
				playerUnit.GetComponent<NavMeshAgent>().enabled = true;
				playerUnits.Add(playerUnit);
				
				firstRowPlayerUnits.z += 5;
			}
			firstRowPlayerUnits.z = playerUnitsSpawner.position.z;
			firstRowPlayerUnits.x -= 3;
		}

		/*
		 * Spawn Enemy's General
		 *
		enemyGeneral = Instantiate(enemy, enemySpawner.position, Quaternion.Euler(new Vector3(0,270,0))) as Player;
		enemyGeneral.transform.parent = warScene.transform;
		enemyGeneral.gameObject.AddComponent<AIController>();
		enemyGeneral.GetComponent<NavMeshAgent>().enabled = true;
		enemyUnits.Add(enemyGeneral);
		
		/*
		 * Spawn Enemy's squads
		 *
		firstRowEnemyUnits = enemyUnitsSpawner.position;
		for(int i = 0; i < enemyUnitsRow; i++)
		{
			for(int j = 0; j < enemyUnitsColumn; j++)
			{
				enemyUnit = Instantiate(enemy, firstRowEnemyUnits, Quaternion.Euler(new Vector3(0,270,0))) as Player;
				enemyUnit.transform.parent = warScene.transform;
				enemyUnit.gameObject.AddComponent<AIController>();
				enemyUnit.GetComponent<NavMeshAgent>().enabled = true;
				enemyUnits.Add(enemyUnit);	
				
				firstRowEnemyUnits.z += 5;
			}
			firstRowEnemyUnits.z = enemyUnitsSpawner.position.z;
			firstRowEnemyUnits.x += 3;
		}*/
	}



	void Update()
	{
		switch(battleState)
		{
		case BattleState.Pre_Battle:
			StartCoroutine("AnimateCamera");
			break;
		case BattleState.In_Battle:
			InBattleMonitor();
			break;
		case BattleState.Halt_Battle:
			break;
		case BattleState.Post_Battle:
			break;
		}
	}

	IEnumerator AnimateCamera()
	{
		yield return new WaitForSeconds(5);
		battleState = BattleState.In_Battle;
	}

	void InBattleMonitor()
	{
		/*if(!playerGeneral.isAlive || !enemyGeneral.isAlive ||
			playerUnits.Count == 0 || enemyUnits.Count == 0)
		{
			battleState = BattleState.Post_Battle;
		}*/
	}

	Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}
}
