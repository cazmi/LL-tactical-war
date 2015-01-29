using UnityEngine;
using System.Collections;

public class TurnStatePlayerTurn : MonoBehaviour {

	Vector3 finalDestination;
	Vector3 previousPosition;
	Quaternion previousRotation;

	TileMap tMap;
	TurnManager tMan;
	WarSceneManager wMan;
	PathFinder pf;

	BotEnemy pickedEnemy;
	Vector3 playerPositionOnScreen;

	//Ray rayMouse;
	Transform pointer;	
	PointerScript pointerScript;
	RaycastHit hitInfo;
	
	Player highlightPlayer;
	bool showPlayerInfo;
	
	int totalTurn;
	bool reset;

	enum PlayerState
	{
		None,
		Pick,
		Menu,
			Move,
			Attack,
			InBattle,
			Wait,
			Facing,
		Animation,
		End
	}	
	PlayerState playerState;

	string[] buttonNames = new string[3] {"Move", "Attack", "Wait"};
	bool[] buttons = new bool[3];
	int selectedMenu;

	void Awake()
	{
		tMap = TileMap.instance;
		tMan = TurnManager.instance;
		wMan = WarSceneManager.instance;
		pf = gameObject.AddComponent<PathFinder> ();
		//col = GameObject.Find ("TileMap").GetComponent<MeshCollider> ();
		
		pointer = GameObject.Find ("Pointer").GetComponent<Transform>();
		pointerScript = GameObject.Find ("Pointer").GetComponent<PointerScript> ();
	}

	public void EnterPickState()
	{
		tMan.onGoingTurn = true;	
		reset = false;
		totalTurn = tMan.players.Count;
		playerState = PlayerState.Pick;
	}
	
	// Update is called once per frame
	void Update () {
		//rayMouse = Camera.main.ScreenPointToRay (Input.mousePosition);

		switch (playerState) 
		{
		case PlayerState.None:
			if(!reset) ResetPlayerInfo();
			break;
		case PlayerState.Pick:			
			pointerScript.gameObject.SetActive(true);
			selectedMenu = 0;

			PickPlayer();
			break;
		case PlayerState.Menu:
			pointerScript.gameObject.SetActive(false);
			pointerScript.AdjustPosition(tMan.currentTurn.transform.position);

			InMenu();
			break;
		case PlayerState.Move:
			pointerScript.gameObject.SetActive(true);

			Move();
			break;
		case PlayerState.Attack:
			pointerScript.gameObject.SetActive(true);

			Attack();
			break;
		case PlayerState.InBattle:
			WaitForBattleToFinish();
			break;
		case PlayerState.Wait:
			Wait();
			break;
		case PlayerState.Facing:
			pointerScript.gameObject.SetActive(false);
			pointerScript.AdjustPosition(tMan.currentTurn.transform.position);

			ChooseDirection();
			break;
		case PlayerState.Animation:
			MovingAnimation();
			break;
		case PlayerState.End:
			EndTurn();
			break;
		}
	}

	void ResetPlayerInfo()
	{
		for(int i=0; i < tMan.players.Count; i++)
		{
			tMan.players[i].isTurnOver = false;
			tMan.players[i].attackEnabled = true;
			tMan.players[i].moveEnabled = true;
			tMan.players[i].waitEnabled = true;
		}

		reset = true;
	}

