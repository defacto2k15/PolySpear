using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Symbols
{
    public class SymbolModel : MonoBehaviour
    {
        private UnitModel _owningUnit;

        public IEffectContainer EffectContainer;
        public Orientation Orientation;

        public void Start()
        {
            _owningUnit = GetComponentInParent<UnitModel>();
        }

        public Orientation LocalOrientation => Orientation;

        public IEffect PassiveEffect => EffectContainer.PassiveEffect;
        public IEffect ActiveEffect => EffectContainer.ActiveEffect;
        public IEffect ReactEffect => EffectContainer.ReactEffect;
    }
}
