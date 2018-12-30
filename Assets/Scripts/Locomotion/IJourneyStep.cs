using Assets.Scripts.Game;
using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public interface IJourneyStep
    {
        void ApplyStepToModel(GameCourseModel model, UnitModel locomotionTarget);
        MyAnimation CreateAnimation(UnitModel animationTarget);
        bool ShouldExecuteBattle { get; } //ugly as hell, but we treat battles differently
    }
}