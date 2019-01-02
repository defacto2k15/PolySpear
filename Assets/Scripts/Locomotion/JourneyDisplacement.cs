using Assets.Scripts.Game;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Locomotion
{
    public class JourneyDisplacement : IJourneyStep<UnitModel>
    {
    public MyHexPosition To;
    private IJourneyStep<UnitModel> _step;

    public BattleResults ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget)
    {
        return GetInternalStep(model).ApplyStepToModel(model, locomotionTarget);
    }

    public MyAnimation CreateAnimation(GameCourseModel model, UnitModel animationTarget)
    {
        return GetInternalStep(model).CreateAnimation(model, animationTarget);
    }

    public bool ShouldRemoveUnitAfterStep(GameCourseModel model)
    {
        var sr = GetInternalStep(model).ShouldRemoveUnitAfterStep(model);
        return sr;
    }

    private IJourneyStep<UnitModel> GetInternalStep(GameCourseModel model)
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
    }
}