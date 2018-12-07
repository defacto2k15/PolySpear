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
    public class SpearSymbolModel : MonoBehaviour, ISymbolModel
    {
        private UnitModel _owningUnit;
        private IEffect _passiveEffect;
        private IEffect _activeEffect;

        public void Start()
        {
            _owningUnit = GetComponentInParent<UnitModel>();
            _passiveEffect = new SpearSymbolPassiveEffect();
            _activeEffect = new SpearSymbolActiveEffect();
        }

        public Orientation LocalOrientation
        {
            get { return _owningUnit.Symbols.Where(c => c.Value == this).Select(c => c.Key).First(); }
        }

        public IEffect PassiveEffect => _passiveEffect;
        public IEffect ActiveEffect => _activeEffect;
    }
}
