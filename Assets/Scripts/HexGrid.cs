using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class HexGrid : MonoBehaviour, IPlayerInterface
{
	public GameObject marker;
	private List<LegacyUnit> units;
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
	private LegacyHexPosition mouse = null;
	private LegacyHexPosition selection = null;
	private LegacyHexPosition[] path = null;
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

	public void addUnit (LegacyUnit legacyUnit)
	{
		units.Add (legacyUnit);
		legacyUnit.Coordinates = new LegacyHexPosition (legacyUnit.transform.position);
	}

	public void removeUnit (LegacyUnit legacyUnit)
	{
		units.Remove (legacyUnit);
	}

	//Returns true if there are any selectable units.
	private bool selectSelectable ()
	{
		bool nonempty = false;
		foreach (LegacyUnit unit in units) {
			if (unit.Player == player && unit.Status != LegacyUnit.State.Wait) {
				unit.Coordinates.select ("Selectable");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//TODO: Move to Unit.cs
	private bool isAttackable (LegacyUnit attacker, LegacyUnit attacked, LegacyHexPosition coordinates)
	{
		return attacked.Player != player && coordinates.dist (attacked.Coordinates) <= attacker.Range;
	}

	private bool isAttackable (LegacyUnit attacker, LegacyUnit attacked)
	{
		return isAttackable (attacker, attacked, attacker.Coordinates);
	}

	//Returns true if there's at least one attackable unit.
	private bool selectAttackable (LegacyUnit attacker, LegacyHexPosition coordinates)
	{
		bool nonempty = false;
		foreach (LegacyUnit unit in units) {
			if (isAttackable (attacker, unit, coordinates)) {
				unit.Coordinates.select ("Attack");
				nonempty = true;
			}
		}
		return nonempty;
	}

	//Returns true if there's at least one attackable unit.
	private bool selectAttackable (LegacyUnit attacker)
	{
		return selectAttackable (attacker, attacker.Coordinates);
	}

	void Start ()
	{
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
		units = new List<LegacyUnit> (Object.FindObjectsOfType<LegacyUnit> ());
		foreach (LegacyUnit unit in units) {
			unit.setPlayerInterface (this, true);
		}
	}

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
				print ("Error: Action " + legacyUnit.Status + " not implemented.");
				break;
			}
		}
	}

	public void endTurn ()
	{
		LegacyHexPosition.clearSelection ();
		foreach (LegacyUnit unit in units) {	//I only need to do this with units on that team, but checking won't speed things up. I could also only do it when player overflows.
			unit.newTurn ();
		}
		player = (player + 1) % PLAYERS;
		if (player == 0 || !computerPlayer) {
			selectSelectable ();
		}
	}

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

	private void actuallyAttack ()
	{
		LegacyUnit legacyUnit = selection.getUnit ();
		legacyUnit.attack (mouse, legacyUnit.getDamage ());
		checkGameOver ();
		unselect ();
	}

	private void move ()
	{
		if (mouse.Equals (selection)) {
			unselect ();
		} else if (!mouse.containsKey ("Unit")) {
			if (path.Length > 0) {
				LegacyUnit myLegacyUnit = selection.getUnit ();
				myLegacyUnit.move (path);
				LegacyHexPosition.clearSelection ();
				selection = mouse;
				selection.select ("Selection");
				if (selectAttackable (myLegacyUnit)) {
					turn = Turn.Attack;
				} else {
					myLegacyUnit.skipAttack ();
					unselect ();
				}
			}
		} else {
			LegacyUnit enemy = mouse.getUnit ();
			if (enemy != null) {
				LegacyUnit myLegacyUnit = selection.getUnit ();
				if (isAttackable (myLegacyUnit, enemy)) {
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
}
