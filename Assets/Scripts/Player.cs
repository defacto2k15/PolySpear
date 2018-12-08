using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class Player : NetworkBehaviour, IPlayerInterface
{
	public GameObject marker;
	private List<LegacyUnit> units;

	private bool waiting = false;
	//private int timeout = 0;		//waiting is still false as this counts down.
	//private const int MAX_TIME = 1000;

	private enum Turn
	{
		Select,
		Move,
		Attack
	}

	private Turn turn = Turn.Select;
	private int player;
	private LegacyHexPosition mouse = null;
	private LegacyHexPosition selection = null;
	private LegacyHexPosition[] path = null;
	bool gameOver = false;
	private bool myTurn = false;

	[ClientRpc]
	public void RpcSetPlayerNumber (int player)
	{
		if (!isLocalPlayer)
			return;
		this.player = player;
		units = new List<LegacyUnit> (Object.FindObjectsOfType<LegacyUnit> ());
		foreach (LegacyUnit unit in units) {
			unit.setPlayerInterface (this, unit.Player == player);
		}
	}

	[Client]
	public void moveComplete ()
	{
		if (!isLocalPlayer)
			return;
		waiting = false;
		if (selectAttackable (selection.getUnit ())) {
			turn = Turn.Attack;
		} else {
			CmdUnitSkipAttack (LegacyHexPosition.hexToIntPair (selection));
			unselect ();
		}
	}

	[Client]
	public void attackComplete ()
	{
		if (!isLocalPlayer)
			return;
		waiting = false;
		checkGameOver ();
		unselect ();
	}

	[Client]
	public void removeUnit (LegacyUnit legacyUnit)
	{
		if (!isLocalPlayer)	//Should always be local player.
			return;
		units.Remove (legacyUnit);
	}

	//Returns true if there are any selectable units.
	[Client]
	private bool selectSelectable ()
	{
		bool nonempty = false;
		foreach (LegacyUnit otherUnit in units) {
			if (otherUnit.Player == player && otherUnit.Status != LegacyUnit.State.Wait) {
				otherUnit.Coordinates.select ("Selectable");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//TODO: Move to Unit.cs
	[Client]
	private bool isAttackable (LegacyUnit attacker, LegacyUnit attacked, LegacyHexPosition coordinates)
	{
		return attacked.Player != player && coordinates.dist (attacked.Coordinates) <= attacker.Range;
	}

	private bool isAttackable (LegacyUnit attacker, LegacyUnit attacked)
	{
		return isAttackable (attacker, attacked, attacker.Coordinates);
	}

	//Returns true if there's at least one attackable unit.
	[Client]
	private bool selectAttackable (LegacyUnit attacker, LegacyHexPosition coordinates)
	{
		bool nonempty = false;
		foreach (LegacyUnit otherUnit in units) {
			if (isAttackable (attacker, otherUnit, coordinates)) {
				otherUnit.Coordinates.select ("Attack");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//Returns true if there's at least one attackable unit.
	[Client]
	private bool selectAttackable (LegacyUnit attacker)
	{
		return selectAttackable (attacker, attacker.Coordinates);
	}

	[Client]
	private void select ()
	{
		if (mouse.isSelected ("Selectable")) {
			LegacyHexPosition.clearSelection ("Selectable");
			selection = mouse;
			mouse.select ("Selection");
			LegacyUnit legacyUnit = mouse.getUnit ();
			selectAttackable (legacyUnit);
			switch (legacyUnit.Status) {
			case LegacyUnit.State.Move:
				turn = Turn.Move;
				break;
			case LegacyUnit.State.Attack:
				turn = Turn.Attack;
				break;
			default:
				Debug.LogError ("Error: Action " + legacyUnit.Status + " not implemented.");
				break;
			}
		}
	}

	[Client]
	private void endTurn ()
	{
		LegacyHexPosition.clearSelection ();
		myTurn = false;
		CmdEndTurn ();
	}

	[ClientRpc]
	public void RpcBeginTurn ()
	{
		if (isLocalPlayer) {
			foreach (LegacyUnit unit in units) {
				unit.newTurn ();
			}
			myTurn = true;
			selectSelectable ();
		}
	}

	[Client]
	private void unselect ()
	{
		LegacyHexPosition.clearSelection ();
		selection = null;
		mouse.select ("Cursor");
		if (!(selectSelectable () || gameOver)) {
			endTurn ();
		}
		turn = Turn.Select;
	}

	[Client]
	private void checkGameOver ()
	{
		gameOver = true;
		foreach (LegacyUnit unit in units) {
			if (unit.Player != player) {
				gameOver = false;
				break;
			}
		}
	}

	[Client]
	private void actuallyAttack ()
	{
		CmdUnitAttack (LegacyHexPosition.hexToIntPair (selection), LegacyHexPosition.hexToIntPair (mouse));
		waiting = true;
	}

	[Client]
	private void move ()
	{
		if (mouse.Equals (selection)) {
			unselect ();
		} else if (!mouse.containsKey ("Unit")) {
			if (path.Length > 0) {
				waiting = true;
				LegacyHexPosition.clearSelection ();
				selection = mouse;
				selection.select ("Selection");
				CmdMoveUnit (LegacyHexPosition.pathToIntString (path));
			}
		} else {
			object enemy = null;
			if (mouse.tryGetValue ("Unit", out enemy)) {
				if (isAttackable (selection.getUnit (), (LegacyUnit)enemy)) {
					actuallyAttack ();
				}
			}
		}
	}

	[Client]
	private void attack ()
	{
		if (mouse.isSelected ("Attack")) {
			actuallyAttack ();
		}
	}

	[Client]
	private LegacyHexPosition getMouseHex ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll (ray);
		if (hits.Length == 0) {
			return null;
		} else {
			float minDist = float.PositiveInfinity;
			int min = 0;
			for (int i = 0; i < hits.Length; ++i) {
				if (hits [i].distance < minDist) {
					minDist = hits [i].distance;
					min = i;
				}
			}
			return (new LegacyHexPosition (hits [min].point));
		}
	}

	[Client]
	void Update ()
	{
		if (!isLocalPlayer || !myTurn || waiting) {
			return;
		}
		if (!Input.mousePresent) {
			mouse = null;
		} else {
			LegacyHexPosition newMouse = getMouseHex ();
			if (newMouse == null) {
				LegacyHexPosition.clearSelection ("Path");
				LegacyHexPosition.clearSelection ("Attack");
				path = null;
			} else {
				if (newMouse != mouse) {
					if (mouse != null) {
						mouse.unselect ("Cursor");
					}
					if (newMouse.containsKey ("Obstacle")) {	//The Obstacle tag is being used to make the tile unselectable.
						if (mouse != null && turn == Turn.Move) {
							LegacyHexPosition.clearSelection ("Path");
							LegacyHexPosition.clearSelection ("Attack");
							path = null;
						}
						mouse = null;
						return;
					}
					mouse = newMouse;
					mouse.select ("Cursor");
					if (turn == Turn.Move) {
						LegacyUnit legacyUnit = selection.getUnit ();
						LegacyHexPosition.clearSelection ("Path");
						LegacyHexPosition.clearSelection ("Attack");
						path = AStar.search (selection, mouse, legacyUnit.Speed);
						LegacyHexPosition.select ("Path", path);
						selectAttackable (legacyUnit, mouse);
					}
				}
				if (Input.GetButtonDown ("Fire1")) {
					switch (turn) {
					case Turn.Select:
						select ();
						break;
					case Turn.Move:
						move ();
						break;
					case Turn.Attack:
						attack ();
						break;
					default:
						print ("Error: Turn " + turn + " not implemented.");
						break;
					}
					return;
				}
			}
		}
	}

	[Client]
	void OnGUI ()
	{
		if (!isLocalPlayer)
			return;
		if (gameOver) {
			GUIStyle style = new GUIStyle ();
			style.fontSize = 72;
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Box (new Rect (10, 10, Screen.width - 20, Screen.height - 20), "Player " + (player + 1) + " Wins!", style);
			return;
		}
		if (!myTurn) {
			GUI.Box (new Rect (10, 10, 150, 20), "Waiting for your turn...");
			return;
		}
		if (waiting) {
			return;
		}
		GUI.Box (new Rect (10, 10, 90, 20), "Player " + (player + 1));
		switch (turn) {
		case Turn.Select:
			GUI.Box (new Rect (10, 40, 90, 20), "Select");
			if (GUI.Button (new Rect (10, 70, 90, 20), "End Turn")) {
				endTurn ();
			}
			break;
		case Turn.Move:
			GUI.Box (new Rect (10, 40, 90, 20), "Move");
			if (GUI.Button (new Rect (10, 70, 90, 20), "Cancel Move")) {
				unselect ();
			}
			break;
		case Turn.Attack:
			GUI.Box (new Rect (10, 40, 90, 20), "Attack");
			if (GUI.Button (new Rect (10, 70, 90, 20), "Skip Attack")) {
				LegacyHexPosition.clearSelection ();
				selection = null;
				if (mouse != null) {
					mouse.select ("Cursor");
				}
				selectSelectable ();
				turn = Turn.Select;
			}
			break;
		}
	}

	[Client]
	override public void OnStartLocalPlayer ()
	{
		Object.Destroy (GameObject.FindGameObjectWithTag ("Network Manager HUD").GetComponents<Component> () [2]);
		LegacyHexPosition.setColor ("Path", Color.yellow, 1);
		LegacyHexPosition.setColor ("Selection", Color.green, 2);
		LegacyHexPosition.setColor ("Selectable", Color.green, 3);
		LegacyHexPosition.setColor ("Attack", Color.red, 4);
		LegacyHexPosition.setColor ("Cursor", Color.blue, 5);
		LegacyHexPosition.Marker = marker;
		foreach (GameObject child in GameObject.FindGameObjectsWithTag("Obstacle")) {
			LegacyHexPosition position = new LegacyHexPosition (child.transform.position);
			child.transform.position = position.getPosition ();
			position.flag ("Obstacle");
		}
		CmdAddPlayer ();
	}

	[Command]
	private void CmdAddPlayer ()
	//override public void OnStartServer ()
	{
		ServerGameController.getSingleton ().addPlayer (this);
	}

	[Command]
	private void CmdEndTurn ()
	{
		ServerGameController.getSingleton ().endTurn ();
	}

	[ClientRpc]
	private void RpcMoveUnit (int[] intString)
	{
		LegacyHexPosition[] path = LegacyHexPosition.intStringToPath (intString);
		path [0].getUnit ().move (path);
	}

	[Command]
	private void CmdMoveUnit (int[] path)
	{
		//foreach (Player player in ServerGameController.getSingleton().getPlayers()) {
		RpcMoveUnit (path);
		//}
	}

	[ClientRpc]
	private void RpcUnitAttack (int[] attacker, int[] defender, int damage)
	{
		LegacyUnit legacyUnit = LegacyHexPosition.intPairToHex (attacker).getUnit ();
		legacyUnit.attack (LegacyHexPosition.intPairToHex (defender), damage);
	}

	[Command]
	private void CmdUnitAttack (int[] attacker, int[] defender)
	{
		RpcUnitAttack (attacker, defender, LegacyHexPosition.intPairToHex (attacker).getUnit ().getDamage ());
	}

	[ClientRpc]
	private void RpcSkipAttack (int[] position)
	{
		LegacyHexPosition.intPairToHex (position).getUnit ().skipAttack ();
	}

	[Command]
	private void CmdUnitSkipAttack (int[] position)
	{
		RpcSkipAttack (position);
	}
}
