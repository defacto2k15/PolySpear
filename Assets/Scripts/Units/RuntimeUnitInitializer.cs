using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Symbols;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class RuntimeUnitInitializer : MonoBehaviour
    {
        public void Start()
        {
            var model = GetComponent<UnitModel>();
            model.Symbols =  GetComponentsInChildren<SymbolModel>().ToDictionary(c => c.Orientation, c => c);
        }
    }
}
