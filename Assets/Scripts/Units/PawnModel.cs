using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public abstract class PawnModel : MonoBehaviour
    {
        public MyHexPosition Position { get; set; }
        public Orientation Orientation { get; set; } 
        public bool IsUnitAlive = true;

        public List<MyHexPosition> PossibleMoveTargets => Position.Neighbors.ToList();

        public event Action OnUnitKilled;

        public void SetUnitKilled()
        {
            IsUnitAlive = false;
            OnUnitKilled?.Invoke();
        }
    }
}