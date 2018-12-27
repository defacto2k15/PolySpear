using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Symbols
{
    [ExecuteInEditMode]
    public class SymbolView : MonoBehaviour
    {
        private SymbolModel _model;

        public void Start()
        {
            _model = GetComponent<SymbolModel>();
        }

        public void Update()
        {
            transform.localEulerAngles = new Vector3(180, 0, _model.LocalOrientation.FlatRotation());
        }
    }
}
