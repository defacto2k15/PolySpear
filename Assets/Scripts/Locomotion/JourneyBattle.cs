using System;
using System.Collections.Generic;
using Assets.Scripts.Animation;
using Assets.Scripts.Battle;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyBattle : IJourneyStep<UnitModelComponent>
    {
        private BattleCircumstances _battleCircumstances;

        public JourneyBattle(BattleCircumstances battleCircumstances)
        {
            _battleCircumstances = battleCircumstances;
        }

        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return model.PerformBattleAtPlace(locomotionTarget.Model.Position, _battleCircumstances);
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