using System.Collections.Generic;
using Assets.Scripts.Animation;
using Assets.Scripts.Battle;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyPassiveOnlyBattle : IJourneyStep<UnitModelComponent>
    {
        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return model.PerformPassiveOnlyBattleAtPlace(locomotionTarget.Model.Position, BattleCircumstances.Step);
        }

        public IAnimation CreateAnimation(GameCourseModel model, UnitModelComponent animationTarget)
        {
            return new EmptyAnimation();
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