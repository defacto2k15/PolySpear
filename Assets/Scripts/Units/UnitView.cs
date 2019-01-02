using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Units
{
    public class UnitView : PawnView
    {
        private UnitModel _unitModel;
        private UnitFlagView _flagChild;

        protected override void MyStart()
        {
            _flagChild = GetComponentInChildren<UnitFlagView>();
            _unitModel = GetComponent<UnitModel>();
        }

        protected override void MyUpdate()
        {
            UpdateView(); // very wasteful, but works!
        }

        private void UpdateView()
        {
            if (_flagChild == null) // todo more elegant solution
            {
                _flagChild = GetComponentInChildren<UnitFlagView>();
                _unitModel = GetComponent<UnitModel>();
            }
            _flagChild.SetFlagColor( Constants.PlayersFlagColors[_unitModel.Owner]);
        }
    }
}
