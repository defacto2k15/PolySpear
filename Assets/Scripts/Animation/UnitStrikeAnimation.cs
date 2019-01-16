using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class UnitStrikeAnimation : MyAnimation
    {
        private UnitModel _model;
        private readonly Vector3 _targetPosition;
        private readonly bool _isAttackMotion;
        private readonly Vector3 _startPosition;

        public UnitStrikeAnimation(UnitModelComponent targetUnit, Vector3 startPosition, Vector3 targetPosition, bool isAttackMotion) :base(targetUnit)
        {
            _model = targetUnit.Model;
            _targetPosition = targetPosition;
            _isAttackMotion = isAttackMotion;
            _startPosition = startPosition;
            _startPosition = targetUnit.PawnModel.Position.GetPosition();
        }

        protected override bool Finished => Vector3.Distance(_targetPosition, _animationTarget.transform.localPosition) < Constants.MotionEpsilon;

        protected override void Update()
        {
            _animationTarget.transform.localPosition = Vector3.Lerp(_animationTarget.transform.localPosition, _targetPosition,
                Time.deltaTime * Constants.MotionSpeed);
        }

        protected override void MyStart()
        {
            _animationTarget.transform.localPosition = _startPosition;
            if (_isAttackMotion)
            {
                _model.OnAttack();
            }
        }
    }
}