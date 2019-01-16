using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDisplacement : IJourneyStep<UnitModelComponent>
    {
        public MyHexPosition To;
        private IJourneyStep<UnitModelComponent> _step;

        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return GetInternalStep(model).ApplyStepToModel(model, locomotionTarget);
        }

        public IAnimation CreateAnimation(GameCourseModel model, UnitModelComponent animationTarget)
        {
            return GetInternalStep(model).CreateAnimation(model, animationTarget);
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            var sr = GetInternalStep(model).ShouldRemoveUnitAfterStep(model);
            return sr;
        }

        private IJourneyStep<UnitModelComponent> GetInternalStep(GameCourseModel model)
        {
            if (_step == null)
            {
                if (model.HasTileAt(To) && !model.HasUnitAt(To))
                {
                    _step = new JourneyMotion()
                    {
                        To = To
                    };
                }
                else
                {
                    _step = new JourneyDeath();
                }
            }
            return _step;
        }

        public List<IJourneyStep<UnitModelComponent>> GenerateFinalSteps(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return new List<IJourneyStep<UnitModelComponent>>();
        }
    }
}