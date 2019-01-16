using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Game;
using Assets.Scripts.Symbols;

namespace Assets.Scripts.Units
{
    public class UnitModel : PawnModel
    {
        public Dictionary<Orientation, SymbolModel> Symbols { get; set; }

        public event Action OnStepEvent;
        public event Action OnDeathEvent;
        public event Action OnAttackEvent;

        public void OnDeath() => OnDeathEvent?.Invoke();
        public void OnStep() => OnStepEvent?.Invoke();
        public void OnAttack() => OnAttackEvent?.Invoke();

        public UnitModel Clone()
        {
            return new UnitModel()
            {
                Owner = Owner,
                Position = Position,
                Orientation = Orientation,
                Symbols = Symbols.ToDictionary(c => c.Key, c => c.Value),
                IsUnitAlive = IsUnitAlive
            };
        }
    }
}
