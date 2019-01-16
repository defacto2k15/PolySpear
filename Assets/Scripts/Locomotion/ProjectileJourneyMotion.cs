using System.Collections.Generic;
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

        public IAnimation CreateAnimation(GameCourseModel model, ProjectileModelComponent animationTarget)
        {
            return new ProjectileConstantSpeedMotionAnimation(animationTarget, From, To);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        }

        public List<IJourneyStep<ProjectileModelComponent>> GenerateFinalSteps(GameCourseModel model, ProjectileModelComponent locomotionTarget)
        {
            return new List<IJourneyStep<ProjectileModelComponent>>();
        }
    }
}