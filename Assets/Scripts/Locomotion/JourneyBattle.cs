using System;
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

        public MyAnimation CreateAnimation(GameCourseModel model, UnitModelComponent animationTarget)
        {
            return new EmptyAnimation(animationTarget);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        } 
    }
}