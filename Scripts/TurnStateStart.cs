using UnityEngine;
using System.Collections;

public class TurnStateStart : MonoBehaviour {

	TileMap tm;
	ArrayList tileLocationIndex = new ArrayList();
	int tileSize;
	HumanPlayer player;
	BotEnemy enemy;
	PointerScript ps;

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
	public void Initialize(int totalPlayer, GameObject playerPref, int totalEnemy, GameObject enemyPref)
	{
		int j = 67;
		// spawn players
		for(int i=0; i<totalPlayer; i++)
		{
			//int randPosition = (int)tileLocationIndex[Random.Range(0, tileLocationIndex.Count)];
			int randPosition = j+i;
			player = ((GameObject)Instantiate(playerPref, tm.tiles[randPosition].position, playerPref.transform.rotation)).GetComponent<HumanPlayer>();
			player.transform.parent = TurnManager.instance.tacticScene.transform;
			player.tilePosition = randPosition;
			tm.tiles[randPosition].reachable = false;

			tileLocationIndex.Remove(randPosition);

			TurnManager.instance.players.Add(player);
		}

		j = 105;
		// spawn enemies
		for(int i=0; i<totalEnemy; i++)
		{
			//int randPosition = (int)tileLocationIndex[Random.Range(0, tileLocationIndex.Count)];
			int randPosition = j+(i*2);
			enemy = ((GameObject)Instantiate(enemyPref, tm.tiles[randPosition].position, enemyPref.transform.rotation)).GetComponent<BotEnemy>();
			enemy.transform.parent = TurnManager.instance.tacticScene.transform;
			enemy.tilePosition = randPosition;
			tm.tiles[randPosition].reachable = false;
			
			tileLocationIndex.Remove(randPosition);

			TurnManager.instance.enemies.Add(enemy);
		}
		
		// decide who goes first based on scenario (?)
		ChooseWhoGoesFirst ();

		// summon pointer
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
