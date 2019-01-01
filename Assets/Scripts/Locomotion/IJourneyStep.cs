using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public interface IJourneyStep
    {
        BattleResults ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget);
        MyAnimation CreateAnimation(GameCourseModel model, UnitModel animationTarget);
        bool ShouldRemoveUnitAfterStep(GameCourseModel model);
    }
}