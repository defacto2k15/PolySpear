using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public interface IJourneyStep<T> where T : PawnModelComponent
    {
        BattleResults ApplyStepToModel(GameCourseModel model, T locomotionTarget);
        MyAnimation CreateAnimation(GameCourseModel model, T animationTarget);
        bool ShouldRemoveUnitAfterStep(GameCourseModel model);
    }
}