using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDirector : IJourneyStep
    {
        public Orientation To;

        public void ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
            model.OrientUnit(locomotionTarget, To);
        }

        public MyAnimation CreateAnimation(UnitModel locomotionTarget)
        {
            return new RotationAnimation(locomotionTarget, To);
        }

        public bool ShouldExecuteBattle => false;
    }
}