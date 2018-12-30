using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyBattle : IJourneyStep
    {
        public void ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
        }

        public MyAnimation CreateAnimation(UnitModel animationTarget)
        {
            return new EmptyAnimation(animationTarget);
        }

        public bool ShouldExecuteBattle => true;
    }
}