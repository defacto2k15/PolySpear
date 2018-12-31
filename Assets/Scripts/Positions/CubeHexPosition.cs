using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Assets.Scripts.Positions
{
    public class CubeHexPosition
    {
        private int _x;
        private int _y;
        private int _z;

        public CubeHexPosition(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
            Assert.IsTrue(_x+_y+_z==0,$"Sum of x:{_x}, y:{_y}, z:{_z} is not 0");
        }

        public int X => _x;

        public int Y => _y;

        public int Z => _z;

        protected bool Equals(CubeHexPosition other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CubeHexPosition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _x;
                hashCode = (hashCode * 397) ^ _y;
                hashCode = (hashCode * 397) ^ _z;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(_x)}: {_x}, {nameof(_y)}: {_y}, {nameof(_z)}: {_z}";
        }
    }
}
