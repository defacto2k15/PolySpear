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
            var at = _animationTarget.transform.localPosition;
            _isAttackMotion = isAttackMotion;
            _startPosition = startPosition;
        }

        protected override bool Finished 
        {
            get
            {
                var dist = Vector3.Distance(_targetPosition,
                    _animationTarget.transform.localPosition);
                var r = dist < Constants.MotionEpsilon;
                return r;
            }
        } 

        protected override void Update()
        {
            _animationTarget.transform.localPosition = Vector3.Lerp(_animationTarget.transform.localPosition, _targetPosition,
                Time.deltaTime * Constants.StrikeMotionSpeed);
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