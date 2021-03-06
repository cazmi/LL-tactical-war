﻿using UnityEngine;
using System.Collections;

public class TurnStateEnemyBot : MonoBehaviour {
	
	TileMap tMap;
	TurnManager tMan;
	WarSceneManager wMan;
	PathFinder pf;
	HumanPlayer pickedPlayer;

	bool reset;
	bool foundEnemy;
	TileMapInfo destinationTile;
	Vector3 finalDestination;

	int totalTurn;
	
	enum EnemyState
	{
		None,
		Pick,
		Decide,
			Move,
			Attack,
			InBattle,
			Wait,
		Animation,
		End
	}
	EnemyState enemyState;

	void Awake()
	{
		tMap = TileMap.instance;
		tMan = TurnManager.instance;
		wMan = WarSceneManager.instance;
		pf = gameObject.AddComponent<PathFinder> ();
	}

	public void InitializeAction()
	{
		tMan.onGoingTurn = true;
		reset = false;
		totalTurn = tMan.enemies.Count;
		enemyState = EnemyState.Pick;
	}

	
	// Update is called once per frame
	void Update () {
		switch(enemyState)
		{
		case EnemyState.None:
			if(!reset) ResetPlayerInfo();
			break;
		case EnemyState.Pick:
			BotPick();
			break;
		case EnemyState.Move:
			BotMove();
			break;
		case EnemyState.Decide:
			if(tMan.tacticScene.activeInHierarchy) DecideNextAction();
			break;
		case EnemyState.Attack:
			BotAttack();
			break;
		case EnemyState.InBattle:
			WaitForBattleToFinish();
			break;
		case EnemyState.Wait:
			Wait();
			break;
		case EnemyState.Animation:
			MovingAnimation();
			break;
		case EnemyState.End:
			EndTurn();
			break;
		}
	}

	void ResetPlayerInfo()
	{
		for(int i=0; i < tMan.enemies.Count; i++)
		{
			tMan.enemies[i].isTurnOver = false;
			tMan.enemies[i].attackEnabled = true;
			tMan.enemies[i].moveEnabled = true;
			tMan.enemies[i].waitEnabled = true;
		}

		reset = true;
	}

	void BotPick()
	{
		do{
			tMan.currentTurn = tMan.enemies[Random.Range(0, tMan.enemies.Count)];
		}while(tMan.currentTurn.isTurnOver);
		enemyState = EnemyState.Decide;
	}

	void DecideNextAction()
	{
		if(tMan.currentTurn == null)
		{
			enemyState = EnemyState.End;
		}
		else
		{
			foundEnemy = false;
			if(tMan.currentTurn.attackEnabled)
			{
				tMap.DetermineAvailableTiles(tMan.currentTurn.tilePosition, tMan.currentTurn.playerClass.TileAttack);
				for(int i=0; i < tMan.players.Count; i++)
				{
					if(tMap.inRangeTiles != null && tMap.inRangeTiles.Contains(tMap.tiles[tMan.players[i].tilePosition]))
					{
						foundEnemy = true;
						pickedPlayer = tMan.players[i];
						enemyState = EnemyState.Attack;
					}
				}
				ResetInRangeTiles();
			}
			
			if(!foundEnemy)
			{
				if(tMan.currentTurn.moveEnabled)
				{
					enemyState = EnemyState.Move;
				}
				else
				{
					enemyState = EnemyState.Wait;
				}
			}
			
			if(!tMan.currentTurn.attackEnabled && !tMan.currentTurn.moveEnabled)
			{
				enemyState = EnemyState.Wait;
			}
		}
	}

	void BotMove()
	{
		tMap.DetermineAvailableTiles(tMan.currentTurn.tilePosition, tMan.currentTurn.playerClass.TileMove);
		do {
			destinationTile = tMap.inRangeTiles[Random.Range(0, tMap.inRangeTiles.Count)];
		}while(!destinationTile.reachable);
		finalDestination = destinationTile.position;

		if(destinationTile.reachable)
		{
			pf.FindPath(tMap.tiles[tMan.currentTurn.tilePosition], destinationTile);
			enemyState = EnemyState.Animation;
		}
	}

