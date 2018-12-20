using System;
using Assets.Scripts.Game;
using Assets.Scripts.Map;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;

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
                            if (battleResults.UnitsKilled.Contains(intruderUnit))
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
                var effectReciever = new EffectReciever();
                attackingEffectExtractor(symbol).Execute(effectReciever);
                if (defender.Symbols.ContainsKey(defendingSymbolLocalDirection))
                {
                    defender.Symbols[defendingSymbolLocalDirection].ReactEffect.Execute(effectReciever);
                }

                if (!effectReciever.IsAlive)
                {
                    battleResults.UnitsKilled.Add(defender);
                    return battleResults;
                }
                if (effectReciever.WasPushed)
                {
                    var newDefenderPosition = defender.Position.GoInDirection(attackerDirectionFromDefender.Opposite());
                    if (!_mapModel.HasTileAt(newDefenderPosition) || _units.IsUnitAt(newDefenderPosition))
                    {
                        battleResults.UnitsKilled.Add(defender);
                    }
                    else
                    {
                        battleResults.UnitsPushed.Add(new PushResult()
                        {
                            UnitPushed = defender,
                            StartPosition = defender.Position,
                            EndPosition = newDefenderPosition
                        });
                    }

                }
            }
            return battleResults;
        }

       
    }
}