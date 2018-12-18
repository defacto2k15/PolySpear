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
        public MyPlayer Owner { get; set; }
        public MyHexPosition Position { get; set; }
        public Orientation Orientation { get; set; } 
        public Dictionary<Orientation, SymbolModel> Symbols { get; set; }

        public List<MyHexPosition> PossibleMoveTargets => Position.Neighbors.ToList();

        public event Action OnUnitKilled;

        public void SetUnitKilled()
        {
            OnUnitKilled?.Invoke();
        }

        public UnitModel Clone()
        {
            return new UnitModel()
            {
                Owner = Owner,
                Position = Position,
                Orientation = Orientation,
                Symbols = Symbols.ToDictionary(c => c.Key, c => c.Value)
            };
        }
    }

}
