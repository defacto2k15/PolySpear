using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Map;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public class GameCourseModel : MonoBehaviour //model przebiegu gry
    {
        private Phrase _phrase;
        private GameTurn _turn;

        public MapModel MapModel;
        public UnitsContainer Units;


        public void Start()
        {
            _phrase = Phrase.Placing;
            _turn = GameTurn.FirstPlayerTurn;
        }

        public Phrase Phrase
        {
            get { return _phrase; }
            set { _phrase = value; }
        }

        public GameTurn Turn
        {
            get { return _turn; }
            set { _turn = value; }
        }

        public void AddUnit(MyHexPosition position, MyPlayer player, Orientation orientation, GameObject unitPrefab) // todo redesign
        {
            Units.AddUnit(position, player, orientation, unitPrefab);
        }

        public bool HasTileAt(MyHexPosition hexPosition)
        {
            return MapModel.HasTileAt(hexPosition);
        }

        public UnitModel GetUnitAt(MyHexPosition position)
        {
            if (Units.HasUnitAt(position))
            {
                return Units.GetUnitAt(position);
            }
            else
            {
                return null;
            }
        }

        public bool IsTileMovable(MyHexPosition position)
        {
            return MapModel.HasTileAt(position) && !Units.HasUnitAt(position);
        }

        public void NextTurn()
        {
            _turn = _turn.NextPlayerTurn;
        }

        public void NextPhrase()
        {
            if (_phrase == Phrase.Placing)
            {
                _phrase = Phrase.Play;
            }
        }

        public void OrientUnit(UnitModel unit, Orientation orientation)
        {
            Units.OrientUnit(unit.Position, orientation);
        }

        public void MoveUnit(UnitModel unit, MyHexPosition newPosition)
        {
            Units.MoveUnit(unit.Position, newPosition);
        }

        public bool HasUnitAt(MyHexPosition position)
        {
            return Units.HasUnitAt(position);
        }

        public BattleResults PerformBattle(MyHexPosition battleActivatorPosition)
        {
            var battleResults = new BattleResults();

            //todo: maybe some kind of battle manager?
            // need to uogólnić walkę z pasywnymi i z aktywnymi
            // todo: way, way too complicated

            var attackingUnit = Units.GetUnitAt(battleActivatorPosition);

            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (MapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (Units.HasUnitAt(pair.NeighbourPosition))
                    {
                        // Passive effects
                        var neighbourUnit = Units.GetUnitAt(pair.NeighbourPosition).GetComponent<UnitModel>();

                        if (neighbourUnit.Owner != attackingUnit.Owner)
                        {
                            var passiveSymbolLocalDirection = neighbourUnit.Orientation.CalculateLocalDirection(pair.NeighbourDirection.Opposite);
                            if (neighbourUnit.Symbols.ContainsKey(passiveSymbolLocalDirection))
                            {
                                var symbol = neighbourUnit.Symbols[passiveSymbolLocalDirection];
                                var effectReciever = new EffectReciever();
                                symbol.PassiveEffect.Execute(effectReciever);
                                if (attackingUnit.Symbols.ContainsKey(pair.NeighbourDirection))
                                {
                                    attackingUnit.Symbols[pair.NeighbourDirection].ReactEffect.Execute(effectReciever);
                                }

                                if (!effectReciever.IsAlive)
                                {
                                    battleResults.UnitsKilled.Add(attackingUnit);
                                    return battleResults;
                                }
                            }

                            // active effects
                            var attackingSymbolLocalDirection = attackingUnit.Orientation.CalculateLocalDirection(pair.NeighbourDirection);
                            if (attackingUnit.Symbols.ContainsKey(attackingSymbolLocalDirection))
                            {
                                var symbol = attackingUnit.Symbols[attackingSymbolLocalDirection];
                                var effectReciever = new EffectReciever();
                                symbol.ActiveEffect.Execute(effectReciever);

                                if (neighbourUnit.Symbols.ContainsKey(passiveSymbolLocalDirection))
                                {
                                    neighbourUnit.Symbols[passiveSymbolLocalDirection].ReactEffect.Execute(effectReciever);
                                }

                                if (!effectReciever.IsAlive)
                                {
                                    battleResults.UnitsKilled.Add(neighbourUnit);
                                }

                                if (effectReciever.WasPushed)
                                {
                                    var newNeighbourPosition = neighbourUnit.Position.GoInDirection(pair.NeighbourDirection);
                                    if (!MapModel.HasTileAt(newNeighbourPosition) || Units.HasUnitAt(newNeighbourPosition))
                                    {
                                        battleResults.UnitsKilled.Add(neighbourUnit);
                                    }
                                    else
                                    {
                                        battleResults.UnitsPushed.Add(new PushResult()
                                        {
                                            UnitPushed = neighbourUnit,
                                            StartPosition = neighbourUnit.Position,
                                            EndPosition = newNeighbourPosition
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return battleResults;
        }

        public void FinalizeKillUnit(UnitModel unit) // ugly code
        {
            Units.RemoveUnit(unit.Position);
        }

    }

        public class BattleResults
        {
            public List<UnitModel> UnitsKilled = new List<UnitModel>();
            public List<PushResult> UnitsPushed = new List<PushResult>();
        }

    public class PushResult
    {
        public UnitModel UnitPushed;
        public MyHexPosition StartPosition;
        public MyHexPosition EndPosition;
    }
}
