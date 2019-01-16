using System;
using System.Collections.Generic;
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

        public BattleResults PerformBattleAtPlace(MyHexPosition battleActivatorPosition, BattleCircumstances battleCircumstances)
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
                            battleResults.Add(PerformSingleFight(intruderUnit, neighbourUnit, pair.NeighbourDirection, symbol => symbol.PassiveEffect, battleCircumstances));
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
                battleResults.Add(PerformSingleActiveFight(intruderUnit, orientation, battleCircumstances));
                if (battleResults.UnitIncapaciated(intruderUnit))
                {
                    return battleResults;
                }
            }
            return battleResults;
        }

        public BattleResults PerformPassiveBattleAtPlace(MyHexPosition battleActivatorPosition, BattleCircumstances battleCircumstances)
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
                            battleResults.Add(PerformSingleFight(intruderUnit, neighbourUnit, pair.NeighbourDirection, symbol => symbol.PassiveEffect,
                                battleCircumstances));
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
                            battleResults.Add(PerformSingleFight(neighbourUnit, intruderUnit, pair.NeighbourDirection.Opposite(), symbol => symbol.PassiveEffect,
                                battleCircumstances));
                            
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

        private BattleEngagement PerformSingleFight(UnitModel defender, UnitModel attacker, Orientation attackerDirectionFromDefender,
            Func<SymbolModel, IEffect> attackingEffectExtractor, BattleCircumstances battleCircumstances)
        {
            var engagementResult = new BattleEngagementResult();
            var engagement = new BattleEngagement(engagementResult);

            if (defender.Owner == attacker.Owner)
            {
                return engagement;
            }        

            var attackerSymbolLocalDirection = attacker.Orientation.CalculateLocalDirection(attackerDirectionFromDefender.Opposite());
            var defendingSymbolLocalDirection = defender.Orientation.CalculateLocalDirection(attackerDirectionFromDefender);

            if (attacker.Symbols.ContainsKey(attackerSymbolLocalDirection))
            {
                var symbol = attacker.Symbols[attackerSymbolLocalDirection];
                var effect = attackingEffectExtractor(symbol);

                var attackerBattlefieldVision = new BattlefieldVision(attacker, attackerSymbolLocalDirection, _units, _mapModel, battleCircumstances);
                if (effect.IsActivated(attackerBattlefieldVision, attacker.Position))
                {
                    AddEngagementElement(attacker, defender, engagement, effect);
                    effect.Execute(attackerBattlefieldVision, attacker.Position, engagementResult);
                }
                if (!effect.IsDefendableEffect) return engagement;
                if (defender.Symbols.ContainsKey(defendingSymbolLocalDirection))
                {
                    var defenderBattlefieldVision = new BattlefieldVision(defender, defendingSymbolLocalDirection, _units, _mapModel, battleCircumstances);
                    var defenderEffect = defender.Symbols[defendingSymbolLocalDirection].ReactEffect;
                    if (defenderEffect.IsActivated(defenderBattlefieldVision, attacker.Position))
                    {
                        AddEngagementElement(defender, attacker, engagement, defenderEffect);
                        defenderEffect.Execute(defenderBattlefieldVision, attacker.Position, engagementResult);
                    }
                }
            }
            return engagement;
        }

        private BattleEngagement PerformSingleActiveFight(UnitModel attacker, Orientation orientation, BattleCircumstances battleCircumstances)
        {
            var engagementResult = new BattleEngagementResult();
            var engagement = new BattleEngagement(engagementResult);

            var attackerSymbolLocalDirection = attacker.Orientation.CalculateLocalDirection(orientation);
            if (!attacker.Symbols.ContainsKey(attackerSymbolLocalDirection)) return engagement;

            var symbol = attacker.Symbols[attackerSymbolLocalDirection];
            var effect = symbol.ActiveEffect;
            var attackerBattlefieldVision = new BattlefieldVision(attacker, attackerSymbolLocalDirection, _units, _mapModel, battleCircumstances);
            if (!effect.IsActivated(attackerBattlefieldVision, attacker.Position)) return engagement;

            var defender = effect.RetriveTarget(attackerBattlefieldVision, attacker.Position);
            var defendingSymbolLocalDirection = defender.Orientation.CalculateLocalDirection(orientation.Opposite());

            AddEngagementElement(attacker, defender, engagement, effect);
            effect.Execute(attackerBattlefieldVision, attacker.Position, engagementResult);

            if (!effect.IsDefendableEffect) return engagement;
            if (!defender.Symbols.ContainsKey(defendingSymbolLocalDirection)) return engagement;

            var defenderBattlefieldVision = new BattlefieldVision(defender, defendingSymbolLocalDirection, _units, _mapModel, battleCircumstances);
            var defenderEffect = defender.Symbols[defendingSymbolLocalDirection].ReactEffect;
            if (defenderEffect.IsActivated(defenderBattlefieldVision, attacker.Position))
            {
                AddEngagementElement(defender, attacker, engagement,defenderEffect);
                defenderEffect.Execute(defenderBattlefieldVision, attacker.Position,engagementResult);
            }
            return engagement;
        }

        private static void AddEngagementElement(PawnModel active, PawnModel passive, BattleEngagement engagement, IEffect defenderEffect)
        {
            engagement.AddEngagementElement(new EngagementElement()
            {
                ActivePawn = active,
                PassivePawn = passive,
                EngagementVisibleConsequence = new EngagementVisibleConsequence(defenderEffect.UsageAnimationGenerator)
            });
        }

        public BattleResults PerformProjectileHitAtPlace(MyHexPosition projectileHitPosition)
        {
            Assert.IsTrue(_units.IsPawnAt(projectileHitPosition), "There is no unit at " + projectileHitPosition);
            Assert.IsTrue(_projectiles.IsPawnAt(projectileHitPosition), "There is no projectile at " + projectileHitPosition);
            var unit = _units.GetPawnAt(projectileHitPosition);
            var projectile = _projectiles.GetPawnAt(projectileHitPosition);
            var projectileOrientation = projectile.Orientation;
            var projectileEffect = projectile.HitEffect;

            BattleEngagementResult engagementResult = new BattleEngagementResult();
            var engagement = new BattleEngagement(engagementResult);
            AddEngagementElement( projectile, unit, engagement, projectileEffect);

            projectileEffect.Execute(new BattlefieldVision(projectile, projectileOrientation, _units, _mapModel, BattleCircumstances.ProjectileHit), projectileHitPosition, engagementResult );
            var defenderSymbolOrientation = unit.Orientation.CalculateLocalDirection(projectileOrientation.Opposite());
            if (unit.Symbols.ContainsKey(defenderSymbolOrientation))
            {
                var reactEffect = unit.Symbols[defenderSymbolOrientation].ReactEffect;
                AddEngagementElement(unit, projectile, engagement, reactEffect);

                BattlefieldVision vision = new BattlefieldVision(unit, projectileOrientation, _units, _mapModel, BattleCircumstances.ProjectileHit);
                reactEffect.Execute(vision, projectileHitPosition, engagementResult);
            }
            var results = new BattleResults();
            results.Add(engagement);
            return results;
        }
    }
}