	void PickPlayer()
	{
		/*if(Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(rayMouse, out hitInfo, Mathf.Infinity)) 
			{
				if(hitInfo.collider.gameObject.tag == "Player")
				{
					HumanPlayer chosenPlayer = hitInfo.collider.gameObject.GetComponent<HumanPlayer>();
					int indexList = tMan.players.IndexOf(chosenPlayer);
					tMan.currentTurn = tMan.players[indexList];

					if(!tMan.currentTurn.isTurnOver)
						playerState = PlayerState.Menu;
				}
			}
		}*/
		if(Physics.Raycast(pointer.transform.position, -Vector3.up, out hitInfo, Mathf.Infinity))
		{
			if(hitInfo.collider.tag == "Player" || hitInfo.collider.tag == "Enemy")
			{
				showPlayerInfo = true;
				highlightPlayer = hitInfo.collider.gameObject.GetComponent<Player>();
			}
			else
			{
				showPlayerInfo = false;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Z))
		{
			if(Physics.Raycast(pointer.transform.position, -Vector3.up, out hitInfo, Mathf.Infinity))
			{
				if(hitInfo.collider.tag == "Player")
				{
					HumanPlayer chosenPlayer = hitInfo.collider.gameObject.GetComponent<HumanPlayer>();
					int indexList = tMan.players.IndexOf(chosenPlayer);
					tMan.currentTurn = tMan.players[indexList];

					if(!tMan.currentTurn.isTurnOver)
						playerState = PlayerState.Menu;
				}
			}
		}
	}

