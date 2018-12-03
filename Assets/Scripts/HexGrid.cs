using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour, IPlayerInterface
{
	public GameObject marker;
	private List<Unit> units;
	private bool waiting = false;

	private enum Turn
	{
		Select,
		Move,
		Attack
	}

	private Turn turn = Turn.Select;
	public int PLAYERS = 2;
	private int player = 0;
	private HexPosition mouse = null;
	private HexPosition selection = null;
	private HexPosition[] path = null;
	private AI ai;
	bool gameOver = false;
	bool modeSelected = false;
	bool computerPlayer;

	public void wait ()
	{
		waiting = true;
	}

	public void moveComplete ()
	{
		waiting = false;
	}

	public void attackComplete ()
	{
		//Since it currently doesn't wait for an attack, this is empty.
	}

	public void addUnit (Unit unit)
	{
		units.Add (unit);
		unit.Coordinates = new HexPosition (unit.transform.position);
	}

	public void removeUnit (Unit unit)
	{
		units.Remove (unit);
	}

	//Returns true if there are any selectable units.
	private bool selectSelectable ()
	{
		bool nonempty = false;
		foreach (Unit unit in units) {
			if (unit.Player == player && unit.Status != Unit.State.Wait) {
				unit.Coordinates.select ("Selectable");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//TODO: Move to Unit.cs
	private bool isAttackable (Unit attacker, Unit attacked, HexPosition coordinates)
	{
		return attacked.Player != player && coordinates.dist (attacked.Coordinates) <= attacker.Range;
	}

	private bool isAttackable (Unit attacker, Unit attacked)
	{
		return isAttackable (attacker, attacked, attacker.Coordinates);
	}

	//Returns true if there's at least one attackable unit.
	private bool selectAttackable (Unit attacker, HexPosition coordinates)
	{
		bool nonempty = false;
		foreach (Unit unit in units) {
			if (isAttackable (attacker, unit, coordinates)) {
				unit.Coordinates.select ("Attack");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//Returns true if there's at least one attackable unit.
	private bool selectAttackable (Unit attacker)
	{
		return selectAttackable (attacker, attacker.Coordinates);
	}

	void Start ()
	{
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
		units = new List<Unit> (Object.FindObjectsOfType<Unit> ());
		foreach (Unit unit in units) {
			unit.setPlayerInterface (this, true);
		}
	}

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
				print ("Error: Action " + unit.Status + " not implemented.");
				break;
			}
		}
	}

	public void endTurn ()
	{
		HexPosition.clearSelection ();
		foreach (Unit unit in units) {	//I only need to do this with units on that team, but checking won't speed things up. I could also only do it when player overflows.
			unit.newTurn ();
		}
		player = (player + 1) % PLAYERS;
		if (player == 0 || !computerPlayer) {
			selectSelectable ();
		}
	}

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

	private void actuallyAttack ()
	{
		Unit unit = selection.getUnit ();
		unit.attack (mouse, unit.getDamage ());
		checkGameOver ();
		unselect ();
	}

	private void move ()
	{
		if (mouse.Equals (selection)) {
			unselect ();
		} else if (!mouse.containsKey ("Unit")) {
			if (path.Length > 0) {
				Unit myUnit = selection.getUnit ();
				myUnit.move (path);
				HexPosition.clearSelection ();
				selection = mouse;
				selection.select ("Selection");
				if (selectAttackable (myUnit)) {
					turn = Turn.Attack;
				} else {
					myUnit.skipAttack ();
					unselect ();
				}
			}
		} else {
			Unit enemy = mouse.getUnit ();
			if (enemy != null) {
				Unit myUnit = selection.getUnit ();
				if (isAttackable (myUnit, enemy)) {
					actuallyAttack ();
				}
			}
		}
	}

	private void attack ()
	{
		if (mouse.isSelected ("Attack")) {
			actuallyAttack ();
		}
	}

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

	void Update ()
	{
		if (waiting || gameOver || !modeSelected) {
			return;
		}
		if (player == 1 && computerPlayer) {
			if (ai.go ()) {
				endTurn ();
			}
			checkGameOver ();
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

	void OnGUI ()
	{
		if (!modeSelected) {
			if (GUI.Button (new Rect (10, 10, 90, 20), "1 Player")) {
				selectSelectable ();
				computerPlayer = true;
				modeSelected = true;
				ai = new AI (units, 1);
				return;
			}
			if (GUI.Button (new Rect (10, 40, 90, 20), "2 Player")) {
				selectSelectable ();
				computerPlayer = false;
				modeSelected = true;
				return;
			}
			return;
		}
		if (gameOver) {
			GUIStyle style = new GUIStyle ();
			style.fontSize = 72;
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Box (new Rect (10, 10, Screen.width - 20, Screen.height - 20), "Player " + (player + 1) + " Wins!", style);
			return;
		}
		if (waiting || (player == 1 && computerPlayer)) {
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
}
