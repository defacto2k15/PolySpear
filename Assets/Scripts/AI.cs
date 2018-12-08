using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;

public class AI
{
	private List<LegacyUnit> units;
	private int player;
	private LegacyUnit target;
	private LegacyUnit legacyUnit;

	private enum Phase
	{
		Move,
		Attack
	}

	private Phase phase = Phase.Move;

	public AI (List<LegacyUnit> units, int player)
	{
		this.units = units;
		this.player = player;
	}

	private LegacyUnit getPreferredEnemy ()
	{
		LegacyUnit current = null;
		double score = 0;
		foreach (LegacyUnit unit in units) {
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

	private LegacyUnit getNextUnit ()
	{
		LegacyUnit current = null;
		double score = 0;
		foreach (LegacyUnit unit in units) {
			if (unit.Player != player || unit.Status != LegacyUnit.State.Move) {
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

	private LegacyHexPosition[] getPath (LegacyUnit legacyUnit, LegacyUnit enemy)
	{
		LegacyHexPosition[] path = AStar.search (legacyUnit.Coordinates, enemy.Coordinates, 64, legacyUnit.Range);
		if (path == null) {
			return null;
		}
		if (path.Length <= legacyUnit.Speed) {
			return path;
		}
		LegacyHexPosition[] new_path = new LegacyHexPosition[legacyUnit.Speed];
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
			legacyUnit = getNextUnit ();
			if (legacyUnit == null) {
				return true;
			}
			target = getPreferredEnemy ();
			if (target == null) {
				return true;
			}
			LegacyHexPosition[] path = getPath (legacyUnit, target);
			//You can freeze the enemy AI by hiding the desired enemy out of reach. I should fix this eventually,
			//and make it go on to the next target or something. Or try to clear a path to that unit.
			if (path == null) {
				legacyUnit.skipMove ();
			} else {
				legacyUnit.move (path);
			}
			phase = Phase.Attack;
			//If it's the attack phase, attack the target, or if you haven't gotten close enough,
			//find the best enemy in range to attack. If nobody is in range, do nothing.
		} else {
			if (legacyUnit.Coordinates.dist (target.Coordinates) <= legacyUnit.Range) {
				legacyUnit.attack (target.Coordinates, legacyUnit.getDamage ());
				phase = Phase.Move;
			} else {
				target = null;
				double score = 0;
				foreach (LegacyUnit other_unit in units) {
					if (other_unit.Player == player || legacyUnit.Coordinates.dist (other_unit.Coordinates) > legacyUnit.Range) {
						continue;
					}
					double new_score = other_unit.Strength / other_unit.HP;
					if (new_score > score) {
						score = new_score;
						target = other_unit;
					}
				}
				if (target != null) {
					legacyUnit.attack (target.Coordinates, legacyUnit.getDamage ());
				}
				phase = Phase.Move;
			}
		}
		return false;
	}
}
