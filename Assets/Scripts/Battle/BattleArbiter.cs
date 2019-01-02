using System;
using Assets.Scripts.Game;
using Assets.Scripts.Map;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Battle
{
    public class BattleArbiter
    {
        private UnitsContainer _units;
        private ProjectilesContainer _projectiles;
        private MapModel _mapModel;

        public BattleArbiter(UnitsContainer units, ProjectilesContainer projectiles, MapModel mapModel)
        {
            _units = units;
            _projectiles = projectiles;
            _mapModel = mapModel;
        }

        public BattleResults PerformBattleAtPlace(MyHexPosition battleActivatorPosition)
        {
            var battleResults = new BattleResults();

            var intruderUnit = _units.GetPawnAt(battleActivatorPosition);

            // First - Passive effects of all neighbours
            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (_mapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (_units.IsPawnAt(pair.NeighbourPosition))
                    {
                        var neighbourUnit = _units.GetPawnAt(pair.NeighbourPosition);
                        if (!battleResults.UnitIncapaciated(neighbourUnit))
                        {
                            battleResults.Add(PerformSingleFight(intruderUnit, neighbourUnit, pair.NeighbourDirection, symbol => symbol.PassiveEffect));
                            if (battleResults.UnitIncapaciated(intruderUnit))
                            {
                                return battleResults;
                            }
                        }
                    }
                }
            }

            // Then - our actions
            foreach (var orientation in OrientationUtils.AllOrientationsClockwise)
            {
                battleResults.Add(PerformSingleActiveFight(intruderUnit, orientation));
                if (battleResults.UnitIncapaciated(intruderUnit))
                {
                    return battleResults;
                }
            }
            return battleResults;
        }

        public BattleResults PerformPassiveBattleAtPlace(MyHexPosition battleActivatorPosition)
        {
            var battleResults = new BattleResults();

            var intruderUnit = _units.GetPawnAt(battleActivatorPosition);

            // First - Passive effects of all neighbours
            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (_mapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (_units.IsPawnAt(pair.NeighbourPosition))
                    {
                        var neighbourUnit = _units.GetPawnAt(pair.NeighbourPosition);
                        if (!battleResults.UnitIncapaciated(neighbourUnit))
                        {
                            battleResults.Add(PerformSingleFight(intruderUnit, neighbourUnit, pair.NeighbourDirection, symbol => symbol.PassiveEffect));
                            if (battleResults.UnitIncapaciated(intruderUnit))
                            {
                                return battleResults;
                            }
                        }
                    }
                }
            }

            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (_mapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (_units.IsPawnAt(pair.NeighbourPosition))
                    {
                        var neighbourUnit = _units.GetPawnAt(pair.NeighbourPosition);
                        if (!battleResults.UnitIncapaciated(neighbourUnit))
                        {
                            battleResults.Add(PerformSingleFight(neighbourUnit, intruderUnit, pair.NeighbourDirection.Opposite(), symbol => symbol.PassiveEffect));
                            if (battleResults.UnitIncapaciated(intruderUnit))
                            {
                                return battleResults;
                            }
                        }
                    }
                }
            }
            return battleResults;
        }

        private BattleResults PerformSingleFight(UnitModel defender, UnitModel attacker, Orientation attackerDirectionFromDefender,
            Func<SymbolModel, IEffect> attackingEffectExtractor)
        {
            var battleResults = new BattleResults();

            if (attacker.Owner == defender.Owner) // no friendly fire for now
            {
                return battleResults;
            }

            var attackerSymbolLocalDirection = attacker.Orientation.CalculateLocalDirection(attackerDirectionFromDefender.Opposite());
            var defendingSymbolLocalDirection = defender.Orientation.CalculateLocalDirection(attackerDirectionFromDefender);

            if (attacker.Symbols.ContainsKey(attackerSymbolLocalDirection))
            {
                var symbol = attacker.Symbols[attackerSymbolLocalDirection];
                var effect = attackingEffectExtractor(symbol);

                var attackerBattlefieldVision = new BattlefieldVision(attacker, attackerSymbolLocalDirection, _units, _mapModel);
                if (effect.IsActivated(attackerBattlefieldVision, attacker.Position))
                {
                    effect.Execute(attackerBattlefieldVision, attacker.Position, battleResults);
                }
                if (!effect.IsDefendableEffect) return battleResults;
                if (defender.Symbols.ContainsKey(defendingSymbolLocalDirection))
                {
                    var defenderBattlefieldVision = new BattlefieldVision(defender,defendingSymbolLocalDirection, _units, _mapModel);
                    var defenderEffect = defender.Symbols[defendingSymbolLocalDirection].ReactEffect;
                    if (defenderEffect.IsActivated(defenderBattlefieldVision, attacker.Position))
                    {
                        defenderEffect.Execute(defenderBattlefieldVision, attacker.Position, battleResults);
                    }
                }
            }
            return battleResults;
        }

        private BattleResults PerformSingleActiveFight(UnitModel attacker, Orientation orientation)
        {
            var battleResults = new BattleResults();
            var attackerSymbolLocalDirection = attacker.Orientation.CalculateLocalDirection(orientation);
            if (!attacker.Symbols.ContainsKey(attackerSymbolLocalDirection)) return battleResults;

            var symbol = attacker.Symbols[attackerSymbolLocalDirection];
            var effect = symbol.ActiveEffect;
            var attackerBattlefieldVision = new BattlefieldVision(attacker, attackerSymbolLocalDirection, _units, _mapModel);
            if (!effect.IsActivated(attackerBattlefieldVision, attacker.Position)) return battleResults;

            var defender = effect.RetriveTarget(attackerBattlefieldVision, attacker.Position);
            var defendingSymbolLocalDirection = defender.Orientation.CalculateLocalDirection(orientation.Opposite());
            effect.Execute(attackerBattlefieldVision, attacker.Position, battleResults);

            if (!effect.IsDefendableEffect) return battleResults;
            if (!defender.Symbols.ContainsKey(defendingSymbolLocalDirection)) return battleResults;

            var defenderBattlefieldVision = new BattlefieldVision(defender, defendingSymbolLocalDirection, _units, _mapModel);
            var defenderEffect = defender.Symbols[defendingSymbolLocalDirection].ReactEffect;
            if (defenderEffect.IsActivated(defenderBattlefieldVision, attacker.Position))
            {
                defenderEffect.Execute(defenderBattlefieldVision, attacker.Position, battleResults);
            }
            return battleResults;
        }

        public BattleResults PerformProjectileHitAtPlace(MyHexPosition projectileHitPosition)
        {
            Assert.IsTrue(_units.IsPawnAt(projectileHitPosition), "There is no unit at "+projectileHitPosition);
            Assert.IsTrue(_projectiles.IsPawnAt(projectileHitPosition), "There is no projectile at "+projectileHitPosition);
            var unit = _units.GetPawnAt(projectileHitPosition);
            var projectile = _projectiles.GetPawnAt(projectileHitPosition);
            var projectileOrientation = projectile.Orientation;

            var battleResults = new BattleResults();
            battleResults.AddStruckUnit(unit);
            var defenderSymbolOrientation = unit.Orientation.CalculateLocalDirection(projectileOrientation.Opposite());
            if (unit.Symbols.ContainsKey(defenderSymbolOrientation))
            {
                BattlefieldVision vision = new BattlefieldVision(unit, projectileOrientation, _units, _mapModel);
                unit.Symbols[defenderSymbolOrientation].ReactEffect.Execute(vision, projectileHitPosition, battleResults);
            }
            return battleResults;
        }
    }
}