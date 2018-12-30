using System;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class RotationAnimation : MyAnimation
    {
        private readonly Orientation _targetOrientation;

        public RotationAnimation(UnitModel targetUnit, Orientation targetOrientation) :base(targetUnit)
        {
            _targetOrientation = targetOrientation;
        }

        protected override bool Finished => Math.Abs(_animationTarget.transform.localEulerAngles.y - _targetOrientation.FlatRotation()) < Constants.RotationEpsilon;

        protected override void Update()
        {
            var qStart = Quaternion.Euler(_animationTarget.transform.localEulerAngles.x, _targetOrientation.FlatRotation(), _animationTarget.transform.localEulerAngles.z);
            var qFinal = _animationTarget.transform.localRotation;
            _animationTarget.transform.localRotation =  Quaternion.Lerp(qStart, qFinal, Time.deltaTime * 1/Constants.RotationSpeed);
        }
    }
}