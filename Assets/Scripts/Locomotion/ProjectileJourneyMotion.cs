using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class ProjectileJourneyMotion : IJourneyStep<ProjectileModel>
    {
        public MyHexPosition To;
        public BattleResults ApplyStepToModel(GameCourseModel model, ProjectileModel locomotionTarget)
        {
            model.MoveProjectile(locomotionTarget, To);
            return BattleResults.Empty;
        }

        public MyAnimation CreateAnimation(GameCourseModel model, ProjectileModel animationTarget)
        {
            return new MotionAnimation(animationTarget, To);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        }
    }
}