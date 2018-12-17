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
            if (Units.IsUnitAt(position))
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
            return MapModel.HasTileAt(position) && !Units.IsUnitAt(position);
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
            return Units.IsUnitAt(position);
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
                    if (Units.IsUnitAt(pair.NeighbourPosition))
                    {
                        // Passive effects
                        var neighbourUnit = Units.GetUnitAt(pair.NeighbourPosition).GetComponent<UnitModel>();

                        if (neighbourUnit.Owner != attackingUnit.Owner)
                        {
                            var passiveSymbolLocalDirection = neighbourUnit.Orientation.CalculateLocalDirection(pair.NeighbourDirection.Opposite);
                            var attackingSymbolLocalDirection = attackingUnit.Orientation.CalculateLocalDirection(pair.NeighbourDirection);

                            if (neighbourUnit.Symbols.ContainsKey(passiveSymbolLocalDirection))
                            {
                                var symbol = neighbourUnit.Symbols[passiveSymbolLocalDirection];
                                var effectReciever = new EffectReciever();
                                symbol.PassiveEffect.Execute(effectReciever);
                                if (attackingUnit.Symbols.ContainsKey(attackingSymbolLocalDirection))
                                {
                                    attackingUnit.Symbols[attackingSymbolLocalDirection].ReactEffect.Execute(effectReciever);
                                }

                                if (!effectReciever.IsAlive)
                                {
                                    battleResults.UnitsKilled.Add(attackingUnit);
                                    return battleResults;
                                }
                            }

                            // active effects
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
                                    if (!MapModel.HasTileAt(newNeighbourPosition) || Units.IsUnitAt(newNeighbourPosition))
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

        public bool IsFinished()
        {
            return PlayerLost(MyPlayer.Player1) || PlayerLost(MyPlayer.Player2);
        }

        public MyPlayer GetWinner()
        {
            Assert.IsTrue(IsFinished());
            if (PlayerLost(MyPlayer.Player1))
            {
                return MyPlayer.Player2;
            }
            else
            {
                return MyPlayer.Player1;
            }
        }

        private bool PlayerLost(MyPlayer player)
        {
            return !Units.HasAnyUnits(player) || UnitsOfPlayerCannotMove(player);
        }

        private bool UnitsOfPlayerCannotMove(MyPlayer player)
        {
            return Units.GetUnitsOfPlayer(player).All(c =>
            {
                return c.PossibleMoveTargets.All(k => !MapModel.HasTileAt(k) || Units.IsUnitAt(k));
            });
        }

        public bool CanMoveTo(UnitModel unitMoved, MyHexPosition target)
        {
            if (IsTileMovable(target)) //empty!
            {
                return true;
            }
            else
            {
                var neighbourDirection = unitMoved.Position.NeighboursWithDirections.Where(c => c.NeighbourPosition.Equals(target)).Select(c => c.NeighbourDirection).First();
                var oldUnitOrientation = unitMoved.Orientation;
                unitMoved.Orientation = neighbourDirection; //todo
                if (MapModel.HasTileAt(target)) // is valid position
                {
                    if (Units.IsUnitAt(target))
                    {
                        // Passive effects
                        var neighbourUnit = Units.GetUnitAt(target).GetComponent<UnitModel>();

                        if (neighbourUnit.Owner != unitMoved.Owner)
                        {
                            var passiveSymbolLocalDirection = neighbourUnit.Orientation.CalculateLocalDirection(neighbourDirection.Opposite);
                            var attackingSymbolLocalDirection = unitMoved.Orientation.CalculateLocalDirection(neighbourDirection);

                            if (neighbourUnit.Symbols.ContainsKey(passiveSymbolLocalDirection))
                            {
                                var symbol = neighbourUnit.Symbols[passiveSymbolLocalDirection];
                                var effectReciever = new EffectReciever();
                                symbol.PassiveEffect.Execute(effectReciever);
                                if (unitMoved.Symbols.ContainsKey(attackingSymbolLocalDirection))
                                {
                                    unitMoved.Symbols[attackingSymbolLocalDirection].ReactEffect.Execute(effectReciever);
                                }

                                if (!effectReciever.IsAlive)
                                {
                                    unitMoved.Orientation = oldUnitOrientation;
                                    return false;
                                }
                            }

                            // active effects
                            if (unitMoved.Symbols.ContainsKey(attackingSymbolLocalDirection))
                            {
                                var symbol = unitMoved.Symbols[attackingSymbolLocalDirection];
                                var effectReciever = new EffectReciever();
                                symbol.ActiveEffect.Execute(effectReciever);

                                if (neighbourUnit.Symbols.ContainsKey(passiveSymbolLocalDirection))
                                {
                                    neighbourUnit.Symbols[passiveSymbolLocalDirection].ReactEffect.Execute(effectReciever);
                                }

                                if (!effectReciever.IsAlive)
                                {
                                    return true;
                                    //battleResults.UnitsKilled.Add(neighbourUnit);
                                }

                                if (effectReciever.WasPushed)
                                {
                                    var newNeighbourPosition = neighbourUnit.Position.GoInDirection(neighbourDirection);
                                    if (!MapModel.HasTileAt(newNeighbourPosition) || Units.IsUnitAt(newNeighbourPosition))
                                    {
                                        return true;
                                        //battleResults.UnitsKilled.Add(neighbourUnit);
                                    }
                                    else
                                    {
                                        return true;
                                        //battleResults.UnitsPushed.Add(new PushResult()
                                        //{
                                        //    UnitPushed = neighbourUnit,
                                        //    StartPosition = neighbourUnit.Position,
                                        //    EndPosition = newNeighbourPosition
                                        //});
                                    }
                                }
                            }
                        }
                    }
                }
                unitMoved.Orientation = oldUnitOrientation;
            }
            return false;
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
