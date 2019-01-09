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

        [SerializeField]
        public SerializableIEffect SerializablePassiveEffect;
        [SerializeField]
        public SerializableIEffect SerializableActiveEffect;
        [SerializeField]
        public SerializableIEffect SerializableReactiveEffect;
        [SerializeField]
        public Orientation Orientation;

        public void Start()
        {
            _owningUnit = GetComponentInParent<UnitModelComponent>().Model;
        }

        public Orientation LocalOrientation => Orientation;

        public IEffect PassiveEffect => SerializablePassiveEffect.Value;
        public IEffect ActiveEffect => SerializableActiveEffect.Value;
        public IEffect ReactEffect => SerializableReactiveEffect.Value;
    }
}
