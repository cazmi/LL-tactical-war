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
	int unitsCounter;

	Vector3 firstRowPlayerUnits;
	Vector3 firstRowEnemyUnits;

	bool animating;
	bool showFormationMenu;
	
	int battleTime;
	float startingTime;
	public int limitTime= 60;

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

		print ("general direction: " + playerGeneral.transform.forward);
		/*
		 * Spawn Player's squads
		 */
		for(int i = 0; i < playerGeneral.currentUnits; i++)
		{
			GameObject SpawnedUnit = Instantiate(player, playerUnitsSpawner.position, Quaternion.Euler(new Vector3(0,90,0))) as GameObject;
			playerUnit = SpawnedUnit.GetComponent<Player>();
			playerUnit.gameObject.AddComponent<PlayerAIController>();
			playerUnit.GetComponent<NavMeshAgent>().enabled = true;
			playerUnit.transform.parent = troopsGameObject.transform;
			playerTroops.Add(playerUnit);
		}
		GeneralFormation();

		/*firstRowPlayerUnits = playerUnitsSpawner.position;
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

		unitsCounter = (int)playerGeneral.currentUnits;
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
		}*/

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
			animating = false;				// set camera animation value to false
			showFormationMenu = true;		// show formation menu UI 
			battleTime = 0;					// reset battle time to 0
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
		battleTime = (int)(Time.time - startingTime);

		if(!playerGeneral.isAlive || !enemyGeneral.isAlive
		   || battleTime >= limitTime)
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
		to.playerClass.BaseDefense = from.modifiedDefense;
	}

	void OnGUI()
	{
		if(battleState != BattleState.Preview)
		{
			GUI.Label(new Rect(Screen.width/2, 0, 20, 20), battleTime.ToString());

			if(showFormationMenu)
			{
				if(GUI.Button(new Rect(10, 80, 100, 20), "Line"))
				{
					GeneralFormation();
				}
				if(GUI.Button(new Rect(10, 100, 100, 20), "Wedge"))
				{
					OffensiveFormation();
				}
				if(GUI.Button(new Rect(10, 120, 100, 20), "Phalanx"))
				{
					DefensiveFormation();
				}
				if(GUI.Button(new Rect(10, 160, 100, 20), "Start Battle"))
				{
					showFormationMenu = false;
					startingTime = Time.time;
					battleState = BattleState.In_Battle;

					for(int i = 0; i < playerTroops.Count; i++)
					{
						if(playerTroops[i] != playerGeneral)
						{
							playerTroops[i].GetComponent<PlayerAIController>().botState = PlayerAIController.BotState.Targeting;
						}
					}
				}
			}
		}
	}

	void GeneralFormation()
	{
		int column = 1, maxPerRow = 10;
		float addition = 0;
		Vector3 centerPosition = new Vector3(playerSpawner.position.x - 5, playerSpawner.position.y, playerSpawner.position.z);
		firstRowPlayerUnits = centerPosition;

		for(int i = 0; i < playerTroops.Count; i++)
		{
			playerTroops[i].modifiedAttack = playerTroops[i].playerClass.BaseAttack;
			playerTroops[i].modifiedDefense = playerTroops[i].playerClass.BaseDefense;

			if(playerTroops[i] != playerGeneral)
			{
				if(column % 2 == 1)
				{
					firstRowPlayerUnits.z = centerPosition.z + (2.5f + addition);
				}
				else
				{
					firstRowPlayerUnits.z = centerPosition.z - (2.5f + addition);
					addition += 5;
				}

				playerTroops[i].transform.position = firstRowPlayerUnits;
				column++;

				// go to next row if column exceeds maximum per row
				if(column > maxPerRow)
				{
					column = 1;
					addition = 0;
					firstRowPlayerUnits.x -= 5;
				}
			}
		}
	}

	void OffensiveFormation()
	{	
		int column = 1, row = 1;
		float addition = 0;

		// center position of each row (invisible in-game), used to determine column position as well
		Vector3 centerPosition = new Vector3(playerSpawner.position.x - 5, playerSpawner.position.y, playerSpawner.position.z);
		firstRowPlayerUnits = centerPosition; // put the guy at center position

		for(int i = 0; i < playerTroops.Count; i++)
		{
			playerTroops[i].modifiedAttack = playerTroops[i].playerClass.BaseAttack + 10;
			playerTroops[i].modifiedDefense = playerTroops[i].playerClass.BaseDefense - 10;

			if(playerTroops[i] != playerGeneral)
			{
					// even row
					if(row % 2 == 0)		
					{
						if(column % 2 == 1)
						{
							firstRowPlayerUnits.z = centerPosition.z + (2 + addition);	// odd column-guy goes to the left
						}
						else
						{
							firstRowPlayerUnits.z = centerPosition.z - (2 + addition);	// even column-guy goes to the right

							// additional factor is added here in even column because odd(left) -> even(right), back to odd again
							addition += 4;				
						}
					}
					// odd row except 1 (3, 5, 7, etc)
					else if(row % 2 == 1)	
					{
						if(column == 1)
						{
							firstRowPlayerUnits.z = centerPosition.z;		// first column guy goes exactly at the center
						}
						else if(column % 2 == 1)
						{
							firstRowPlayerUnits.z = centerPosition.z - (4 + addition);	// odd column-guy goes to the right

							// additional factor is added here in odd column because first(center) -> even(left) -> odd(right), back to even again
							addition += 4;
						}
						else
						{
							firstRowPlayerUnits.z = centerPosition.z + (4 + addition);	// even column-guy goes to the left
						}
					}


				playerTroops[i].transform.position = firstRowPlayerUnits;
				column++;

				// go to next row if column exceeds maximum per row (here, row = max per row)
				if(column > row)
				{
					row++;
					column = 1; 
					addition = 0;

					// next row will be set behind the current row
					firstRowPlayerUnits.x -= 3;
				}
			}
		}
	}

	void DefensiveFormation()
	{
		int column = 1, maxPerRow = 7;
		float addition = 0;
		Vector3 centerPosition = new Vector3(playerSpawner.position.x - 5, playerSpawner.position.y, playerSpawner.position.z);
		firstRowPlayerUnits = centerPosition;

		for(int i = 0; i < playerTroops.Count; i++)
		{
			playerTroops[i].modifiedAttack = playerTroops[i].playerClass.BaseAttack - 10;
			playerTroops[i].modifiedDefense = playerTroops[i].playerClass.BaseDefense + 10;	

			if(playerTroops[i] != playerGeneral)
			{
				if(column % 2 == 1)
				{
					firstRowPlayerUnits.z = centerPosition.z + (1.5f + addition);
				}
				else
				{
					firstRowPlayerUnits.z = centerPosition.z - (1.5f + addition);
					addition += 3;
				}
				
				playerTroops[i].transform.position = firstRowPlayerUnits;
				column++;
				
				// go to next row if column exceeds maximum per row
				if(column > maxPerRow)
				{
					column = 1;
					addition = 0;
					firstRowPlayerUnits.x -= 3;
				}
			}
		}
	}
}
