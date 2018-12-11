using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Game;
using Assets.Scripts.Symbols;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitModel : MonoBehaviour
    {
        public MyPlayer Owner;
        public MyHexPosition Position;
        public Orientation Orientation;
        public Dictionary<Orientation, SymbolModel> Symbols => GetComponentsInChildren<SymbolModel>().ToDictionary(c => c.Orientation, c => c);

        public List<MyHexPosition> PossibleMoveTargets => Position.Neighbors.ToList();
    }
}