	void MovingAnimation()
	{
		if(pf.GetPathList().Count != 0)
			tMan.currentTurn.Move(pf.GetPathList());
		
		if (tMan.currentTurn.transform.position == finalDestination) 
		{	
			PostMoveCondition();
		}
	}

	// update enemy and tile info after moving
	void PostMoveCondition()
	{
		tMan.currentTurn.moveEnabled = false;
		
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = true;	// enable previous occupied tile 								
		tMan.currentTurn.tilePosition = tMap.WorldToTilePoint (tMan.currentTurn.transform.position);
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = false;	// disable current occupied tile
		
		ResetInRangeTiles();

		enemyState = EnemyState.Decide;
	}

	void BotAttack()
	{
		tMap.DetermineAvailableTiles(tMan.currentTurn.tilePosition, tMan.currentTurn.playerClass.TileAttack);

		if(tMap.inRangeTiles.Contains(tMap.tiles[pickedPlayer.tilePosition]))
		{
			tMan.tacticScene.SetActive(false);
			TurnManager.instance.warScene.SetActive(true);
			WarSceneManager.instance.InitializeWar(pickedPlayer.gameObject, tMan.currentTurn.gameObject);

			enemyState = EnemyState.InBattle;
		}
	}

	void WaitForBattleToFinish()
	{
		print ("waiting...");
		if(tMan.tacticScene.activeInHierarchy)
		{
			PostAttackCondition();
		}
	}
	
	void PostAttackCondition()
	{
		tMan.currentTurn.attackEnabled = false;
	
		ResetInRangeTiles();

		// update health and units
		tMan.currentTurn.currentHealth = wMan.enemyGeneral.currentHealth;
		tMan.currentTurn.currentUnits = wMan.enemyTroops.Count - 1;
		
		pickedPlayer.currentHealth = wMan.playerGeneral.currentHealth;
		pickedPlayer.currentUnits = wMan.playerTroops.Count - 1;
		
		if(tMan.currentTurn.currentHealth <= 0)
		{
			Destroy(tMan.currentTurn.gameObject);
			tMan.enemies.Remove((BotEnemy)tMan.currentTurn);
			tMap.tiles[tMan.currentTurn.tilePosition].reachable = true;
		}

		if(pickedPlayer.currentHealth <= 0)
		{
			Destroy(pickedPlayer.gameObject);
			tMan.players.Remove(pickedPlayer);
			tMap.tiles[pickedPlayer.tilePosition].reachable = true;
		}

		tMan.checkTacticResult();

		if(tMan.turnState == TurnManager.TurnState.EndGame)
			enemyState = EnemyState.None;
		else if(!tMan.currentTurn.moveEnabled && !tMan.currentTurn.attackEnabled)
			enemyState = EnemyState.End;
		else
			enemyState = EnemyState.Decide;
	}

	void Wait()
	{
		enemyState = EnemyState.End;
	}

	void EndTurn()
	{
		tMan.currentTurn.isTurnOver = true;
		if(totalTurn > 0)
			totalTurn--;
		
		if(totalTurn == 0)
		{
			tMan.onGoingTurn = false;
			tMan.NextTurn();
			enemyState = EnemyState.None;
		}
		else
		{
			enemyState = EnemyState.Pick;
		}
	}

	void ResetInRangeTiles()
	{
		// reset previous inRange tiles
		for(int i=0; i < tMap.inRangeTiles.Count; i++)
		{
			tMap.inRangeTiles[i].inRange = false;
		}
		tMap.inRangeTiles.Clear();

		tMap.DrawTransparentTexture ();
	}

	void OnGUI()
	{
		GUI.Label (new Rect (20, 27, 200, 100), "Enemy >> " + enemyState);
	}
}
