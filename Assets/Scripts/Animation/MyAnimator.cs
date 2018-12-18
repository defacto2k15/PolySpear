using System;
using Assets.Scripts.Animation;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public class MyAnimator
    {
        private IAnimation _animation;
        private UnitModel _animationTarget;
        private Action _completedCallback;

        public bool WeAreDuringAnimation() // remove, animator is now to use once
        {
            return _animation != null;
        }

        public void UpdateAnimation()
        {
            _animation.Update();
            if (_animation.Finished)
            {
                _animationTarget.GetComponent<UnitView>().enabled = true;
                _animation = null;
                _animationTarget = null;
                _completedCallback();
                _completedCallback = null;
            }
        }

        public void StartRotationAnimation(UnitModel targetUnit, Orientation targetOrientation, Action completedCallback)
        {
            Assert.IsFalse(WeAreDuringAnimation());
            _completedCallback = completedCallback;
            // todo, temporary solution
            _animationTarget = targetUnit;
            _animationTarget.GetComponent<UnitView>().enabled = false;
            _animation = new RotationAnimation(targetUnit, targetOrientation);
        }

        public void StartMotionAnimation(UnitModel targetUnit, MyHexPosition targetPosition, Action completedCallback)
        {
            Assert.IsFalse(WeAreDuringAnimation());
            _completedCallback = completedCallback;
            // todo, temporary solution
            _animationTarget = targetUnit;
            _animationTarget.GetComponent<UnitView>().enabled = false;
            _animation = new MotionAnimation(targetUnit, targetPosition);
        }

        public void StartDeathAnimation(UnitModel targetUnit, Action completedCallback)
        {
            Assert.IsFalse(WeAreDuringAnimation());
            _completedCallback = completedCallback;
            // todo, temporary solution
            _animationTarget = targetUnit;
            _animationTarget.GetComponent<UnitView>().enabled = false;
            _animation = new DeathAnimation(targetUnit);
        }

        public UnitModel AnimationTarget => _animationTarget;
    }
}