using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyMotion : IJourneyStep
    {
        public MyHexPosition To;
        public void ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
            model.MoveUnit(locomotionTarget, To);
        }

        public MyAnimation CreateAnimation( UnitModel animationTarget)
        {
            return new MotionAnimation(animationTarget, To);
        }

        public bool ShouldExecuteBattle => false;
    }
}