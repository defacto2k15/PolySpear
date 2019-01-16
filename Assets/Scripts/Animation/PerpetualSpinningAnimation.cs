using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class PerpetualSpinningAnimation : IAnimation
    {
        private readonly PawnModelComponent _animationTarget;

        public PerpetualSpinningAnimation(PawnModelComponent animationTarget)
        {
            _animationTarget = animationTarget;
        }

        public void UpdateAnimation()
        {
            var oldRotation = _animationTarget.transform.localEulerAngles;
            _animationTarget.transform.localEulerAngles = new Vector3(oldRotation.x, oldRotation.y,
                oldRotation.z + Time.deltaTime*Constants.SpinningSpeed);
        }

        public bool WeAreDuringAnimation()
        {
            return false;
        }

        public void StartAnimation()
        {
        }
    }
}