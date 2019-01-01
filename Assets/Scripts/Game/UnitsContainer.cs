using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public class UnitsContainer : MonoBehaviour
    {
        private Dictionary<MyHexPosition, UnitModel> _units = new Dictionary<MyHexPosition, UnitModel>();

        public void Reset()
        {
            foreach (var position in _units.Keys.ToList())
            {
                RemoveUnit(position);
            }
            _units = new Dictionary<MyHexPosition, UnitModel>();
        }

        public UnitModel AddUnit(MyHexPosition position, MyPlayer player, Orientation orientation, GameObject unitPrefab)
        {
            var unit = Instantiate(unitPrefab, transform);
            unit.GetComponent<UnitModel>().Orientation = orientation;
            unit.GetComponent<UnitModel>().Position = position;
            unit.GetComponent<UnitModel>().Owner = player;

            _units[position] = unit.GetComponent<UnitModel>();
            return unit.GetComponent<UnitModel>();
        }

        public bool IsUnitAt(MyHexPosition position)
        {
            return _units.ContainsKey(position);
        }

        public UnitModel GetUnitAt(MyHexPosition position)
        {
            return _units[position];
        }

        public void MoveUnit(MyHexPosition oldPosition, MyHexPosition newPosition)
        {
            Assert.IsTrue(IsUnitAt(oldPosition));
            var unit = _units[oldPosition];
            _units.Remove(oldPosition);
            _units[newPosition] = unit;
            unit.Position = newPosition;
        }

        public void OrientUnit(MyHexPosition unitPosition, Orientation orientation)
        {
            Assert.IsTrue(IsUnitAt(unitPosition));
            var unit = _units[unitPosition];
            unit.Orientation = orientation;
        }

        public void RemoveUnit(MyHexPosition position)
        {
            Assert.IsTrue(_units.ContainsKey(position));
            _units.Remove(position);
        }

        public bool HasAnyUnits(MyPlayer player)
        {
            return _units.Values.Any(c => c.Owner == player);
        }

        public List<UnitModel> GetUnitsOfPlayer(MyPlayer player)
        {
            return _units.Values.Where(c => c.Owner == player).ToList();
        }

        public UnitsContainer Clone()
        {
            return new UnitsContainer()
            {
                _units = _units.ToDictionary(c => c.Key, c => c.Value.Clone())
            };
        }

    }
}