using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class SpinningAxeJourneyMotion : IJourneyStep<ProjectileModelComponent>
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
            return new CompositeAnimation( new List<IAnimation>()
            {
                new ProjectileConstantSpeedMotionAnimation(animationTarget, From, To),
                new PerpetualSpinningAnimation(animationTarget)
            });
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        }
    }
}