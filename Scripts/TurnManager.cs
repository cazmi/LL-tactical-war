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

	public GameObject[] playerPrefab;
	public GameObject[] enemyPrefab;
	public GameObject tacticScene;
	public GameObject warScene;

	public Player currentTurn;
	public bool onGoingTurn;

	public enum TurnState
	{
		Setup,
		PlayerTurn,
		EnemyTurn,
		EndGame
	}
	public TurnState turnState;

	public enum FirstTurn
	{
		Player,
		Enemy
	}
	public FirstTurn firstTurn;

	void Awake()
	{
		instance = this;

		tacticScene = GameObject.Find("TacticScene");
		warScene = GameObject.Find("WarScene");

		turnStateStart = gameObject.AddComponent<TurnStateStart> ();
		turnStatePlayerTurn = gameObject.AddComponent<TurnStatePlayerTurn> ();
		turnStateEnemyBot = gameObject.AddComponent<TurnStateEnemyBot> ();
	}
	
	// Use this for initialization
	void Start () {
		turnState = TurnState.Setup;
		onGoingTurn = false;

		warScene.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		switch (turnState) 
		{
		case TurnState.Setup:
			turnStateStart.Initialize(playerPrefab, enemyPrefab);
			break;
		case TurnState.PlayerTurn:
			if(!onGoingTurn) turnStatePlayerTurn.EnterPickState();
			break;
		case TurnState.EnemyTurn:
			if(!onGoingTurn) turnStateEnemyBot.InitializeAction();
			break;
		case TurnState.EndGame:
			print("END GAME");
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

	public void checkTacticResult()
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
