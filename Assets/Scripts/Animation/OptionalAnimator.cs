using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Animation
{
    public class OptionalAnimator
    {
        private readonly bool _shouldShowAnimations;
        private MyAnimation _animation;
        private Action _callback;
        private UnitModel _animationTarget;

        public OptionalAnimator(bool shouldShowAnimations)
        {
            _shouldShowAnimations = shouldShowAnimations;
            if (_shouldShowAnimations)
            {
                //_animator = new MyAnimator();
            }
        }

        public bool WeAreDuringAnimation()
        {
                if (_shouldShowAnimations)
                {
                    return _animation.WeAreDuringAnimation();
                }
                else
                {
                    return _callback != null;
                }

        }

        public void UpdateAnimation()
        {
            if (_shouldShowAnimations)
            {
                _animation.UpdateAnimation();
            }
            else
            {
                if (_callback != null)
                {
                    _callback();
                    _callback = null;
                }
            }
        }

        public void StartRotationAnimation(UnitModel targetUnit, Orientation targetOrientation, Action completedCallback)
        {
            if (_shouldShowAnimations)
            {
                //_animator.StartRotationAnimation(targetUnit, targetOrientation, completedCallback);
            }
            else
            {
                _animationTarget = targetUnit;
                _callback = completedCallback;
            }
        }

        public void StartMotionAnimation(UnitModel targetUnit, MyHexPosition targetPosition, Action completedCallback)
        {
            if (_shouldShowAnimations)
            {
                //_animator.StartMotionAnimation(targetUnit, targetPosition, completedCallback);
            }
            else
            {
                _animationTarget = targetUnit;
                _callback = completedCallback;
            }
        }

        public void StartDeathAnimation(UnitModel targetUnit, Action completedCallback)
        {
            if (_shouldShowAnimations)
            {
                //_animator.StartDeathAnimation(targetUnit, completedCallback);
            }
            else
            {
                _animationTarget = targetUnit;
                _callback = completedCallback;
            }
        }

        public UnitModel AnimationTarget => _shouldShowAnimations ? _animation.AnimationTarget : _animationTarget;
    }
}
