using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class ProjectileJourneyHit : IJourneyStep<ProjectileModel>
    {
        public BattleResults ApplyStepToModel(GameCourseModel model, ProjectileModel locomotionTarget)
        {
            return model.PerformProjectileHitAtPlace(locomotionTarget.Position);
        }

        public MyAnimation CreateAnimation(GameCourseModel model, ProjectileModel animationTarget)
        {
            return new EmptyAnimation(animationTarget);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return true;
        }
    }
}