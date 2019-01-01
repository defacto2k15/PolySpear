using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Battle;
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

        public void Reset()
        {
            Start();
            MapModel.Reset();
            Units.Reset();
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

        public UnitModel AddUnit(MyHexPosition position, MyPlayer player, Orientation orientation, GameObject unitPrefab) // todo redesign
        {
            return Units.AddUnit(position, player, orientation, unitPrefab);
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

        public BattleResults PerformBattleAtPlace(MyHexPosition battleActivatorPosition)
        {
            var arbiter = new BattleArbiter(Units, MapModel);
            return arbiter.PerformBattleAtPlace(battleActivatorPosition);
        }

        public BattleResults PerformPassiveOnlyBattleAtPlace(MyHexPosition battleActivatorPosition)
        {
            var arbiter = new BattleArbiter(Units, MapModel);
            return arbiter.PerformPassiveBattleAtPlace(battleActivatorPosition);
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
            if (!unitMoved.PossibleMoveTargets.Contains(target))
            {
                return false;
            }
            else if (IsTileMovable(target) ) //empty!
            {
                return true;
            }
            else
            {
                if (Units.IsUnitAt(target))
                {
                    var tempUnitsContainer = Units.Clone();
                    var tempMapModel = MapModel.Clone();

                    var newOrientation = unitMoved.Position.NeighboursWithDirections.Where(c => c.NeighbourPosition.Equals(target))
                        .Select(c => c.NeighbourDirection).First();

                    var tempUnitModel = tempUnitsContainer.GetUnitAt(unitMoved.Position);
                    tempUnitsContainer.OrientUnit(tempUnitModel.Position, newOrientation);

                    var arbiter = new BattleArbiter(tempUnitsContainer, tempMapModel);
                    var battleResults = arbiter.PerformBattleAtPlace(unitMoved.Position);
                    return battleResults.PositionWasFreed(target);
                }

            }
            return false;
        }

    }
}
