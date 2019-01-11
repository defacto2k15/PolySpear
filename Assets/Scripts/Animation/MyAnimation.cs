using System;
using Assets.Scripts.Animation;
using Assets.Scripts.Units;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public abstract class MyAnimation : IAnimation
    {
        private bool _animationStarted = false;

        protected  PawnModelComponent _animationTarget;

        protected MyAnimation(PawnModelComponent animationTarget)
        {
            _animationTarget = animationTarget;
        }

        public virtual void StartAnimation()
        {
            _animationStarted = true;
            _animationTarget.GetComponent<PawnView>().Update();
            _animationTarget.GetComponent<PawnView>().enabled = false;
        }

        public virtual bool WeAreDuringAnimation()
        {
            return _animationStarted &&  !Finished;
        }

        public virtual void UpdateAnimation()
        {
            Assert.IsTrue(_animationStarted, "Animation has not started yet");
            Update();
            if (Finished)
            {
                _animationTarget.GetComponent<PawnView>().enabled = true;
            }
        }

        protected abstract bool Finished { get; }
        protected abstract void Update();
    }

    public interface IAnimation
    {
        void UpdateAnimation();
        bool WeAreDuringAnimation();
        void StartAnimation();
    }
}