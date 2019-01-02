using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyPassiveOnlyBattle : IJourneyStep<UnitModel>
    {
        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
            return model.PerformPassiveOnlyBattleAtPlace(locomotionTarget.Position);
        }

        public MyAnimation CreateAnimation(GameCourseModel model, UnitModel animationTarget)
        {
            return new EmptyAnimation(animationTarget);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        } 
    }
}