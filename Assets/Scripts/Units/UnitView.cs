using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class UnitView : MonoBehaviour
    {
        private UnitModel _model;
        private UnitFlagView _flagChild;

        public void Start()
        {
            _flagChild = GetComponentInChildren<UnitFlagView>();
            _model = GetComponent<UnitModel>();
            _model.OnUnitKilled += () => GameObject.Destroy(transform.gameObject);
        }

        public void Update()
        {
            UpdateView(); // very wasteful, but works!
        }

        private void UpdateView()
        {
            if (_flagChild == null) // todo more elegant solution
            {
                _flagChild = GetComponentInChildren<UnitFlagView>();
                _model = GetComponent<UnitModel>();
            }
            _flagChild.SetFlagColor( Constants.PlayersFlagColors[_model.Owner]);
            transform.localPosition = _model.Position.GetPosition();
            transform.localEulerAngles = new Vector3(90,_model.Orientation.FlatRotation(),0);
        }
    }
}
