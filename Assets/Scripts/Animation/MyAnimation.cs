using System;
using Assets.Scripts.Animation;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public abstract class MyAnimation
    {
        private bool _animationStarted = false;

        protected  UnitModel _animationTarget;

        protected MyAnimation(UnitModel animationTarget)
        {
            _animationTarget = animationTarget;
        }

        public UnitModel AnimationTarget => _animationTarget;

        public void StartAnimation()
        {
            _animationStarted = true;
            _animationTarget.GetComponent<UnitView>().enabled = false;
        }

        public bool WeAreDuringAnimation()
        {
            return _animationStarted &&  !Finished;
        }

        public void UpdateAnimation()
        {
            Assert.IsTrue(_animationStarted, "Animation has not started yet");
            Update();
            if (Finished)
            {
                _animationTarget.GetComponent<UnitView>().enabled = true;
            }
        }

        protected  abstract bool Finished { get; }
        protected abstract void Update();
    }
}