	void InMenu()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			// if player has not committed any action, player can go back to pick state
			if(tMan.currentTurn.moveEnabled && tMan.currentTurn.attackEnabled)
			{
				playerState = PlayerState.Pick;
			}
			// if player has moved, player can go back to previous potition
			else if(!tMan.currentTurn.moveEnabled)
			{
				CancelMove();
			}
		}

		tMap.DrawTransparentTexture ();

		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			selectedMenu = NavigateMenu(buttonNames, selectedMenu, "up");
		}
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			selectedMenu = NavigateMenu(buttonNames, selectedMenu, "down");
		}
	}

	int NavigateMenu(string[] buttonsArray, int selectedItem, string direction)
	{
		if(direction == "down")
		{
			if(selectedItem == buttonsArray.Length - 1)
				selectedItem = 0;
			else
				selectedItem += 1;
		}

		if(direction == "up")
		{
			if(selectedItem == 0)
				selectedItem = buttonsArray.Length - 1;
			else
				selectedItem -= 1;
		}

		return selectedItem;
	}

	void Move()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			playerState = PlayerState.Menu;
		}

		tMap.DetermineAvailableTiles(tMan.currentTurn.tilePosition, tMan.currentTurn.playerClass.TileMove);

		previousPosition = tMan.currentTurn.transform.position;
		previousRotation = tMan.currentTurn.transform.rotation;
		
		if(Input.GetKeyDown(KeyCode.Z))
		{
			if(Physics.Raycast(pointer.transform.position, -Vector3.up, out hitInfo, Mathf.Infinity))
			{
				int destinationTileIndex = tMap.WorldToTilePoint(hitInfo.point);
				finalDestination = tMap.tiles[destinationTileIndex].position;
				
				if(tMap.tiles[destinationTileIndex].inRange && tMap.tiles[destinationTileIndex].reachable)
				{
					pf.FindPath(tMap.tiles[tMan.currentTurn.tilePosition], tMap.tiles[destinationTileIndex]);
					playerState = PlayerState.Animation;
				}
			}
		}

	}

	void MovingAnimation()
	{
		if(pf.GetPathList().Count != 0)
		{
			tMan.currentTurn.Move(pf.GetPathList());
		}
		if (tMan.currentTurn.transform.position == finalDestination) 
		{	
			PostMoveCondition();
		}
	}

	// restore state before moving
	void CancelMove()
	{
		tMan.currentTurn.transform.position = previousPosition;		// <<<<<<< can sometimes cause inaccurate position
		tMan.currentTurn.transform.rotation = previousRotation;

		tMap.tiles[tMan.currentTurn.tilePosition].reachable = true;	// enable previous occupied tile 								
		tMan.currentTurn.tilePosition = tMap.WorldToTilePoint(previousPosition);
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = false;	// disable current occupied tile

		//pointerScript.AdjustPosition(previousPosition);
		tMan.currentTurn.moveEnabled = true;

		playerState = PlayerState.Move;
	}
	
	// update player and tile info after moving
	void PostMoveCondition()
	{
		tMan.currentTurn.moveEnabled = false;
		
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = true;			// enable previous occupied tile 								
		tMan.currentTurn.tilePosition = tMap.WorldToTilePoint (tMan.currentTurn.transform.position);
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = false;		// disable current occupied tile

		ResetInRangeTiles();
		playerState = PlayerState.Menu;
	}

	void Attack()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			playerState = PlayerState.Menu;
		}

		tMap.DetermineAvailableTiles(tMan.currentTurn.tilePosition, tMan.currentTurn.playerClass.TileAttack);

		// adjust player to look at the cursor direction
		Vector3 lookAt = pointer.transform.position;
		lookAt.y = tMan.currentTurn.transform.position.y;
		tMan.currentTurn.transform.LookAt(lookAt);
		print (tMan.currentTurn.transform.eulerAngles.y);
		if(Mathf.Round(tMan.currentTurn.transform.eulerAngles.y) == 0)
		{
			tMan.currentTurn.playerDirection = Player.Direction.North;
		}
		else if(Mathf.Round(tMan.currentTurn.transform.eulerAngles.y) == 90)
		{
			tMan.currentTurn.playerDirection = Player.Direction.East;
		}
		else if(Mathf.Round(tMan.currentTurn.transform.eulerAngles.y) == 180)
		{
			tMan.currentTurn.playerDirection = Player.Direction.South;
		}
		else if(Mathf.Round(tMan.currentTurn.transform.eulerAngles.y) == 270)
		{
			tMan.currentTurn.playerDirection = Player.Direction.West;
		}

		if(Input.GetKeyDown(KeyCode.Z))
		{
			if(Physics.Raycast(pointer.transform.position, -Vector3.up, out hitInfo, Mathf.Infinity))
			{
				if(hitInfo.collider.gameObject.tag == "Enemy")
				{
					BotEnemy chosenEnemy = hitInfo.collider.gameObject.GetComponent<BotEnemy>();
					int indexList = tMan.enemies.IndexOf(chosenEnemy);
					pickedEnemy = tMan.enemies[indexList];

					if(tMap.inRangeTiles.Contains(tMap.tiles[pickedEnemy.tilePosition]))
					{
						// check the player's direction and enemy's to determine side/back attack bonus
						int angle = Mathf.RoundToInt(Quaternion.Angle(tMan.currentTurn.transform.rotation, pickedEnemy.transform.rotation));
						if(angle == 0)
						{
							print ("back attack!");
							tMan.currentTurn.modifiedAttack += 10;
						}
						else if(angle == 90)
						{
							print ("side attack!");
							tMan.currentTurn.modifiedAttack += 5;
						}
						else if(angle == 180)
						{
							print("normal attack!");
						}

						// hide tactic scene and show war scene
						tMan.tacticScene.SetActive(false);
						TurnManager.instance.warScene.SetActive(true);
						WarSceneManager.instance.InitializeWar(tMan.currentTurn.gameObject, pickedEnemy.gameObject);

						playerState = PlayerState.InBattle;
					}
				} 
			}
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
		tMan.currentTurn.currentHealth = wMan.playerGeneral.currentHealth;
		tMan.currentTurn.currentUnits = wMan.playerTroops.Count - 1;

		pickedEnemy.currentHealth = wMan.enemyGeneral.currentHealth;
		pickedEnemy.currentUnits = wMan.enemyTroops.Count - 1;

		if(tMan.currentTurn.currentHealth <= 0)
		{
			Destroy(tMan.currentTurn.gameObject);
			tMan.players.Remove((HumanPlayer)tMan.currentTurn);
			tMap.tiles[tMan.currentTurn.tilePosition].reachable = true;

			playerState = PlayerState.Wait;
		}
		else
		{
			if(!tMan.currentTurn.moveEnabled && !tMan.currentTurn.attackEnabled)
			{
				playerState = PlayerState.Wait;
			}
			else
			{
				playerState = PlayerState.Menu;
			}
		}

		if(pickedEnemy.currentHealth <= 0)
		{
			Destroy(pickedEnemy.gameObject);
			tMan.enemies.Remove(pickedEnemy);
			tMap.tiles[pickedEnemy.tilePosition].reachable = true;
		}

		tMan.checkTacticResult();
		
		if(tMan.turnState == TurnManager.TurnState.EndGame)
		{
			playerState = PlayerState.None;
		}
	}

	void Wait()
	{
		tMan.currentTurn.isTurnOver = true;
		
		if(totalTurn > 0)
			totalTurn--;

		if(tMan.currentTurn == null)
		{
			if(totalTurn == 0)
			{
				playerState = PlayerState.End;
			}
			else
			{
				playerState = PlayerState.Pick;
			}
		}
		else
		{
			playerState = PlayerState.Facing;
		}
	}	

	void ChooseDirection()
	{
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			tMan.currentTurn.Rotate("north");
		}
		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			tMan.currentTurn.Rotate("east");
		}
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			tMan.currentTurn.Rotate("south");
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			tMan.currentTurn.Rotate("west");
		}

		if(Input.GetKeyDown(KeyCode.Z))
		{
			if(totalTurn == 0)
			{
				playerState = PlayerState.End;
			}
			else
			{
				playerState = PlayerState.Pick;
			}
		}
	}

	void EndTurn()
	{		
		tMan.onGoingTurn = false;
		tMan.NextTurn();
		playerState = PlayerState.None;

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
		if (tMan.currentTurn != null && tMan.players.Count != 0) {
			playerPositionOnScreen = Camera.main.WorldToScreenPoint (tMan.currentTurn.transform.position);
		}

		GUI.Label (new Rect (20, 15, 200, 100), "Player >> " + playerState);

		
		if(playerState == PlayerState.Pick)
		{
			if(showPlayerInfo)
			{
				GUI.Box (new Rect(10, Screen.height * 0.8f, 100, 50), "HP : " + highlightPlayer.currentHealth + "\n" +
			         "Units : " + highlightPlayer.currentUnits);
			}
		}

		if (playerState == PlayerState.Menu) 
		{
			GUI.enabled = tMan.currentTurn.moveEnabled;
			GUI.SetNextControlName(buttonNames[0]);
			buttons[0] = GUI.Button (new Rect (playerPositionOnScreen.x+20, (Screen.height - playerPositionOnScreen.y)-50, 75, 25), buttonNames[0]);
		
			GUI.enabled = tMan.currentTurn.attackEnabled;
			GUI.SetNextControlName(buttonNames[1]);
			buttons[1] = GUI.Button (new Rect (playerPositionOnScreen.x+20, playerPositionOnScreen.y-25, 75, 25), buttonNames[1]);
						
			GUI.enabled = tMan.currentTurn.waitEnabled;
			GUI.SetNextControlName(buttonNames[2]);
			buttons[2] = GUI.Button (new Rect (playerPositionOnScreen.x+20, playerPositionOnScreen.y, 75, 25), buttonNames[2]);
		
			if(Input.GetKeyDown(KeyCode.Z))
			{
				buttons[selectedMenu] = true;
			}

			if(buttons[0] && tMan.currentTurn.moveEnabled)
			{
				playerState = PlayerState.Move;
			}
			if(buttons[1] && tMan.currentTurn.attackEnabled)
			{
				playerState = PlayerState.Attack;
			}
			if(buttons[2])
			{
				playerState = PlayerState.Wait;
			}

			GUI.FocusControl(buttonNames[selectedMenu]);
		}

		if(playerState == PlayerState.Facing)
		{
			GUI.Toggle(new Rect(playerPositionOnScreen.x - 7, playerPositionOnScreen.y, 75, 25), tMan.currentTurn.playerDirection == Player.Direction.South, "");
			GUI.Toggle(new Rect(playerPositionOnScreen.x - 7, playerPositionOnScreen.y - 120, 75, 25), tMan.currentTurn.playerDirection == Player.Direction.North, "");
			GUI.Toggle(new Rect(playerPositionOnScreen.x - 60, playerPositionOnScreen.y - 60, 75, 25), tMan.currentTurn.playerDirection == Player.Direction.West, "");
			GUI.Toggle(new Rect(playerPositionOnScreen.x + 50, playerPositionOnScreen.y - 60, 75, 25), tMan.currentTurn.playerDirection == Player.Direction.East, "");
		}
	}
}
