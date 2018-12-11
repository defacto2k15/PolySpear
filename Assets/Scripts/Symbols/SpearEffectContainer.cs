using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class SpearEffectContainer : IEffectContainer
    {
        public SpearEffectContainer()
        {
            PassiveEffect = new StrikeEffect();
            ActiveEffect = new StrikeEffect();
            ReactEffect = new EmptyEffect();
        }

        public IEffect PassiveEffect { get; }
        public IEffect ActiveEffect { get; }
        public IEffect ReactEffect { get; }
    }
}