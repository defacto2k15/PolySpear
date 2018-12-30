using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDeath : IJourneyStep
    {
        public void ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
        {
            Debug.Log("W12 APPLYTING!!");
            model.FinalizeKillUnit(locomotionTarget);
            GameObject.Destroy(locomotionTarget);
        }

        public MyAnimation CreateAnimation(UnitModel animationTarget)
        {
            return new DeathAnimation(animationTarget);
        }

        public bool ShouldExecuteBattle => false;
    }
}