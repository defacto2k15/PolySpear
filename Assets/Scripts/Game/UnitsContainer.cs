using System.Collections.Generic;
using Assets.Scripts.Symbols;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public class UnitsContainer : MonoBehaviour
    {
        private Dictionary<MyHexPosition, GameObject> _units = new Dictionary<MyHexPosition, GameObject>();
        //public GameObject UnitPrefab; //todo delete
        //public GameObject SpearSymbolPrefab;

        public void AddUnit(MyHexPosition position, MyPlayer player, Orientation orientation, GameObject unitPrefab)
        {
            var unit = Instantiate(unitPrefab, transform);
            unit.GetComponent<UnitModel>().Orientation = orientation;
            unit.GetComponent<UnitModel>().Position = position;
            unit.GetComponent<UnitModel>().Owner = player;

            _units[position] = unit;
        }

        public bool HasUnitAt(MyHexPosition position)
        {
            return _units.ContainsKey(position);
        }

        public UnitModel GetUnitAt(MyHexPosition position)
        {
            return _units[position].GetComponent<UnitModel>();
        }

        public void MoveUnit(MyHexPosition oldPosition, MyHexPosition newPosition)
        {
            Assert.IsTrue(HasUnitAt(oldPosition));
            var unit = _units[oldPosition];
            _units.Remove(oldPosition);
            _units[newPosition] = unit;
            unit.GetComponent<UnitModel>().Position = newPosition;
        }

        public void OrientUnit(MyHexPosition unitPosition, Orientation orientation)
        {
            Assert.IsTrue(HasUnitAt(unitPosition));
            var unit = _units[unitPosition];
            unit.GetComponent<UnitModel>().Orientation = orientation;
        }

        public void RemoveUnit(MyHexPosition position)
        {
            Assert.IsTrue(_units.ContainsKey(position));
            GameObject.Destroy(_units[position]);
            _units.Remove(position);
        }
    }
}