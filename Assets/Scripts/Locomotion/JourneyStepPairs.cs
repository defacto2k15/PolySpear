using Assets.Scripts.Units;

namespace Assets.Scripts.Locomotion
{
    public class JourneyStepPairs<T> where T : PawnModelComponent
    {
        public IJourneyStep<T> PreviousStep;
        public IJourneyStep<T> NextStep;
    }
}