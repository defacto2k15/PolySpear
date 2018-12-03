using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour, IPlayerInterface
{
	public GameObject marker;
	private List<Unit> units;

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
	private HexPosition mouse = null;
	private HexPosition selection = null;
	private HexPosition[] path = null;
	bool gameOver = false;
	private bool myTurn = false;

	[ClientRpc]
	public void RpcSetPlayerNumber (int player)
	{
		if (!isLocalPlayer)
			return;
		this.player = player;
		units = new List<Unit> (Object.FindObjectsOfType<Unit> ());
		foreach (Unit unit in units) {
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
			CmdUnitSkipAttack (HexPosition.hexToIntPair (selection));
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
	public void removeUnit (Unit unit)
	{
		if (!isLocalPlayer)	//Should always be local player.
			return;
		units.Remove (unit);
	}

	//Returns true if there are any selectable units.
	[Client]
	private bool selectSelectable ()
	{
		bool nonempty = false;
		foreach (Unit otherUnit in units) {
			if (otherUnit.Player == player && otherUnit.Status != Unit.State.Wait) {
				otherUnit.Coordinates.select ("Selectable");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//TODO: Move to Unit.cs
	[Client]
	private bool isAttackable (Unit attacker, Unit attacked, HexPosition coordinates)
	{
		return attacked.Player != player && coordinates.dist (attacked.Coordinates) <= attacker.Range;
	}

	private bool isAttackable (Unit attacker, Unit attacked)
	{
		return isAttackable (attacker, attacked, attacker.Coordinates);
	}

	//Returns true if there's at least one attackable unit.
	[Client]
	private bool selectAttackable (Unit attacker, HexPosition coordinates)
	{
		bool nonempty = false;
		foreach (Unit otherUnit in units) {
			if (isAttackable (attacker, otherUnit, coordinates)) {
				otherUnit.Coordinates.select ("Attack");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//Returns true if there's at least one attackable unit.
	[Client]
	private bool selectAttackable (Unit attacker)
	{
		return selectAttackable (attacker, attacker.Coordinates);
	}

	[Client]
	private void select ()
	{
		if (mouse.isSelected ("Selectable")) {
			HexPosition.clearSelection ("Selectable");
			selection = mouse;
			mouse.select ("Selection");
			Unit unit = mouse.getUnit ();
			selectAttackable (unit);
			switch (unit.Status) {
			case Unit.State.Move:
				turn = Turn.Move;
				break;
			case Unit.State.Attack:
				turn = Turn.Attack;
				break;
			default:
				Debug.LogError ("Error: Action " + unit.Status + " not implemented.");
				break;
			}
		}
	}

	[Client]
	private void endTurn ()
	{
		HexPosition.clearSelection ();
		myTurn = false;
		CmdEndTurn ();
	}

	[ClientRpc]
	public void RpcBeginTurn ()
	{
		if (isLocalPlayer) {
			foreach (Unit unit in units) {
				unit.newTurn ();
			}
			myTurn = true;
			selectSelectable ();
		}
	}

	[Client]
	private void unselect ()
	{
		HexPosition.clearSelection ();
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
		foreach (Unit unit in units) {
			if (unit.Player != player) {
				gameOver = false;
				break;
			}
		}
	}

	[Client]
	private void actuallyAttack ()
	{
		CmdUnitAttack (HexPosition.hexToIntPair (selection), HexPosition.hexToIntPair (mouse));
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
				HexPosition.clearSelection ();
				selection = mouse;
				selection.select ("Selection");
				CmdMoveUnit (HexPosition.pathToIntString (path));
			}
		} else {
			object enemy = null;
			if (mouse.tryGetValue ("Unit", out enemy)) {
				if (isAttackable (selection.getUnit (), (Unit)enemy)) {
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
	private HexPosition getMouseHex ()
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
			return (new HexPosition (hits [min].point));
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
			HexPosition newMouse = getMouseHex ();
			if (newMouse == null) {
				HexPosition.clearSelection ("Path");
				HexPosition.clearSelection ("Attack");
				path = null;
			} else {
				if (newMouse != mouse) {
					if (mouse != null) {
						mouse.unselect ("Cursor");
					}
					if (newMouse.containsKey ("Obstacle")) {	//The Obstacle tag is being used to make the tile unselectable.
						if (mouse != null && turn == Turn.Move) {
							HexPosition.clearSelection ("Path");
							HexPosition.clearSelection ("Attack");
							path = null;
						}
						mouse = null;
						return;
					}
					mouse = newMouse;
					mouse.select ("Cursor");
					if (turn == Turn.Move) {
						Unit unit = selection.getUnit ();
						HexPosition.clearSelection ("Path");
						HexPosition.clearSelection ("Attack");
						path = AStar.search (selection, mouse, unit.Speed);
						HexPosition.select ("Path", path);
						selectAttackable (unit, mouse);
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
				HexPosition.clearSelection ();
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
		HexPosition.setColor ("Path", Color.yellow, 1);
		HexPosition.setColor ("Selection", Color.green, 2);
		HexPosition.setColor ("Selectable", Color.green, 3);
		HexPosition.setColor ("Attack", Color.red, 4);
		HexPosition.setColor ("Cursor", Color.blue, 5);
		HexPosition.Marker = marker;
		foreach (GameObject child in GameObject.FindGameObjectsWithTag("Obstacle")) {
			HexPosition position = new HexPosition (child.transform.position);
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
		HexPosition[] path = HexPosition.intStringToPath (intString);
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
		Unit unit = HexPosition.intPairToHex (attacker).getUnit ();
		unit.attack (HexPosition.intPairToHex (defender), damage);
	}

	[Command]
	private void CmdUnitAttack (int[] attacker, int[] defender)
	{
		RpcUnitAttack (attacker, defender, HexPosition.intPairToHex (attacker).getUnit ().getDamage ());
	}

	[ClientRpc]
	private void RpcSkipAttack (int[] position)
	{
		HexPosition.intPairToHex (position).getUnit ().skipAttack ();
	}

	[Command]
	private void CmdUnitSkipAttack (int[] position)
	{
		RpcSkipAttack (position);
	}
}
