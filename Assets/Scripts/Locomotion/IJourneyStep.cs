using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public interface IJourneyStep<T> where T : PawnModelComponent
    {
        BattleResults ApplyStepToModel(GameCourseModel model, T locomotionTarget);
        IAnimation CreateAnimation(GameCourseModel model, T animationTarget);
        bool ShouldRemoveUnitAfterStep(GameCourseModel model);
        List<IJourneyStep<T>> GenerateFinalSteps(GameCourseModel model, T locomotionTarget);
    }
}