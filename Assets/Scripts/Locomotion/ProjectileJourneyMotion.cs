using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class ProjectileJourneyMotion : IJourneyStep<ProjectileModelComponent>
    {
        public MyHexPosition From;
        public MyHexPosition To;
        public BattleResults ApplyStepToModel(GameCourseModel model, ProjectileModelComponent locomotionTarget)
        {
            model.MoveProjectile(locomotionTarget.Model, To);
            return BattleResults.Empty;
        }

        public MyAnimation CreateAnimation(GameCourseModel model, ProjectileModelComponent animationTarget)
        {
            return new ProjectileConstantSpeedMotionAnimation(animationTarget, From, To);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        }
    }
}