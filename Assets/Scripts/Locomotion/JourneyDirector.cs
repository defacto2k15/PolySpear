using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDirector : IJourneyStep<UnitModelComponent>
    {
        public Orientation To;

        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            model.OrientUnit(locomotionTarget.Model, To);
            return BattleResults.Empty;
        }

        public MyAnimation CreateAnimation(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return new RotationAnimation(locomotionTarget, To);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        } 
    }
}