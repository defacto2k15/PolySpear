using System;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class RotationAnimation : IAnimation
    {
        private readonly UnitModel _targetUnit;
        private readonly Orientation _targetOrientation;

        public RotationAnimation(UnitModel targetUnit, Orientation targetOrientation)
        {
            _targetUnit = targetUnit;
            _targetOrientation = targetOrientation;
        }

        public bool Finished => Math.Abs(_targetUnit.transform.localEulerAngles.y - _targetOrientation.FlatRotation) < Constants.RotationEpsilon;

        public void Update()
        {
            var qStart = Quaternion.Euler(_targetUnit.transform.localEulerAngles.x, _targetOrientation.FlatRotation, _targetUnit.transform.localEulerAngles.z);
            var qFinal = _targetUnit.transform.localRotation;
            _targetUnit.transform.localRotation =  Quaternion.Lerp(qStart, qFinal, Time.deltaTime * 1/Constants.RotationSpeed);
        }
    }
}