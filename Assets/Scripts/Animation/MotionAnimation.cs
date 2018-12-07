using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class MotionAnimation : IAnimation
    {
        private readonly UnitModel _targetUnit;
        private readonly MyHexPosition _targetPosition;

        public MotionAnimation(UnitModel targetUnit, MyHexPosition targetPosition)
        {
            _targetUnit = targetUnit;
            _targetPosition = targetPosition;
        }

        public bool Finished => Vector3.Distance(_targetPosition.GetPosition(), _targetUnit.transform.localPosition) < Constants.MotionEpsilon;

        public void Update()
        {
            _targetUnit.transform.localPosition = Vector3.Lerp(_targetUnit.transform.localPosition, _targetPosition.GetPosition(),
                Time.deltaTime * Constants.MotionSpeed);
        }
    }
}