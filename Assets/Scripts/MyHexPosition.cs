using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class MyHexPosition 
    {
        [SerializeField]
        public int U;
        [SerializeField]
        public int V;
        public const int SIZE = 1;
        private const float SQRT32 = 0.86602540378443864676372317075294f;

        public MyHexPosition()
        {
            U = 0;
            V = 0;
        }

        public MyHexPosition(int u, int v)
        {
            U = u;
            V = v;
        }

        public Vector3 GetPosition ()
        {
            float x = SIZE * V * 1.5f;
            float y = SIZE * (2 * U - V) * SQRT32;
            return new Vector3 (x, 0, y);
        }

        public HexPosition N {	get { return new HexPosition (U + 1,	V); } }

        public HexPosition NE {	get { return new HexPosition (U + 1,	V + 1); } }

        public HexPosition SE {	get { return new HexPosition (U, V + 1); } }

        public HexPosition S {	get { return new HexPosition (U - 1,	V); } }

        public HexPosition SW {	get { return new HexPosition (U - 1,	V - 1); } }

        public HexPosition NW {	get { return new HexPosition (U, V - 1); } }

        public HexPosition[] Neighbors { get { return new HexPosition[6] {
            this.N,
            this.NE,
            this.SE,
            this.S,
            this.SW,
            this.NW
        }; } }

        //Gives a hex n in a given direction. You can get to any hex in two steps.
        //This will be more understandable than trying to give coordinates.
        public HexPosition goN (int n)
        {
            return new HexPosition (U + n, V);
        }

        public HexPosition goNE (int ne)
        {
            return new HexPosition (U + ne,	V + ne);
        }

        public HexPosition goSE (int se)
        {
            return new HexPosition (U, V + se);
        }

        public HexPosition goS (int s)
        {
            return new HexPosition (U - s, V);
        }

        public HexPosition goSW (int sw)
        {
            return new HexPosition (U - sw,	V - sw);
        }

        public HexPosition goNW (int nw)
        {
            return new HexPosition (U, V - nw);
        }

        public override string ToString()
        {
            return $"{nameof(U)}: {U}, {nameof(V)}: {V}";
        }

        protected bool Equals(MyHexPosition other)
        {
            return U == other.U && V == other.V;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyHexPosition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (U * 397) ^ V;
            }
        }

        public static MyHexPosition FromWorldPosition(Vector3 position)
        {
            float yy = 1 / SQRT32 * position.z / SIZE + 1;
            float xx = position.x / SIZE + yy / 2 + 0.5f;
            var u = Mathf.FloorToInt ((Mathf.Floor (xx) + Mathf.Floor (yy)) / 3);
            var v = Mathf.FloorToInt ((xx - yy + u + 1) / 2);
            return new MyHexPosition(u,v);
        }
    }
}