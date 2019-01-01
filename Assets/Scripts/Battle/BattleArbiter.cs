﻿using System;
using Assets.Scripts.Game;
using Assets.Scripts.Map;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class BattleArbiter
    {
        private UnitsContainer _units;
        private MapModel _mapModel;

        public BattleArbiter(UnitsContainer units, MapModel mapModel)
        {
            _units = units;
            _mapModel = mapModel;
        }

        public BattleResults PerformBattleAtPlace(MyHexPosition battleActivatorPosition)
        {
            var battleResults = new BattleResults();

            var intruderUnit = _units.GetUnitAt(battleActivatorPosition);

            // First - Passive effects on all neighburs
            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (_mapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (_units.IsUnitAt(pair.NeighbourPosition))
                    {
                        var neighbourUnit = _units.GetUnitAt(pair.NeighbourPosition);
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
            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (_mapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (_units.IsUnitAt(pair.NeighbourPosition))
                    {
                        var neighbourUnit = _units.GetUnitAt(pair.NeighbourPosition);
                        if (!battleResults.UnitIncapaciated(neighbourUnit))
                        {
                            battleResults.Add(PerformSingleFight(neighbourUnit, intruderUnit, pair.NeighbourDirection.Opposite(), symbol => symbol.ActiveEffect));
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

       
    }
}