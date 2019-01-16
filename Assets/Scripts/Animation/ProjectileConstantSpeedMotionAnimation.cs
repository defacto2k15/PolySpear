using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ProjectileConstantSpeedMotionAnimation : MyAnimation
    {
        private readonly MyHexPosition _startPosition;
        private readonly MyHexPosition _targetPosition;
        private float _startTime;

        public ProjectileConstantSpeedMotionAnimation(PawnModelComponent targetUnit, MyHexPosition startPosition, MyHexPosition targetPosition) :base(targetUnit)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _startTime = Time.time;
        }

        protected override bool Finished => Vector3.Distance(_targetPosition.GetPosition(), _animationTarget.transform.localPosition) < Constants.MotionEpsilon;

        protected override void Update()
        {
            _animationTarget.transform.localPosition = 
                Vector3.Lerp(_startPosition.GetPosition(), _targetPosition.GetPosition(),
                Mathf.Clamp01((Time.time - _startTime) * Constants.ConstantMotionSpeed));
        }
    }
}