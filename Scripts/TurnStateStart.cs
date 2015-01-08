using UnityEngine;
using System.Collections;

 	public class TurnStateStart : MonoBehaviour {

	TileMap tm;
	ArrayList tileLocationIndex = new ArrayList();
	int tileSize;
	HumanPlayer player;
	BotEnemy enemy;
	PointerScript ps;

	/*BaseClass[] classTypes = new BaseClass[]{new Apprentice(), 
											new Archer(), 
											new Brawler(),
											new Dragoon(),
											new Fighter(),
											new Thief()};*/
	BaseClass[] classTypes; 

	void Awake()
	{
		tm = TileMap.instance;	
		ps = GameObject.Find("Pointer").GetComponent<PointerScript>();
	}

	void Start()
	{
		tileSize = tm.tileX * tm.tileZ;
		// assign all available tiles to list of arrays
		for(int i=0; i<tileSize; i++)
		{
			tileLocationIndex.Add(i);
		}
	}

	// spawn players and enemies, set their positions
	public void Initialize(GameObject[] playerPref, GameObject[] enemyPref)
	{
		int j = 67;
		// spawn players
		for(int i=0; i<playerPref.Length; i++)
		{
			//int randPosition = (int)tileLocationIndex[Random.Range(0, tileLocationIndex.Count)];
			int randPosition = j+i;
			player = ((GameObject)Instantiate(playerPref[i], tm.tiles[randPosition].position, playerPref[i].transform.rotation)).GetComponent<HumanPlayer>();
			player.transform.parent = TurnManager.instance.tacticScene.transform;
			player.tilePosition = randPosition;
			player.playerClass = player.GetComponent<BaseClass>();
			player.playerClass.BoostStats();
			print (player.playerClass.BaseAttack);

			player.playerClass.TerrainEffect(tm.terrainType);

			tm.tiles[randPosition].reachable = false;

			tileLocationIndex.Remove(randPosition);

			TurnManager.instance.players.Add(player);
		}

		j = 105;
		// spawn enemies
		for(int i=0; i<enemyPref.Length; i++)
		{
			//int randPosition = (int)tileLocationIndex[Random.Range(0, tileLocationIndex.Count)];
			int randPosition = j+(i*2);
			enemy = ((GameObject)Instantiate(enemyPref[i], tm.tiles[randPosition].position, enemyPref[i].transform.rotation)).GetComponent<BotEnemy>();
			enemy.transform.parent = TurnManager.instance.tacticScene.transform;
			enemy.tilePosition = randPosition;
			enemy.playerClass = enemy.GetComponent<BaseClass>();

			enemy.playerClass.TerrainEffect(tm.terrainType);

			tm.tiles[randPosition].reachable = false;
			
			tileLocationIndex.Remove(randPosition);

			TurnManager.instance.enemies.Add(enemy);
		}

		// decide who goes first based on scenario (?)
		ChooseWhoGoesFirst ();

		// position pointer to first human player
		ps.AdjustPosition(TurnManager.instance.players[0].transform.position);
	}

	void ChooseWhoGoesFirst()
	{
		if (TurnManager.instance.firstTurn == TurnManager.FirstTurn.Player) 
		{
			TurnManager.instance.turnState = TurnManager.TurnState.PlayerTurn;
		}
		else
		{
			TurnManager.instance.turnState = TurnManager.TurnState.EnemyTurn;
		}
	}
}
