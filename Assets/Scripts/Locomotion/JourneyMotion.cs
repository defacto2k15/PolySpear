using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyMotion  : IJourneyStep<UnitModel>
    {
        public MyHexPosition To;
        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
            model.MoveUnit(locomotionTarget, To);
            return BattleResults.Empty;
        }

        public MyAnimation CreateAnimation(GameCourseModel model, UnitModel animationTarget)
        {
            return new MotionAnimation(animationTarget, To);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        } 
    }
}