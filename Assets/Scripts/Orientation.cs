using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public enum Orientation
    {
        N,
        NE,
        SE,
        S,
        SW,
        NW

    }

    public static class OrientationUtils
    {
        public static List<Orientation> AllOrientationsClockwise = new List<Orientation>()
        {
            Orientation.N,
            Orientation.NE,
            Orientation.SE,
            Orientation.S,
            Orientation.SW,
            Orientation.NW,
        };

        private static OrientationInnerData GetInnerData(Orientation orientation)
        {
            return OrientationInnerData.AllOrientationInnerDatasClockwise[AllOrientationsClockwise.IndexOf(orientation)];
        }

        private static Orientation GetOrientation(OrientationInnerData orientationInnerData)
        {
            return OrientationUtils.AllOrientationsClockwise[OrientationInnerData.AllOrientationInnerDatasClockwise.IndexOf(orientationInnerData)];
        }

        public static float FlatRotation(this Orientation orientation)
        {
            return GetInnerData(orientation).FlatRotation;
        }


        public static Orientation Opposite(this Orientation orientation)
        {
            return GetOrientation(GetInnerData(orientation).Opposite);
        }

        public static MyHexPosition NeighbourOffset(this Orientation orientation)
        {
            return GetInnerData(orientation).NeighbourOffset;
        }


        public static List<Orientation> GetOrientationsToTarget(Orientation startOrientation,
            Orientation targetOrientation)
        {
            return OrientationInnerData.GetOrientationInnerDatasToTarget(GetInnerData(startOrientation), GetInnerData(targetOrientation))
                .Select(GetOrientation).ToList();
        }

        public static Orientation CalculateLocalDirection(this Orientation orientation, Orientation globalDirection)
        {
            return GetOrientation(GetInnerData(orientation).CalculateLocalDirection(GetInnerData(globalDirection)));
        }

        private class OrientationInnerData
        {

            public static OrientationInnerData N = new OrientationInnerData("N", 0, new MyHexPosition(1, 0), (360 / 6.0f) * 0);
            public static OrientationInnerData NE = new OrientationInnerData("NE", 1, new MyHexPosition(1, 1), (360 / 6.0f) * 1);
            public static OrientationInnerData SE = new OrientationInnerData("SE", 2, new MyHexPosition(0, +1), (360 / 6.0f) * 2);
            public static OrientationInnerData S = new OrientationInnerData("S", 3, new MyHexPosition(-1, 0), (360 / 6.0f) * 3);
            public static OrientationInnerData SW = new OrientationInnerData("SW", 4, new MyHexPosition(-1, -1), (360 / 6.0f) * 4);
            public static OrientationInnerData NW = new OrientationInnerData("NW", 5, new MyHexPosition(0, -1), (360 / 6.0f) * 5);

            public static List<OrientationInnerData> AllOrientationInnerDatasClockwise = new List<OrientationInnerData>()
            {
                OrientationInnerData.N,
                OrientationInnerData.NE,
                OrientationInnerData.SE,
                OrientationInnerData.S,
                OrientationInnerData.SW,
                OrientationInnerData.NW,
            };

            private readonly string _name;
            private readonly int _indexClockwise;
            private readonly MyHexPosition _neighboutOffset;
            private readonly float _rotationInDegrees;

            private OrientationInnerData(string name, int indexClockwise, MyHexPosition neighboutOffset, float rotationInDegrees)
            {
                _name = name;
                _indexClockwise = indexClockwise;
                _neighboutOffset = neighboutOffset;
                _rotationInDegrees = rotationInDegrees;
            }

            public float FlatRotation => _rotationInDegrees;
            public OrientationInnerData Opposite => AllOrientationInnerDatasClockwise[(_indexClockwise + 3) % 6];

            public MyHexPosition NeighbourOffset => _neighboutOffset;

            public static List<OrientationInnerData> GetOrientationInnerDatasToTarget(OrientationInnerData startOrientationInnerData,
                OrientationInnerData targetOrientationInnerData)
            {
                List<OrientationInnerData> clockwiseOrientationInnerDatas = new List<OrientationInnerData>();
                List<OrientationInnerData> counterCloskwiseOrientationInnerDatas = new List<OrientationInnerData>();

                int startIndex = AllOrientationInnerDatasClockwise.IndexOf(startOrientationInnerData);
                int targetIndex = AllOrientationInnerDatasClockwise.IndexOf(targetOrientationInnerData);
                if (startIndex == targetIndex)
                {
                    return new List<OrientationInnerData>();
                }

                if (targetIndex > startIndex)
                {
                    clockwiseOrientationInnerDatas = Enumerable.Range(startIndex + 1, targetIndex - startIndex)
                        .Select(i => AllOrientationInnerDatasClockwise[i]).ToList();
                }
                else
                {
                    clockwiseOrientationInnerDatas = Enumerable.Range(startIndex + 1, AllOrientationInnerDatasClockwise.Count + targetIndex - startIndex)
                        .Select(i => AllOrientationInnerDatasClockwise[i % AllOrientationInnerDatasClockwise.Count]).ToList();
                }

                if (startIndex > targetIndex)
                {
                    counterCloskwiseOrientationInnerDatas = Enumerable.Range(targetIndex, startIndex - targetIndex).Reverse()
                        .Select(i => AllOrientationInnerDatasClockwise[i])
                        .ToList();
                }
                else
                {
                    counterCloskwiseOrientationInnerDatas = Enumerable.Range(targetIndex, AllOrientationInnerDatasClockwise.Count + startIndex - targetIndex)
                        .Reverse()
                        .Select(i => AllOrientationInnerDatasClockwise[i % AllOrientationInnerDatasClockwise.Count]).ToList();
                }

                if (clockwiseOrientationInnerDatas.Count <= counterCloskwiseOrientationInnerDatas.Count)
                {
                    return clockwiseOrientationInnerDatas;
                }
                else
                {
                    return counterCloskwiseOrientationInnerDatas;
                }
            }

            public string Name => _name;

            public override string ToString()
            {
                return $"{nameof(_name)}: {_name}";
            }

            public OrientationInnerData CalculateLocalDirection(OrientationInnerData globalDirection)
            {
                return AllOrientationInnerDatasClockwise[
                    (AllOrientationInnerDatasClockwise.Count + globalDirection._indexClockwise - this._indexClockwise) %
                    AllOrientationInnerDatasClockwise.Count];
            }
        }
    }
}

