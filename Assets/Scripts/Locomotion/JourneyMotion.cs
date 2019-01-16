using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyMotion  : IJourneyStep<UnitModelComponent>
    {
        public MyHexPosition To;
        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            model.MoveUnit(locomotionTarget.Model, To);
            return BattleResults.Empty;
        }

        public IAnimation CreateAnimation(GameCourseModel model, UnitModelComponent animationTarget)
        {
            return new UnitMotionAnimation(animationTarget, To);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        }

        public List<IJourneyStep<UnitModelComponent>> GenerateFinalSteps(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
           return new List<IJourneyStep<UnitModelComponent>>(); 
        }
    }
}