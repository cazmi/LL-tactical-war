using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarSceneManager : MonoBehaviour {

	public static WarSceneManager instance;

	GameObject troopsGameObject;

	Transform playerSpawner;	
	Transform playerUnitsSpawner;
	Transform enemySpawner;
	Transform enemyUnitsSpawner;

	public Player playerGeneral;
	public Player playerUnit;
	public List<Player> playerTroops = new List<Player>();
	public Player enemyGeneral;
	public Player enemyUnit;
	public List<Player> enemyTroops = new List<Player>();

	int playerUnitsRow;
	int playerUnitsColumn;
	int enemyUnitsRow;
	int enemyUnitsColumn;

	Vector3 firstRowPlayerUnits;
	Vector3 firstRowEnemyUnits;

	bool animating;
	bool showFormationMenu;

	public enum BattleState
	{
		Preview,
		Pre_Battle,
		In_Battle,
		Halt_Battle,
		Post_Battle
	}

	public BattleState battleState;

	void Awake()
	{
		instance = this;

		troopsGameObject = GameObject.Find("Troops");

		playerSpawner = transform.Find("PlayerSpawner");
		playerUnitsSpawner = transform.Find("PlayerUnitsSpawner");
		enemySpawner = transform.Find("EnemySpawner");
		enemyUnitsSpawner = transform.Find("EnemyUnitsSpawner");
	}
	
	public void InitializeWar(GameObject player, GameObject enemy)
	{
		battleState = BattleState.Preview;

		// Clear previous battle troops
		playerTroops.Clear();
		enemyTroops.Clear();

		/*
		 * Spawn Player's General
		 */
		GameObject spawnedGeneral = Instantiate(player, playerSpawner.position, Quaternion.Euler(new Vector3(0,90,0))) as GameObject;
		playerGeneral = spawnedGeneral.GetComponent<Player>();
		CopyStats(player.GetComponent<Player>(), playerGeneral);
		playerGeneral.gameObject.AddComponent<PlayerController>();	
		playerGeneral.transform.parent = troopsGameObject.transform;
		playerTroops.Add(playerGeneral);


		/*
		 * Spawn Player's squads
		 */
		firstRowPlayerUnits = playerUnitsSpawner.position;
		if(playerGeneral.currentUnits >= 10)
		{
			playerUnitsRow = Mathf.CeilToInt(playerGeneral.currentUnits / 10);
			playerUnitsColumn = (int)(playerGeneral.currentUnits / (playerGeneral.currentUnits/10));
		}
		else
		{
			playerUnitsRow = 1;
			playerUnitsColumn = (int)playerGeneral.currentUnits;
		}

		int unitsCounter = (int)playerGeneral.currentUnits;
		for(int i = 0; i < playerUnitsRow; i++)
		{
			for(int j = 0; j < playerUnitsColumn; j++)
			{
				GameObject SpawnedUnit = Instantiate(player, firstRowPlayerUnits, Quaternion.Euler(new Vector3(0,90,0))) as GameObject;
				playerUnit = SpawnedUnit.GetComponent<Player>();
				playerUnit.gameObject.AddComponent<PlayerAIController>();
				playerUnit.GetComponent<NavMeshAgent>().enabled = true;
				playerUnit.transform.parent = troopsGameObject.transform;
				playerTroops.Add(playerUnit);
				
				firstRowPlayerUnits.z += 5;

				unitsCounter--;
				if(unitsCounter == 0)
				{
					i = playerUnitsRow; // max out i value so the outer loop will break
					break;
				}
			}
			firstRowPlayerUnits.z = playerUnitsSpawner.position.z;
			firstRowPlayerUnits.x -= 3;
		}

		/*
		 * Spawn Enemy's General
		 */
		GameObject spawnedEnemyGeneral = Instantiate(enemy, enemySpawner.position, Quaternion.Euler(new Vector3(0,270,0))) as GameObject;
		enemyGeneral = spawnedEnemyGeneral.GetComponent<Player>();
		CopyStats(enemy.GetComponent<Player>(), enemyGeneral);
		enemyGeneral.gameObject.AddComponent<GeneralAIController>();
		enemyGeneral.GetComponent<NavMeshAgent>().enabled = true;
		enemyGeneral.transform.parent = troopsGameObject.transform;
		enemyTroops.Add(enemyGeneral);
		
		/*
		 * Spawn Enemy's squads
		 */
		firstRowEnemyUnits = enemyUnitsSpawner.position;
		if(enemyGeneral.currentUnits >= 10)
		{
			enemyUnitsRow = Mathf.CeilToInt(enemyGeneral.currentUnits / 10);
			enemyUnitsColumn = (int)(enemyGeneral.currentUnits / (enemyGeneral.currentUnits/10));
		}
		else
		{
			enemyUnitsRow = 1;
			enemyUnitsColumn = (int)enemyGeneral.currentUnits;
		}

		unitsCounter = (int)enemyGeneral.currentUnits;
		for(int i = 0; i < enemyUnitsRow; i++)
		{
			for(int j = 0; j < enemyUnitsColumn; j++)	
			{
				GameObject SpawnedEnemyUnit = Instantiate(enemy, firstRowEnemyUnits, Quaternion.Euler(new Vector3(0,270,0))) as GameObject;
				enemyUnit = SpawnedEnemyUnit.GetComponent<Player>();
				enemyUnit.gameObject.AddComponent<AIController>();
				enemyUnit.GetComponent<NavMeshAgent>().enabled = true;
				enemyUnit.transform.parent = troopsGameObject.transform;
				enemyTroops.Add(enemyUnit);	
				
				firstRowEnemyUnits.z += 5;

				unitsCounter--;
				if(unitsCounter == 0)
				{
					i = enemyUnitsRow; // max out i value so the outer loop will break
					break;
				}
			}
			firstRowEnemyUnits.z = enemyUnitsSpawner.position.z;
			firstRowEnemyUnits.x += 3;
		}
	}

	void Update()
	{
		switch(battleState)
		{
		case BattleState.Preview:
			if(!animating)StartCoroutine("AnimateCamera");
			break;
		case BattleState.Pre_Battle:
			animating = false;		// set back camera animation value to false
			showFormationMenu = true;
			break;
		case BattleState.In_Battle:
			InBattleMonitor();
			break;
		case BattleState.Halt_Battle:
			break;
		case BattleState.Post_Battle:
			EndWar();
			break;
		}
	}

	IEnumerator AnimateCamera()
	{
		animating = true;
		yield return new WaitForSeconds(5);
		//Time.timeScale = 0;
		battleState = BattleState.Pre_Battle;
	}

	void InBattleMonitor()
	{
		if(!playerGeneral.isAlive || !enemyGeneral.isAlive)
		{
			battleState = BattleState.Post_Battle;
		}
	}

	void EndWar()
	{
		foreach(Transform players in troopsGameObject.transform)
		{
			if(players.tag == "Player")
			{
				Destroy(players.gameObject);
			}
		}
		foreach(Transform enemies in troopsGameObject.transform)
		{
			if(enemies.tag == "Enemy")
			{
				Destroy(enemies.gameObject);
			}
		}

		TurnManager.instance.tacticScene.SetActive(true);
		TurnManager.instance.warScene.SetActive(false);
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

	void CopyStats(Player from, Player to)
	{
		to.playerClass.BaseHP = from.currentHealth;
		to.playerClass.TotalUnits = from.currentUnits;
		to.playerClass.BaseAttack = from.modifiedAttack;
		to.playerClass.BaseDefense = from.playerClass.BaseDefense;
	}

	void OnGUI()
	{
		if(showFormationMenu)
		{
			if(GUI.Button(new Rect(10, 80, 100, 20), "Line"))
			{
				for(int i = 0; i < playerTroops.Count; i++)
				{
					playerTroops[i].modifiedAttack = playerTroops[i].playerClass.BaseAttack;
					playerTroops[i].modifiedDefense = playerTroops[i].playerClass.BaseDefense;
				}
			}
			if(GUI.Button(new Rect(10, 100, 100, 20), "Wedge"))
			{
				for(int i = 0; i < playerTroops.Count; i++)
				{
					playerTroops[i].modifiedAttack = playerTroops[i].playerClass.BaseAttack + 10;
					playerTroops[i].modifiedDefense = playerTroops[i].playerClass.BaseDefense - 10;
				}
			}
			if(GUI.Button(new Rect(10, 120, 100, 20), "Phalanx"))
			{
				for(int i = 0; i < playerTroops.Count; i++)
				{
					playerTroops[i].modifiedAttack = playerTroops[i].playerClass.BaseAttack - 10;
					playerTroops[i].modifiedDefense = playerTroops[i].playerClass.BaseDefense + 10;
				}
			}
			if(GUI.Button(new Rect(10, 160, 100, 20), "Start Battle"))
			{
				showFormationMenu = false;
				battleState = BattleState.In_Battle;	
			}
		}
	}
}
