using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnStatePlayerTurn : MonoBehaviour {

	Vector3 finalDestination;
	Vector3 previousPosition;
	Quaternion previousRotation;
	//MeshCollider col;
	TileMap tMap;
	TurnManager tMan;
	PathFinder pf;
	BotEnemy pickedEnemy;
	PointerScript pointerScript;
	Vector3 playerPositionOnScreen;

	Transform pointer;

	//Ray rayMouse;
	RaycastHit hitInfo;
	
	int totalTurn;
	bool reset;

	enum PlayerState
	{
		None,
		Pick,
		Menu,
			Move,
			Attack, 
			Wait,
			View,
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
		case PlayerState.Wait:
			Wait();
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
		reset = true;

		for(int i=0; i < tMan.players.Count; i++)
		{
			tMan.players[i].isTurnOver = false;
			tMan.players[i].attackEnabled = true;
			tMan.players[i].moveEnabled = true;
			tMan.players[i].waitEnabled = true;
		}
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
		
		if(Input.GetKeyDown(KeyCode.Z))
		{
			if(Physics.Raycast(pointer.transform.position, -Vector3.up, out hitInfo, Mathf.Infinity))
			{
				if(hitInfo.collider.tag == "Player")
				{
					print("test");
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
			if(!tMan.currentTurn.moveEnabled)
			{
				CancelMove();
			}
			else
				playerState = PlayerState.Pick;
		}

		tMap.DrawTransparentTexture ();

		if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			selectedMenu = NavigateMenu(buttonNames, selectedMenu, "up");
		}
		if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
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
			pointerScript.AdjustPosition(tMan.currentTurn.transform.position);
			playerState = PlayerState.Menu;
		}

		tMap.DetermineAvailableTiles(tMan.currentTurn.tilePosition, tMan.currentTurn.playerClass.TileMove);
		
		/*if (Input.GetMouseButtonDown (0)) 
		{
			if (col.collider.Raycast(rayMouse, out hitInfo, Mathf.Infinity))
			{		
				int destinationTileIndex = tMap.WorldToTilePoint(hitInfo.point);
				finalDestination = tMap.tiles[destinationTileIndex].position;

				if(tMap.tiles[destinationTileIndex].inRange && tMap.tiles[destinationTileIndex].reachable)
				{
					pf.FindPath(tMap.tiles[tMan.currentTurn.tilePosition], tMap.tiles[destinationTileIndex]);
					playerState = PlayerState.Animation;
				}
			}
		}*/

		
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
		tMan.currentTurn.transform.position = previousPosition;
		tMan.currentTurn.transform.rotation = previousRotation;

		tMap.tiles[tMan.currentTurn.tilePosition].reachable = true;	// enable previous occupied tile 								
		tMan.currentTurn.tilePosition = tMap.WorldToTilePoint(previousPosition);
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = false;	// disable current occupied tile

		pointerScript.AdjustPosition(previousPosition);
		tMan.currentTurn.moveEnabled = true;

		playerState = PlayerState.Move;
	}
	
	// update player and tile info after moving
	void PostMoveCondition()
	{
		tMan.currentTurn.moveEnabled = false;
		
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = true;	// enable previous occupied tile 								
		tMan.currentTurn.tilePosition = tMap.WorldToTilePoint (tMan.currentTurn.transform.position);
		tMap.tiles[tMan.currentTurn.tilePosition].reachable = false;	// disable current occupied tile
		
		ResetInRangeTiles();
		print ("player tile: " + tMan.currentTurn.tilePosition);
		playerState = PlayerState.Menu;
	}

	void Attack()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			pointerScript.AdjustPosition(tMan.currentTurn.transform.position);
			playerState = PlayerState.Menu;
		}

		tMap.DetermineAvailableTiles(tMan.currentTurn.tilePosition, tMan.currentTurn.playerClass.TileAttack);

		/*if(Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(rayMouse, out hitInfo, Mathf.Infinity)) 
			{
				if(hitInfo.collider.gameObject.tag == "Enemy")
				{
					BotEnemy chosenEnemy = hitInfo.collider.gameObject.GetComponent<BotEnemy>();
					int indexList = tMan.enemies.IndexOf(chosenEnemy);
					pickedEnemy = tMan.enemies[indexList];
					
					if(tMap.inRangeTiles.Contains(tMap.tiles[pickedEnemy.tilePosition]))
					{
						tMan.tacticScene.SetActive(false);
						Application.LoadLevelAdditive(1);
						PostAttackCondition();
					}
				}
			}
		}*/

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
						tMan.tacticScene.SetActive(false);
						Application.LoadLevelAdditive(1);
						PostAttackCondition();
					}
				} 
			}
		}
	}

	void PostAttackCondition()
	{
		tMan.currentTurn.attackEnabled = false;
		Destroy(pickedEnemy.gameObject);
		tMan.enemies.Remove(pickedEnemy);
		tMap.tiles[pickedEnemy.tilePosition].reachable = true;
		
		ResetInRangeTiles();
	
		tMan.checkBattleResult();

		if(tMan.turnState == TurnManager.TurnState.EndGame)
			playerState = PlayerState.None;
		else if(!tMan.currentTurn.moveEnabled && !tMan.currentTurn.attackEnabled)
			playerState = PlayerState.Wait;
		else
			playerState = PlayerState.Menu;
	}

	void Wait()
	{
		tMan.currentTurn.isTurnOver = true;

		if(totalTurn > 0)
			totalTurn--;
		
		if(totalTurn == 0)
		{
			playerState = PlayerState.End;
		}
		else
		{
			playerState = PlayerState.Pick;
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

		if (playerState == PlayerState.Menu) 
		{

			GUI.enabled = tMan.currentTurn.moveEnabled;
			GUI.SetNextControlName(buttonNames[0]);
			buttons[0] = GUI.Button (new Rect (playerPositionOnScreen.x+20, (Screen.height - playerPositionOnScreen.y)-50, 75, 25), buttonNames[0]);
		
			GUI.enabled = tMan.currentTurn.attackEnabled;
			GUI.SetNextControlName(buttonNames[1]);
			buttons[1] = GUI.Button (new Rect (playerPositionOnScreen.x+20, (Screen.height - playerPositionOnScreen.y)-25, 75, 25), buttonNames[1]);
						
			GUI.enabled = tMan.currentTurn.waitEnabled;
			GUI.SetNextControlName(buttonNames[2]);
			buttons[2] = GUI.Button (new Rect (playerPositionOnScreen.x+20, (Screen.height - playerPositionOnScreen.y), 75, 25), buttonNames[2]);
		
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
	}
}
