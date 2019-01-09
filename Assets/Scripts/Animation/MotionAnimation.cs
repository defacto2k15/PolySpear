using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class MotionAnimation : MyAnimation
    {
        private readonly MyHexPosition _targetPosition;

        public MotionAnimation(PawnModelComponent targetUnit, MyHexPosition targetPosition) :base(targetUnit)
        {
            _targetPosition = targetPosition;
        }

        protected override bool Finished => Vector3.Distance(_targetPosition.GetPosition(), _animationTarget.transform.localPosition) < Constants.MotionEpsilon;

        protected override void Update()
        {
            _animationTarget.transform.localPosition = Vector3.Lerp(_animationTarget.transform.localPosition, _targetPosition.GetPosition(),
                Time.deltaTime * Constants.MotionSpeed);
        }
    }
}