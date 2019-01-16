using System;
using System.Collections.Generic;
using Assets.Scripts.Animation;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDirector : IJourneyStep<UnitModelComponent>
    {
        public Orientation To;

        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            model.OrientUnit(locomotionTarget.Model, To);
            return BattleResults.Empty;
        }

        public IAnimation CreateAnimation(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return new RotationAnimation(locomotionTarget, To);
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

    public class JourneyStepRepeat : IJourneyStep<UnitModelComponent>
    {
        public MyHexPosition Delta;
        public Func<MyHexPosition, List<IJourneyStep<UnitModelComponent>>> NextStepsGenerator;

        public BattleResults ApplyStepToModel(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return BattleResults.Empty;
        }

        public IAnimation CreateAnimation(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            return new EmptyAnimation();
        }

        public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
        {
            return false;
        } 

        public List<IJourneyStep<UnitModelComponent>> GenerateFinalSteps(GameCourseModel model, UnitModelComponent locomotionTarget)
        {
            var target = locomotionTarget.PawnModel.Position;
            var journeySteps = new List<IJourneyStep<UnitModelComponent>>();

            if (model.IsRepeatField(target))
            {
                var nextField = target + Delta;

                if ((!model.HasTileAt(nextField)) || model.HasUnitAt(nextField))
                {
                    journeySteps.Add(new JourneyDeath());
                }
                else
                {
                    journeySteps.AddRange(NextStepsGenerator(nextField));
                    journeySteps.Add( new JourneyStepRepeat()
                    {
                        Delta = Delta,
                        NextStepsGenerator = NextStepsGenerator
                    });
                }
            }
            return journeySteps;
        }
    }
}