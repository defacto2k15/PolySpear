using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public class PawnsContainer<T> : MonoBehaviour where T : PawnModel
    {
        protected Dictionary<MyHexPosition, T> _pawns = new Dictionary<MyHexPosition, T>();

        public void Reset()
        {
            foreach (var position in _pawns.Keys.ToList())
            {
                RemovePawn(position);
            }
            _pawns = new Dictionary<MyHexPosition, T>();
        }

        protected void AddPawn(MyHexPosition position, T pawn)
        {
            _pawns[position] = pawn;
        }

        public bool IsPawnAt(MyHexPosition position)
        {
            return _pawns.ContainsKey(position);
        }

        public T GetPawnAt(MyHexPosition position)
        {
            return _pawns[position];
        }

        public void MovePawn(MyHexPosition oldPosition, MyHexPosition newPosition)
        {
            Assert.IsTrue(IsPawnAt(oldPosition),"Pawn is not at old position");
            var unit = _pawns[oldPosition];
            _pawns.Remove(oldPosition);
            _pawns[newPosition] = unit;
            unit.Position = newPosition;
        }

        public void OrientPawn(MyHexPosition unitPosition, Orientation orientation)
        {
            Assert.IsTrue(IsPawnAt(unitPosition));
            var unit = _pawns[unitPosition];
            unit.Orientation = orientation;
        }

        public void RemovePawn(MyHexPosition position)
        {
            Assert.IsTrue(_pawns.ContainsKey(position));
            _pawns.Remove(position);
        }
    }
}