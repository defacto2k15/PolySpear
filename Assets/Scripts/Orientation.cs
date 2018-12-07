using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Orientation
    {
        public static Orientation N = new Orientation("N",0, new MyHexPosition ( 1,0),  (360/6.0f)*0);
        public static Orientation NE = new Orientation("NE",1, new MyHexPosition (1, 1),   (360/6.0f)*1);
        public static Orientation SE = new Orientation("SE",2, new MyHexPosition (0, + 1),  (360/6.0f)*2);
        public static Orientation S = new Orientation("S",3, new MyHexPosition (- 1,	0),  (360/6.0f)*3);
        public static Orientation SW = new Orientation("SW",4, new MyHexPosition ( - 1, - 1),  (360/6.0f)*4);
        public static Orientation NW = new Orientation("NW",5, new MyHexPosition (0, - 1),  (360/6.0f)*5);

        public static List<Orientation> AllOrientationsClockwise = new List<Orientation>()
        {
            N,NE,SE,S,SW,NW
        };

        private readonly string _name;
        private readonly int _indexClockwise;
        private readonly MyHexPosition _neighboutOffset;
        private readonly float _rotationInDegrees;

        private Orientation(string name,int indexClockwise, MyHexPosition neighboutOffset, float rotationInDegrees)
        {
            _name = name;
            _indexClockwise = indexClockwise;
            _neighboutOffset = neighboutOffset;
            _rotationInDegrees = rotationInDegrees;
        }

        public float FlatRotation => _rotationInDegrees;
        public Orientation Opposite => AllOrientationsClockwise[(_indexClockwise + 3) % 6];

        public static List<Orientation> GetOrientationsToTarget(Orientation startOrientation, Orientation targetOrientation)
        {
            List<Orientation> clockwiseOrientations = new List<Orientation>();
            List<Orientation> counterCloskwiseOrientations = new List<Orientation>();

            int startIndex = AllOrientationsClockwise.IndexOf(startOrientation);
            int targetIndex = AllOrientationsClockwise.IndexOf(targetOrientation);
            if (startIndex == targetIndex)
            {
                return  new List<Orientation>();
            }

            if (targetIndex > startIndex)
            {
                return Enumerable.Range(startIndex + 1, targetIndex - startIndex).Select(i => AllOrientationsClockwise[i]).ToList();
            }
            else
            {
                return Enumerable.Range(startIndex + 1, AllOrientationsClockwise.Count+ targetIndex - startIndex).Select(i => AllOrientationsClockwise[i%AllOrientationsClockwise.Count]).ToList();
            }

            //if (targetIndex > startIndex) TODO POTEM
            //{
            //    clockwiseOrientations = Enumerable.Range(startIndex+1, targetIndex - startIndex).Select(i => AllOrientationsClockwise[i]).ToList();
            //    counterCloskwiseOrientations = Enumerable.Range(targetIndex , (AllOrientationsClockwise.Count + startIndex - targetIndex) )
            //        .Select(i => AllOrientationsClockwise[i % AllOrientationsClockwise.Count]).Reverse().ToList();
            //}
            //else
            //{
            //    clockwiseOrientations = Enumerable.Range(targetIndex, targetIndex+AllOrientationsClockwise.Count - startIndex).Select(i => AllOrientationsClockwise[i]).ToList();
            //    counterCloskwiseOrientations = Enumerable.Range(targetIndex, (AllOrientationsClockwise.Count + targetIndex - startIndex))
            //        .Select(i => AllOrientationsClockwise[i % AllOrientationsClockwise.Count]).Reverse().ToList();
            //}

            if (clockwiseOrientations.Count <= counterCloskwiseOrientations.Count)
            {
                return clockwiseOrientations;
            }
            else
            {
                return counterCloskwiseOrientations;
            }
        }

        public override string ToString()
        {
            return $"{nameof(_name)}: {_name}";
        }

        public Orientation CalculateLocalDirection(Orientation globalDirection)
        {
            return AllOrientationsClockwise[(AllOrientationsClockwise.Count + globalDirection._indexClockwise - this._indexClockwise) % AllOrientationsClockwise.Count];
        }
    }
}
