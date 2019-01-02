using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Animation
{
    public class EmptyAnimation : MyAnimation
    {
        public EmptyAnimation(PawnModel animationTarget) : base(animationTarget)
        {
        }

        protected override bool Finished => true;
        protected override void Update()
        {
        }
    }
}