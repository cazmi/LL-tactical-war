using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour {

	public static TurnManager instance;

	public List<HumanPlayer> players = new List<HumanPlayer>();
	public List<BotEnemy> enemies = new List<BotEnemy>();
	
	TurnStateStart turnStateStart;
	TurnStatePlayerTurn turnStatePlayerTurn;
	TurnStateEnemyBot turnStateEnemyBot;

	public int totalPlayer = 1;
	public int totalEnemy = 1;

	public GameObject playerPrefab;
	public GameObject enemyPrefab;
	public GameObject tacticScene;
	
	public Player currentTurn;

	public bool onGoingTurn;
	public bool tacticSceneloaded;

	public enum TurnState
	{
		Setup,
		PlayerTurn,
		EnemyTurn,
		EndGame
	}

	public enum FirstTurn
	{
		Player,
		Enemy
	}

	public TurnState turnState;
	public FirstTurn firstTurn;

	void Awake()
	{
		instance = this;

		tacticScene = GameObject.Find("TacticScene");

		turnStateStart = gameObject.AddComponent<TurnStateStart> ();
		turnStatePlayerTurn = gameObject.AddComponent<TurnStatePlayerTurn> ();
		turnStateEnemyBot = gameObject.AddComponent<TurnStateEnemyBot> ();
	}
	
	// Use this for initialization
	void Start () {
		turnState = TurnState.Setup;
		tacticSceneloaded = true;
		onGoingTurn = false;
	}
	
	// Update is called once per frame
	void Update () {
		switch (turnState) 
		{
		case TurnState.Setup:
			turnStateStart.Initialize(totalPlayer, playerPrefab, totalEnemy, enemyPrefab);
			break;
		case TurnState.PlayerTurn:
			if(!onGoingTurn) turnStatePlayerTurn.EnterPickState();
			break;
		case TurnState.EnemyTurn:
			if(!onGoingTurn) turnStateEnemyBot.InitializeAction();
			break;
		}
	}

	public void NextTurn()
	{
		if(turnState == TurnState.PlayerTurn)
			turnState = TurnState.EnemyTurn;
		else
			turnState = TurnState.PlayerTurn;
	}

	public void checkBattleResult()
	{
		if(players.Count == 0 || enemies.Count == 0)
			turnState = TurnState.EndGame;

	}

	void OnGUI()
	{
		GUI.Label (new Rect (10, 0, 110, 25), "State : " + turnState);
		GUI.Label (new Rect (10, 45, 300, 25), "Current Turn : " + currentTurn);
	}
}
