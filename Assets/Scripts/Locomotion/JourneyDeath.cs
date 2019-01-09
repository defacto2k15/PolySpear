using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDeath : IJourneyStep<UnitModelComponent>
    {
        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            model.FinalizeKillUnit(locomotionTarget.Model);
            return BattleResults.Empty;
        }

        public MyAnimation CreateAnimation(GameCourseModel model, UnitModelComponent animationTarget)
        {
            return new DeathAnimation(animationTarget);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return true;
        } 
    }
}