using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDirector : IJourneyStep
    {
        public Orientation To;

        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
            model.OrientUnit(locomotionTarget, To);
            return BattleResults.Empty;
        }

        public MyAnimation CreateAnimation(GameCourseModel model, UnitModel locomotionTarget)
        {
            return new RotationAnimation(locomotionTarget, To);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        } 
    }
}