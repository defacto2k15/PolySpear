using System;
using UnityEngine;

namespace Assets.Scripts.Game
{
    [Serializable]
    public class InitialUnitStateSpecification
    {
        public MyHexPosition Position;
        public Orientation Orientation;
        public GameObject UnitPrefab;
    }
}