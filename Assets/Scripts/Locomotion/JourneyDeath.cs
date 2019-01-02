using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDeath : IJourneyStep<UnitModel>
    {
        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
            model.FinalizeKillUnit(locomotionTarget);
            return BattleResults.Empty;
        }

        public MyAnimation CreateAnimation(GameCourseModel model, UnitModel animationTarget)
        {
            return new DeathAnimation(animationTarget);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return true;
        } 
    }
}