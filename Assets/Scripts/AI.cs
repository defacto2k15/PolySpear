using UnityEngine;
using System.Collections.Generic;

public class AI
{
	private List<Unit> units;
	private int player;
	private Unit target;
	private Unit unit;

	private enum Phase
	{
		Move,
		Attack
	}

	private Phase phase = Phase.Move;

	public AI (List<Unit> units, int player)
	{
		this.units = units;
		this.player = player;
	}

	private Unit getPreferredEnemy ()
	{
		Unit current = null;
		double score = 0;
		foreach (Unit unit in units) {
			if (unit.Player == player) {
				continue;
			}
			double new_score = unit.Strength / unit.HP;
			if (new_score > score) {
				score = new_score;
				current = unit;
			}
		}
		return current;
	}

	private Unit getNextUnit ()
	{
		Unit current = null;
		double score = 0;
		foreach (Unit unit in units) {
			if (unit.Player != player || unit.Status != Unit.State.Move) {
				continue;
			}
			double new_score = unit.Strength;
			if (new_score > score) {
				score = new_score;
				current = unit;
			}
		}
		return current;
	}

	private HexPosition[] getPath (Unit unit, Unit enemy)
	{
		HexPosition[] path = AStar.search (unit.Coordinates, enemy.Coordinates, 64, unit.Range);
		if (path == null) {
			return null;
		}
		if (path.Length <= unit.Speed) {
			return path;
		}
		HexPosition[] new_path = new HexPosition[unit.Speed];
		for (int i = 0; i < new_path.Length; ++i) {
			new_path [i] = path [i];
		}
		return new_path;
	}
	
	//Returns false if there's no pieces left to move.
	public bool go ()
	{
		//If it's the move phase, move the strongest unit that hasn't moved yet towards
		//the enemy with the highest attack to HP ratio.
		if (phase == Phase.Move) {
			unit = getNextUnit ();
			if (unit == null) {
				return true;
			}
			target = getPreferredEnemy ();
			if (target == null) {
				return true;
			}
			HexPosition[] path = getPath (unit, target);
			//You can freeze the enemy AI by hiding the desired enemy out of reach. I should fix this eventually,
			//and make it go on to the next target or something. Or try to clear a path to that unit.
			if (path == null) {
				unit.skipMove ();
			} else {
				unit.move (path);
			}
			phase = Phase.Attack;
			//If it's the attack phase, attack the target, or if you haven't gotten close enough,
			//find the best enemy in range to attack. If nobody is in range, do nothing.
		} else {
			if (unit.Coordinates.dist (target.Coordinates) <= unit.Range) {
				unit.attack (target.Coordinates, unit.getDamage ());
				phase = Phase.Move;
			} else {
				target = null;
				double score = 0;
				foreach (Unit other_unit in units) {
					if (other_unit.Player == player || unit.Coordinates.dist (other_unit.Coordinates) > unit.Range) {
						continue;
					}
					double new_score = other_unit.Strength / other_unit.HP;
					if (new_score > score) {
						score = new_score;
						target = other_unit;
					}
				}
				if (target != null) {
					unit.attack (target.Coordinates, unit.getDamage ());
				}
				phase = Phase.Move;
			}
		}
		return false;
	}
}
