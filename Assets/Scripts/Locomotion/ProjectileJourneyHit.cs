using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class ProjectileJourneyHit : IJourneyStep<ProjectileModelComponent>
    {
        public BattleResults ApplyStepToModel(GameCourseModel model, ProjectileModelComponent locomotionTarget)
        {
            return model.PerformProjectileHitAtPlace(locomotionTarget.Model.Position);
        }

        public IAnimation CreateAnimation(GameCourseModel model, ProjectileModelComponent animationTarget)
        {
            return new EmptyAnimation(animationTarget);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return true;
        }
    }
}