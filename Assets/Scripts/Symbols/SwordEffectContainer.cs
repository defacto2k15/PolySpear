using Assets.Scripts.Units;

namespace Assets.Scripts.Symbols
{
    public class SwordEffectContainer : IEffectContainer
    {
        public SwordEffectContainer()
        {
            PassiveEffect = new EmptyEffect();
            ActiveEffect = new StrikeEffect();
            ReactEffect = new EmptyEffect();
        }

        public IEffect PassiveEffect { get; }
        public IEffect ActiveEffect { get; }
        public IEffect ReactEffect { get; }
    }
}