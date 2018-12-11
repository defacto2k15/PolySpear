using System;
using System.Collections.Generic;
using System.Linq;
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

        public MyHexPosition N {	get { return new MyHexPosition (U + 1,	V); } }

        public MyHexPosition NE {	get { return new MyHexPosition (U + 1,	V + 1); } }

        public MyHexPosition SE {	get { return new MyHexPosition (U, V + 1); } }

        public MyHexPosition S {	get { return new MyHexPosition (U - 1,	V); } }

        public MyHexPosition SW {	get { return new MyHexPosition (U - 1,	V - 1); } }

        public MyHexPosition NW {	get { return new MyHexPosition (U, V - 1); } }

        public List<NeighbourWithDirection> NeighboursWithDirections => new List<NeighbourWithDirection>()
        {
            new NeighbourWithDirection() {NeighbourDirection = Orientation.N, NeighbourPosition = N},
            new NeighbourWithDirection() {NeighbourDirection = Orientation.NE, NeighbourPosition = NE},
            new NeighbourWithDirection() {NeighbourDirection = Orientation.SE, NeighbourPosition = SE},
            new NeighbourWithDirection() {NeighbourDirection = Orientation.S, NeighbourPosition = S},
            new NeighbourWithDirection() {NeighbourDirection = Orientation.SW, NeighbourPosition = SW},
            new NeighbourWithDirection() {NeighbourDirection = Orientation.NW, NeighbourPosition = NW},
        };

        public MyHexPosition[] Neighbors => NeighboursWithDirections.Select(c => c.NeighbourPosition).ToArray();

        public Orientation[] NeighborDirections => NeighboursWithDirections.Select(c => c.NeighbourDirection).ToArray();

        //Gives a hex n in a given direction. You can get to any hex in two steps.
        //This will be more understandable than trying to give coordinates.
        public MyHexPosition goN (int n)
        {
            return new MyHexPosition (U + n, V);
        }

        public MyHexPosition goNE (int ne)
        {
            return new MyHexPosition (U + ne,	V + ne);
        }

        public MyHexPosition goSE (int se)
        {
            return new MyHexPosition (U, V + se);
        }

        public MyHexPosition goS (int s)
        {
            return new MyHexPosition (U - s, V);
        }

        public MyHexPosition goSW (int sw)
        {
            return new MyHexPosition (U - sw,	V - sw);
        }

        public MyHexPosition goNW (int nw)
        {
            return new MyHexPosition (U, V - nw);
        }

        public MyHexPosition GoInDirection(Orientation orientation)
        {
            return new MyHexPosition(U + orientation.NeighboutOffset.U, V + orientation.NeighboutOffset.V);
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

    public class NeighbourWithDirection
    {
        public Orientation NeighbourDirection;
        public MyHexPosition NeighbourPosition;
    }
}