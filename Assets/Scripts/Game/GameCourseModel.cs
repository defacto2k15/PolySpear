using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Map;
using Assets.Scripts.Units;
using UnityEngine;

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

        public void AddUnit(MyHexPosition position, MyPlayer player, Orientation orientation)
        {
            Units.AddUnit(position, player, orientation);
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

        public void PerformBattle(MyHexPosition battleActivatorPosition)
        {
            //todo: maybe some kind of battle manager?
            // need to uogólnić walkę z pasywnymi i z aktywnymi

            var attackingUnit = Units.GetUnitAt(battleActivatorPosition);

            //1: Passive effects
            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (MapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (Units.HasUnitAt(pair.NeighbourPosition))
                    {
                        var neighbourUnit = Units.GetUnitAt(pair.NeighbourPosition).GetComponent<UnitModel>();
                        var passiveSymbolLocalDirection = neighbourUnit.Orientation.CalculateLocalDirection(pair.NeighbourDirection.Opposite);
                        if (neighbourUnit.Symbols.ContainsKey(passiveSymbolLocalDirection))
                        {
                            var symbol = neighbourUnit.Symbols[passiveSymbolLocalDirection];
                            var effectReciever = new EffectReciever();
                            symbol.PassiveEffect.Execute(effectReciever);

                            if (effectReciever.WasKilled)
                            {
                                Units.RemoveUnit(battleActivatorPosition);
                                return;
                            }
                        }
                    }
                }
            }

            //2: Active effects!
            foreach (var pair in battleActivatorPosition.NeighboursWithDirections)
            {
                if (MapModel.HasTileAt(pair.NeighbourPosition)) // is valid position
                {
                    if (Units.HasUnitAt(pair.NeighbourPosition))
                    {
                        var neighbourUnit = Units.GetUnitAt(pair.NeighbourPosition).GetComponent<UnitModel>();
                        var attackingSymbolLocalDirection = attackingUnit.Orientation.CalculateLocalDirection(pair.NeighbourDirection);
                        if (attackingUnit.Symbols.ContainsKey(attackingSymbolLocalDirection))
                        {
                            var symbol = neighbourUnit.Symbols[attackingSymbolLocalDirection];
                            var effectReciever = new EffectReciever();
                            symbol.ActiveEffect.Execute(effectReciever);

                            if (effectReciever.WasKilled)
                            {
                                Units.RemoveUnit(pair.NeighbourPosition);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
