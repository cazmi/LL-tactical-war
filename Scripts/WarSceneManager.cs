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

	public HumanPlayer playerGeneral;
	public List<HumanPlayer> playerUnits = new List<HumanPlayer>();
	public BotEnemy enemyGeneral;
	public List<BotEnemy> enemyUnits = new List<BotEnemy>();

	public int playerUnitsRow;
	public int playerUnitsColumn;
	public int enemyUnitsRow;
	public int enemyUnitsColumn;

	public Transform humanPrefab;
	public Transform humanUnitsPrefab;
	public Transform enemyPrefab;
	public Transform enemyUnitsPrefab;

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

	// Use this for initialization
	void Start () {
		InitializeWar();
		battleState = BattleState.Pre_Battle;
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

	void InitializeWar()
	{
		/*
		 * Spawn Player's General
		 */
		playerGeneral = ((Transform)Instantiate(humanPrefab, playerSpawner.position, Quaternion.Euler(new Vector3(0,90,0)))).GetComponent<HumanPlayer>();
		playerGeneral.transform.parent = warScene.transform;
		playerGeneral.playerClass = GameObject.Find("ClassContainer").GetComponent<BaseClass_Fighter>();
		playerGeneral.playerWeapon = playerGeneral.GetComponent<BaseWeapon_Dagger>();
		playerGeneral.gameObject.AddComponent<PlayerController>();
		playerUnits.Add(playerGeneral);

		/*
		 * Spawn Player's squads
		 */
		firstRowPlayerUnits = playerUnitsSpawner.position;
		for(int i = 0; i < playerUnitsRow; i++)
		{
			for(int j = 0; j < playerUnitsColumn; j++)
			{
				playerUnits.Add(((Transform)Instantiate(humanUnitsPrefab, firstRowPlayerUnits, Quaternion.Euler(new Vector3(0,90,0)))).GetComponent<HumanPlayer>());
				playerUnits[playerUnits.Count-1].transform.parent = warScene.transform;
				playerUnits[playerUnits.Count-1].playerClass = GameObject.Find("ClassContainer").GetComponent<BaseClass_Fighter>();
				playerUnits[playerUnits.Count-1].playerWeapon = playerUnits[playerUnits.Count-1].GetComponent<BaseWeapon_Spear>();
				playerUnits[playerUnits.Count-1].gameObject.AddComponent<PlayerAIController>();

				firstRowPlayerUnits.z += 5;
			}
			firstRowPlayerUnits.z = playerUnitsSpawner.position.z;
			firstRowPlayerUnits.x -= 3;
		}

		/*
		 * Spawn Enemy's General
		 */
		enemyGeneral = ((Transform)Instantiate(enemyPrefab, enemySpawner.position, Quaternion.Euler(new Vector3(0,270,0)))).GetComponent<BotEnemy>();
		enemyGeneral.transform.parent = warScene.transform;
		enemyGeneral.playerClass = GameObject.Find("ClassContainer").GetComponent<BaseClass_Apprentice>();
		enemyGeneral.playerWeapon = enemyGeneral.GetComponent<BaseWeapon_Staff>();
		enemyGeneral.gameObject.AddComponent<AIController>();
		enemyUnits.Add(enemyGeneral);

		/*
		 * Spawn Enemy's squads
		 */		
		firstRowEnemyUnits = enemyUnitsSpawner.position;
		for(int i = 0; i < enemyUnitsRow; i++)
		{
			for(int j = 0; j < enemyUnitsColumn; j++)
			{
				enemyUnits.Add(((Transform)Instantiate(enemyUnitsPrefab, firstRowEnemyUnits, Quaternion.Euler(new Vector3(0,270,0)))).GetComponent<BotEnemy>());
				enemyUnits[enemyUnits.Count-1].transform.parent = warScene.transform;
				enemyUnits[enemyUnits.Count-1].playerClass = GameObject.Find("ClassContainer").GetComponent<BaseClass_Apprentice>();
				enemyUnits[enemyUnits.Count-1].playerWeapon = enemyUnits[enemyUnits.Count-1].GetComponent<BaseWeapon_Staff>();
				enemyUnits[enemyUnits.Count-1].gameObject.AddComponent<AIController>();

				firstRowEnemyUnits.z += 5;
			}
			firstRowEnemyUnits.z = enemyUnitsSpawner.position.z;
			firstRowEnemyUnits.x += 3;
		}
	}

	IEnumerator AnimateCamera()
	{
		yield return new WaitForSeconds(5);
		battleState = BattleState.In_Battle;
	}

	void InBattleMonitor()
	{
		if(playerUnits.Count == 0 || enemyUnits.Count == 0)
		{
			battleState = BattleState.Post_Battle;
		}
	}
}
