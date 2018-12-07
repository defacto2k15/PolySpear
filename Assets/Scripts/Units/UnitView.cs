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
        private GameObject _flagChild;

        public void Start()
        {
            _flagChild = transform.GetChild(0).gameObject;
            _model = GetComponent<UnitModel>();
        }

        public void Update()
        {
            UpdateView(); // very wasteful, but works!
        }

        private void UpdateView()
        {
            if (_flagChild == null) // todo more elegant solution
            {
                _flagChild = transform.GetChild(0).gameObject;
                _model = GetComponent<UnitModel>();
            }
            _flagChild.GetComponent<SpriteRenderer>().color = Constants.PlayersFlagColors[_model.Owner];
            transform.localPosition = _model.Position.GetPosition();
            transform.eulerAngles = new Vector3(90,_model.Orientation.FlatRotation,0);
        }
    }
}
